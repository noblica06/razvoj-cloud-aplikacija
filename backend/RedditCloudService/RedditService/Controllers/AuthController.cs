﻿using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using RedditService.DTOs;
using RedditService.Models;
using RedditService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RedditService.Controllers
{
    //[EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        UserDataRepository repository = new UserDataRepository();

        [HttpGet]
        [Route("auth/get")]
        public IHttpActionResult Get()
        {
            return Ok("TU SAM");
        }

        [HttpPost]
        [Route("auth/login")]
        public IHttpActionResult LoginUser([FromBody] LoginUserDTO loginUserDTO)
        {
            var user = repository.GetUser(loginUserDTO.Email);
            if (user == null) return BadRequest("User with this email does not exist!");

            if (user.Password != loginUserDTO.Password) return BadRequest("Wrong email or password!");

            return Ok(new LoginResponseDTO() { Email = loginUserDTO.Email});
        }

        [HttpPost]
        [Route("auth/register")]
        public IHttpActionResult RegisterUser([FromBody] RegisterUserDTO userDTO)
        {
            try
            {
                if (!repository.Exists(userDTO.Email))
                {
                    string uniqueBlobName = string.Format("image_{0}", userDTO.Email);
                    var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                    CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                    CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                    //blob.Properties.ContentType = userDTO.File.ContentType;
                    // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                    //blob.UploadFromStream(userDTO.File.InputStream);
                    // upis studenta u table storage koristeci StudentDataRepository klasu


                    User newUser = new User(userDTO.Email)
                    {
                        FirstName = userDTO.FirstName,
                        LastName = userDTO.LastName,
                        Address = userDTO.Address,
                        City = userDTO.City,
                        Country = userDTO.Country,
                        PhoneNumber = userDTO.PhoneNumber,
                        Password = userDTO.Password,
                        // PhotoUrl = blob.Uri.ToString(),
                        //ThumbnailUrl = blob.Uri.ToString()
                    };
                    repository.AddUser(newUser);
                    CloudQueue queue = QueueHelper.GetQueueReference("vezba");
                    queue.AddMessage(new CloudQueueMessage(userDTO.Email), null, TimeSpan.FromMilliseconds(30));

                    return Ok("Successfully registered."); //token
                }
                return Conflict();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
