﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RedditService
{
   
    class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            //return new HttpResponseMessage()
            //{
            //    Content = new StringContent("Hello from OWIN!")
            //};
            return Ok("Hello from OWIN");
        }

        public HttpResponseMessage Get(int id)
        {
            string msg = String.Format("Hello from OWIN (id = {0})", id);
            return new HttpResponseMessage()
            {
                Content = new StringContent(msg)
            };
        }
    }
}
