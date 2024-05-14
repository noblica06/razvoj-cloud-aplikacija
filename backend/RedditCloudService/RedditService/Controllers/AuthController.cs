using RedditService.Models;
using RedditService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RedditService.Controllers
{
    public class AuthController : ApiController
    {
        UserDataRepository repository = new UserDataRepository();
        [HttpPost]
        [Route("[controller]/register")]
        public IHttpActionResult RegisterUser([FromBody] User user)
        {
            if(!repository.Exists(user.Email))
            {
                User user1 = new User(user.Email)
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    PhoneNumber = user.PhoneNumber,
                    Password = user.Password,
                    PhotoUrl = user.PhotoUrl,
                    ThumbnailUrl = user.ThumbnailUrl
                };
                repository.Exists(user.Email);

                repository.AddUser(user1);
                return Ok("Successfully registered."); //token
            }
                return Conflict();
        }
    }
}
