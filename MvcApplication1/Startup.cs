using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(GamifiedAppServer.Startup))]

namespace GamifiedAppServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   Audience = ConfigurationManager.AppSettings["ida:Audience"],
                   Tenant = ConfigurationManager.AppSettings["ida:Tenant"]
               });
        }
    }
}
