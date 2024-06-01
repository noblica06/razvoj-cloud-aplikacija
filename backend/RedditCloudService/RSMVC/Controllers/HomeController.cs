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

namespace RSMVC.Controllers
{
    public class HomeController : Controller
    {
        ThemeDataRepository repo = new ThemeDataRepository();
        CommentDataRepo repoComment = new CommentDataRepo();

        public ActionResult Index()
        {
            List<Theme> themes = repo.RetrieveAllThemes().ToList();

            return View(themes);
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

        public ActionResult CreateTheme(CreateThemeViewModel newTheme)
        {
            try
            {
                //Theme theme = new Theme() { Title = newTheme.Title, UserEmail = newTheme.UserEmail, Description = newTheme.Description, CreatedDate = DateTime.Now, }
                if (repo.Exists(newTheme.Title))
                {
                    return View("Error");
                }

                // kreiranje blob sadrzaja i kreiranje blob klijenta
                string uniqueBlobName = string.Format("image_{0}", newTheme.Title);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = newTheme.File.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(newTheme.File.InputStream);
                // upis studenta u table storage koristeci StudentDataRepository klasu
                Theme entry = new Theme(newTheme.Title) { UserEmail = newTheme.UserEmail, Description = newTheme.Description, CreatedDate = DateTime.Now, PhotoUrl = blob.Uri.ToString(), ThumbnailUrl = blob.Uri.ToString() };
                repo.AddTheme(entry);

                CloudQueue queue = QueueHelper.GetQueueReference("vezba");
                queue.AddMessage(new CloudQueueMessage(newTheme.Title), null, TimeSpan.FromMilliseconds(30));

                return RedirectToAction("Index");
            }
            catch
            {
                return View("AddTheme");
            }
        }

       public ActionResult Details(string title)
       {
            Theme theme = repo.GetTheme(title);
            List<Comment> comments = new List<Comment>();
            try {
                comments = repoComment.RetrieveAllComments().Where(c => c.ThemeTitle == title).ToList();
            }
            catch(Exception e)
            {
                comments = null;
            }
            

            ThemeAndComments model = new ThemeAndComments() { Theme = theme, Comments = comments };

            return View(model);
       }

       public ActionResult AddComment(string content, string themeTitle, string userEmail)
       {
            string guid = Guid.NewGuid().ToString();
            Comment comment = new Comment(guid) { Content = content, ThemeTitle = themeTitle, UserEmail = userEmail };


            repoComment.AddComment(comment);

            CloudQueue queue = QueueHelper.GetQueueReference("vezba");
            queue.AddMessage(new CloudQueueMessage(comment.Guid), null, TimeSpan.FromMilliseconds(30));

            return RedirectToAction("Details", new { title = themeTitle });
        }
    }
}