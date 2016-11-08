﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models
{
    public class User
    {
        public User()
        {
            this.Documents = new List<Document>();                        
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }        
        public string Image { get; set; }
        public string Platform { get; set; }
        public string DeviceToken { get; set; }
        public bool IsAdmin { get; set; }


        public List<Message> SentMessages { get; set; }
        public List<Document> Documents { get; set; }        
    }
}