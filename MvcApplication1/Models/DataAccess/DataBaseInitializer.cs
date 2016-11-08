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
            user.Email = "admin@mail.com";
            user.Password = "123";
            user.IsAdmin = true;       
            context.Users.Add(user);

            
            #endregion          

            context.SaveChanges();
        }
    }
}