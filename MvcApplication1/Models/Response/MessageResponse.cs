using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Response
{
    public class MessageResponse
    {
        //public int ID { get; set; }
        public string Identifier { get; set; }
        public string Message { get; set; }
        public UserResponse Sender { get; set; }
        public string CreatedDateStringUtc { get; set; }

        //public int ID { get; set; }

        //public string Message { get; set; }
        //public int SenderID { get; set; }
        //public string DateString { get; set; }
    }

    public class NewMessageResponse : ModelWithStatus
    {
        public int MessageID { get; set; }
        public string CreatedDateStringUtc { get; set; }        
    }
}