using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using RSMVC.DataRepo;
using RSMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RSMVC.Controllers
{
    public class ManageController : Controller
    {
        UserDataRepo userDataRepo = new UserDataRepo();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //return  HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Manage
        public ActionResult Index(string email)
        {
            UserData loggdinUser = userDataRepo.GetUserData(email);


            return View(loggdinUser);
        }

        public async Task<ActionResult> UpdateAccount(UpdateProfileViewModel updateViewModel)
        {
            UserData loggdinUser = userDataRepo.GetUserData(updateViewModel.Email);

            loggdinUser.Address = updateViewModel.Address;
            loggdinUser.City = updateViewModel.City;
            loggdinUser.Country = updateViewModel.Country;
            loggdinUser.FirstName = updateViewModel.FirstName;
            loggdinUser.LastName = updateViewModel.LastName;
            loggdinUser.PhoneNumber = updateViewModel.PhoneNumber;
            if(updateViewModel.File != null)
            {
                string uniqueBName = updateViewModel.Email.Split('@')[0];
                string uniqueBlobName = string.Format("image_{0}", uniqueBName);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.DeleteIfExists();
                CloudBlockBlob blobNew = container.GetBlockBlobReference(uniqueBlobName);
                blobNew.Properties.ContentType = updateViewModel.File.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blobNew.UploadFromStream(updateViewModel.File.InputStream);
                // upis studenta u table storage koristeci StudentDataRepository klasu
                loggdinUser.PhotoUrl = blobNew.Uri.ToString();
                loggdinUser.ThumbnailUrl = blobNew.Uri.ToString();
            }
            userDataRepo.UpdateUserData(loggdinUser);

            //var user = UserManager.FindByEmailAsync(registerViewModel.Email);

            var user = await UserManager.FindByNameAsync(updateViewModel.Email);

            user.Address = updateViewModel.Address;
            user.City = updateViewModel.City;
            user.Country = updateViewModel.Country;
            user.FirstName = updateViewModel.FirstName;
            user.LastName = updateViewModel.LastName;
            user.PhoneNumber = updateViewModel.PhoneNumber;

            await UserManager.UpdateAsync(user);

            return View("Index");


            //var user = _userManager.FindAsync()

            //CloudQueue queue = QueueHelper.GetQueueReference("vezba");
            //queue.AddMessage(new CloudQueueMessage(registerViewModel.Email), null, TimeSpan.FromMilliseconds(30));

        }
    }
}