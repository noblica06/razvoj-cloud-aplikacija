using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace RedditService
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Enable CORS
            var cors = new EnableCorsAttribute("http://localhost:3000", "*", "*");
            config.EnableCors(cors);

            // Configure Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Use Web API
            app.UseWebApi(config);
        }
    }
    public class AllowAllCorsPolicyProvider : System.Web.Http.Cors.ICorsPolicyProvider
    {
        readonly System.Web.Cors.CorsPolicy _CorsPolicy;

        public AllowAllCorsPolicyProvider()
        {
            _CorsPolicy = new System.Web.Cors.CorsPolicy { AllowAnyHeader = true, AllowAnyMethod = true, AllowAnyOrigin = true };
        }
        Task<System.Web.Cors.CorsPolicy> System.Web.Http.Cors.ICorsPolicyProvider.GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_CorsPolicy); ;
        }
    }
}
