using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using DotyAppServer.Models;

namespace DotyAppServer.DataAccess
{
    /// <summary>
    /// This class used before to make connection to DataBase, 
    /// here we can make configurations such as CreateDatabaseIfNotExists, DropCreateDatabaseAlways, DropCreateDatabaseIfModelChanges
    /// </summary>
    public class DataBaseInitializer : CreateDatabaseIfNotExists<DataBaseContext>
    {

        /// <summary>
        /// Populates database tables with default data if any, note this method will only run when database is being created.
        /// </summary>
        protected override void Seed(DataBaseContext context)
        {
         
            #region Users                        
            var user = new User();
            user.FirstName = "Tom";
            user.LastName = "Wilson";
            user.Email = "t1@g.com";
            user.Password = "123";
            user.Image = ToUrl("/Images/avatar.png");
            context.Users.Add(user);

            var user2 = new User();
            user2.FirstName = "Martin";  
            user2.LastName = "Knotzer";
            user2.Email = "t2@g.com";
            user2.Password = "123";
            user2.Image = ToUrl("/Images/avatar.png");
            context.Users.Add(user2);
            #endregion

            context.SaveChanges();
        }

        
        protected string ToUrl(string relativeUrl)
        {
            var requestUrl = HttpContext.Current.Request.Url;
            var port = (requestUrl.AbsoluteUri.Contains("localhost") || requestUrl.AbsoluteUri.Contains("desktop") || requestUrl.AbsoluteUri.Contains("192.168")) ? (":" + requestUrl.Port) : String.Empty;
            var url = String.Format("{0}://{1}{2}{3}", requestUrl.Scheme, requestUrl.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
            return url;
        }
    }
}