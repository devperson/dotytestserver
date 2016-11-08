using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Response
{
    public class EventResponse
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTimeStringUtc { get; set; }
        public string EndTimeStringUtc { get; set; }
        public string EventColor { get; set; }
    }
}