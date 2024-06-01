using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RSMVC.Startup))]
namespace RSMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
