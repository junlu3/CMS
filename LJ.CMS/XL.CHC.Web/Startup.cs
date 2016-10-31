using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XL.CHC.Web.Startup))]
namespace XL.CHC.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
