using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ThubnailPath { get; set; }
                                           //public int UserID { get; set; }

        //public User User { get; set; }
    }
}