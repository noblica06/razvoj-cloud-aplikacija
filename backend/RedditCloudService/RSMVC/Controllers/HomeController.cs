using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using RSMVC.DataRepo;
using RSMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RSMVC.DataRepo.ThemeVotersDataRepo;

namespace RSMVC.Controllers
{
    public class HomeController : Controller
    {
        ThemeDataRepository repo = new ThemeDataRepository();
        CommentDataRepo repoComment = new CommentDataRepo();
        UserDataRepo repoUser = new UserDataRepo();
        ThemeVotersDataRepo repoThemeVoters = new ThemeVotersDataRepo();
        ThemeSubscribersDataRepo repoThemeSubscribers = new ThemeSubscribersDataRepo();

        public ActionResult Index(List<Theme> listOfThemes = null, int page = 0)
        {
            List<Theme> themes = null;
            if (listOfThemes == null)
            {
                themes = repo.RetrieveAllThemes().ToList();
            }
            else
            {
                themes = listOfThemes;
            }

            if (themes != null)
            {
                foreach (Theme theme in themes)
                {
                    theme.Upvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Upvoted && v.ThemeTitle == theme.Title)
                                .Select(v => v.UserEmail)
                                .ToList();
                    theme.Downvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Downvoted && v.ThemeTitle == theme.Title)
                            .Select(v => v.UserEmail)
                            .ToList();
                }
            }

            const int PageSize = 3; 

            var count = themes.Count();

            var data = themes.Skip(page * PageSize).Take(PageSize).ToList();

            this.ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            this.ViewBag.Page = page;

            return View(data);
        }

        public ActionResult Order(string order, int page=0)
        {
            List<Theme> themeList = repo.RetrieveAllThemes().ToList();
            if (order == "ascending")
            {
                themeList = themeList.OrderBy(theme => theme.UpVotes).ThenByDescending(theme => theme.DownVotes).ToList();
            }
            else
            {
                themeList = themeList.OrderByDescending(theme => theme.UpVotes).ThenBy(theme => theme.DownVotes).ToList();
            }

            foreach (Theme theme in themeList)
            {
                theme.Upvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Upvoted && v.ThemeTitle == theme.Title)
                            .Select(v => v.UserEmail)
                            .ToList();
                theme.Downvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Downvoted && v.ThemeTitle == theme.Title)
                        .Select(v => v.UserEmail)
                        .ToList();
            }

            const int PageSize = 3;

            var count = themeList.Count();

            var data = themeList.Skip(page * PageSize).Take(PageSize).ToList();

            this.ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            this.ViewBag.Page = page;

            return View("Index", data);
        }

        public ActionResult Search(string searchTerm, int page=0)
        {
            List<Theme> themeList = new List<Theme>();
            if(searchTerm != "")
            {
                themeList = repo.RetrieveAllThemes().ToList().Where(theme => theme.Title.ToLower().Contains(searchTerm.ToLower())).ToList();
            }
            else
            {
                themeList = repo.RetrieveAllThemes().ToList();
            }
            foreach (Theme theme in themeList)
            {
                theme.Upvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Upvoted && v.ThemeTitle == theme.Title)
                            .Select(v => v.UserEmail)
                            .ToList();
                theme.Downvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Downvoted && v.ThemeTitle == theme.Title)
                        .Select(v => v.UserEmail)
                        .ToList();
            }

            const int PageSize = 3;

            var count = themeList.Count();

            var data = themeList.Skip(page * PageSize).Take(PageSize).ToList();

            this.ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            this.ViewBag.Page = page;

            return View("Index", data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddTheme()
        {
            return View();
        }
        [Authorize]
        public ActionResult CreateTheme(CreateThemeViewModel newTheme)
        {
            try
            { 
                if (repo.Exists(newTheme.Title))
                {
                    return View("Error");
                }

                string themeTitle = newTheme.Title.Replace(' ', '_');
                // kreiranje blob sadrzaja i kreiranje blob klijenta
                string uniqueBlobName = string.Format("image_{0}", themeTitle);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = newTheme.File.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(newTheme.File.InputStream);
                // upis studenta u table storage koristeci StudentDataRepository klasu
                Theme entry = new Theme(newTheme.Title) { Title = newTheme.Title, UserEmail = newTheme.UserEmail, Description = newTheme.Description, CreatedDate = DateTime.Now, PhotoUrl = blob.Uri.ToString(), ThumbnailUrl = blob.Uri.ToString() };
                repo.AddTheme(entry);

                return RedirectToAction("Index");
            }
            catch
            {
                return View("AddTheme");
            }
        }

        [Authorize]
        public ActionResult RemoveTheme(string themeTitle, string userEmail)
        {
            bool themeExists = repo.Exists(themeTitle);
            if (themeExists)
            {
                Theme theme = repo.GetTheme(themeTitle);
                if(theme.UserEmail == userEmail)
                {
                    repo.RemoveTheme(themeTitle);

                    List<Comment> commentsToDelete = repoComment.RetrieveAllComments().Where(c => c.ThemeTitle == themeTitle).ToList();
                    foreach(Comment comment in commentsToDelete)
                    {
                        repoComment.RemoveComment(comment.Guid);
                    }

                    List<ThemeSubscriber> themeSubscribersToDelete = repoThemeSubscribers.RetrieveAllThemeSubscribers().Where(v => v.ThemeTitle == themeTitle).ToList();
                    foreach(ThemeSubscriber themeSubscriber in themeSubscribersToDelete)
                    {
                        repoThemeSubscribers.RemoveThemeSubscriber(themeSubscriber.RowKey);
                    }


                    List<ThemeVoters> themeVotersToDelete = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.ThemeTitle == themeTitle).ToList();
                    foreach (ThemeVoters themeVoters in themeVotersToDelete)
                    {
                        repoThemeVoters.RemoveThemeVoters(themeVoters.RowKey);
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult RemoveComment(string commentGuid, string userEmail)
        {
            bool commentExists = repoComment.Exists(commentGuid);
            string themeTitle = repoComment.GetComment(commentGuid).ThemeTitle;
            if (commentExists)
            {
                repoComment.RemoveComment(commentGuid);
               
            }
            return RedirectToAction("Details", new { title = themeTitle });
        }

        public ActionResult Details(string title="")
       {
            if(title == "" || title == null)
            {
                RedirectToAction("Index");
            }
            Theme theme = repo.GetTheme(title);
            List<Comment> comments = new List<Comment>();
            List<string> upvoters = new List<string>();
            List<string> downvoters = new List<string>();
            List<string> subscribers = new List<string>();
            string userThumbnailUrl = "";

            try {
                comments = repoComment.RetrieveAllComments().Where(c => c.ThemeTitle == title).ToList();
                upvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Upvoted && v.ThemeTitle == title)
                            .Select(v => v.UserEmail)
                            .ToList();
                downvoters = repoThemeVoters.RetrieveAllThemeVoters().Where(v => v.Downvoted && v.ThemeTitle == title)
                            .Select(v => v.UserEmail)
                            .ToList();
                subscribers = repoThemeSubscribers.RetrieveAllThemeSubscribers().Where(v => v.ThemeTitle == title)
                            .Select(v => v.UserEmail)
                            .ToList();
                userThumbnailUrl = repoUser.GetUserData(theme.UserEmail).ThumbnailUrl;
            }
            catch(Exception e)
            {
                comments = null;
            }


            ThemeAndComments model = new ThemeAndComments()
            { Theme = theme,
                Comments = comments,
                Upvoters = upvoters,
                Downvoters = downvoters,
                Subscribers = subscribers,
                UserThumbnailUrl = userThumbnailUrl
            };
            if(model.Theme == null)
            {
                RedirectToAction("Index");
            }

            return View(model);
       }

        [Authorize]
        public ActionResult AddComment(string content, string themeTitle, string userEmail)
       {
            string guid = Guid.NewGuid().ToString();
            Comment comment = new Comment(guid) { Content = content, ThemeTitle = themeTitle, UserEmail = userEmail };
            comment.UserImage = repoUser.GetUserData(userEmail).ThumbnailUrl;

            repoComment.AddComment(comment);

            CloudQueue queue = QueueHelper.GetQueueReference("commentnotificationqueue");
            queue.AddMessage(new CloudQueueMessage(guid), null, TimeSpan.FromMilliseconds(30));

            return RedirectToAction("Details", new { title = themeTitle });
        }

        //Upvotes 
        [Authorize]
        public ActionResult AddUpvote(string themeTitle, string userEmail)
        {
            Theme theme = repo.GetTheme(themeTitle);
            
            string titleEmail= $"{themeTitle}_{userEmail.Split('@')[0]}";

            bool themeVotersExist = repoThemeVoters.Exists(titleEmail);

            if (themeVotersExist)
            {
                ThemeVoters themeVoters = repoThemeVoters.GetThemeVoters(titleEmail);
                themeVoters.Upvoted = true;
                repoThemeVoters.UpdateThemeVoters(themeVoters);
            }
            else
            {
                ThemeVoters themeVoters = new ThemeVoters(titleEmail)
                {
                    ThemeTitle = themeTitle,
                    UserEmail = userEmail,
                    Upvoted = true,
                    Downvoted = false
                };
                repoThemeVoters.AddThemeVote(themeVoters);
            };

            theme.UpVotes += 1;
            repo.UpdateTheme(theme);

            return RedirectToAction("Details", new { title = themeTitle });
        }
        [Authorize]
        public ActionResult RemoveUpvote(string themeTitle, string userEmail)
        {
            Theme theme = repo.GetTheme(themeTitle);
            

            string titleEmail = $"{themeTitle}_{userEmail.Split('@')[0]}";

            bool themeVotersExist = repoThemeVoters.Exists(titleEmail);

            if (themeVotersExist)
            {
                ThemeVoters themeVoters = repoThemeVoters.GetThemeVoters(titleEmail);
                themeVoters.Upvoted = false;
                repoThemeVoters.UpdateThemeVoters(themeVoters);

                theme.UpVotes -= 1;
                repo.UpdateTheme(theme);
            }

            return RedirectToAction("Details", new { title = themeTitle });
        }

        //Downvotes
        [Authorize]
        public ActionResult AddDownvote(string themeTitle, string userEmail)
        {
            Theme theme = repo.GetTheme(themeTitle);

            string titleEmail = $"{themeTitle}_{userEmail.Split('@')[0]}";

            bool themeVotersExist = repoThemeVoters.Exists(titleEmail);

            if (themeVotersExist)
            {
                ThemeVoters themeVoters = repoThemeVoters.GetThemeVoters(titleEmail);
                themeVoters.Downvoted = true;
                repoThemeVoters.UpdateThemeVoters(themeVoters);
            }
            else
            {
                ThemeVoters themeVoters = new ThemeVoters(titleEmail)
                {
                    ThemeTitle = themeTitle,
                    UserEmail = userEmail,
                    Upvoted = false,
                    Downvoted = true
                };
                repoThemeVoters.AddThemeVote(themeVoters);
            };

            theme.DownVotes += 1;
            repo.UpdateTheme(theme);

            return RedirectToAction("Details", new { title = themeTitle });
        }
        [Authorize]
        public ActionResult RemoveDownvote(string themeTitle, string userEmail)
        {
            Theme theme = repo.GetTheme(themeTitle);


            string titleEmail = $"{themeTitle}_{userEmail.Split('@')[0]}";

            bool themeVotersExist = repoThemeVoters.Exists(titleEmail);

            if (themeVotersExist)
            {
                ThemeVoters themeVoters = repoThemeVoters.GetThemeVoters(titleEmail);
                themeVoters.Downvoted = false;
                repoThemeVoters.UpdateThemeVoters(themeVoters);

                theme.DownVotes -= 1;
                repo.UpdateTheme(theme);
            }

            return RedirectToAction("Details", new { title = themeTitle });
        }

        //Subscribes
        [Authorize]
        public ActionResult AddSubscription(string themeTitle, string userEmail)
        {
            Theme theme = repo.GetTheme(themeTitle);

            string titleEmail = $"{themeTitle}_{userEmail.Split('@')[0]}";

            bool themeSubscriberExist = repoThemeSubscribers.Exists(titleEmail);

            if (!themeSubscriberExist)
            {
                ThemeSubscriber themeSubscriber = new ThemeSubscriber(titleEmail)
                {
                    ThemeTitle = themeTitle,
                    UserEmail = userEmail,
                };
                repoThemeSubscribers.AddThemeSubscriber(themeSubscriber);
            }

            return RedirectToAction("Details", new { title = themeTitle });
        }
        [Authorize]
        public ActionResult RemoveSubscription(string themeTitle, string userEmail)
        {
            Theme theme = repo.GetTheme(themeTitle);


            string titleEmail = $"{themeTitle}_{userEmail.Split('@')[0]}";

            bool themeSubscribersExist = repoThemeSubscribers.Exists(titleEmail);

            if (themeSubscribersExist)
            {
                repoThemeSubscribers.RemoveThemeSubscriber(titleEmail);
            }

            return RedirectToAction("Details", new { title = themeTitle });
        }
    }
}