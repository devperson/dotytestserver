using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DotyAppServer.Models;

namespace DotyAppServer.DataAccess
{
    public class MessageModelConfig : EntityTypeConfiguration<Message>
    {
        public MessageModelConfig()
        {            
            this.HasRequired(m => m.FromUser).WithMany(u => u.SentMessages).HasForeignKey(m => m.FromUser_Id).WillCascadeOnDelete(false);            
        }
    }

    //public class UserModelConfig : EntityTypeConfiguration<User>
    //{
    //    public UserModelConfig()
    //    {
    //        this.HasMany(u => u.Documents).WithRequired(d => d.User).HasForeignKey(d => d.UserID).WillCascadeOnDelete(false);
    //    }
    //}
}