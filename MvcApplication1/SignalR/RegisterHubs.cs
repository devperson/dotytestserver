using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ArchivesSocietyAB.RegisterHubs))]
namespace ArchivesSocietyAB
{   
    public class RegisterHubs
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
