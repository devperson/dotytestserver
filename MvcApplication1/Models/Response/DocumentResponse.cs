using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Response
{
    public class DocumentResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Thumbnail { get; set; }
    }

    public class NewDocumentResponse : ModelWithStatus
    {
        public int ID { get; set; }
        public string Link { get; set; }
        public string Thumbnail { get; set; }
    }
}