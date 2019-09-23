using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TransporteMorande.Startup))]
namespace TransporteMorande
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}