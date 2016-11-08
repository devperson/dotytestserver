using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Request
{
    public class NewEventRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTimeStringUtc { get; set; }
        public string EndTimeStringUtc { get; set; }
        public string EventColor { get; set; }
    }

    public class UpdateEventRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTimeStringUtc { get; set; }
        public string EndTimeStringUtc { get; set; }
        public string EventColor { get; set; }
    }
}