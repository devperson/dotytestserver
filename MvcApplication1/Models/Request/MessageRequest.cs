using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Request
{
    public class MessageRequest
    {
        public int SenderId { get; set; }        
        public string Message { get; set; }
        public string Identifier { get; set; }
    }
}