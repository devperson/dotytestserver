using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Response
{
    public class IDResponse : ModelWithStatus
    {
        public int ID { get; set; }
    }
}