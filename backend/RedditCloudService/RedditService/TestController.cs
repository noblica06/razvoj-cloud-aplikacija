using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RedditService
{
   
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("test/get")]
        public IHttpActionResult Get()
        {
            //return new HttpResponseMessage()
            //{
            //    Content = new StringContent("Hello from OWIN!")
            //};
            return Ok("Hello from OWIN");
        }

        
    }
}
