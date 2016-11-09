using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DotyAppServer.Models;

namespace DotyAppServer.DataAccess
{
    /// <summary>
    /// This is EF data context class it holds all tables.
    /// </summary>
    public class DataBaseContext : DbContext
    {
        public DataBaseContext()
            : base("DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
        }
        
        static DataBaseContext()
        {
            Database.SetInitializer<DataBaseContext>(new DataBaseInitializer());
        }

        /// <summary>
        /// Configure table to object mapping.
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new MessageModelConfig());            
            //modelBuilder.Configurations.Add(new UserModelConfig());            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}