using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PharmaCoHealth.Startup))]
namespace PharmaCoHealth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
