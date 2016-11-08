using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }      
        public DateTime Date { get; set; }        
        public int FromUser_Id { get; set; }
        public string Identifier { get; set; }

        public User FromUser { get; set; }        
    }
}