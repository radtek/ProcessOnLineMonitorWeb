using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(POLM.Startup))]
namespace POLM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
