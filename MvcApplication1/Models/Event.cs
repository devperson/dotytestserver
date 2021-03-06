﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models
{
    public class Event
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EventColor { get; set; }
    }
}