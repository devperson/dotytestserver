using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Response
{
    public class ModelWithStatus
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ResultWithStatus<T> : ModelWithStatus
    {
        public T Result { get; set; }
    }

    public class ListResponse<T> : ModelWithStatus
    {
        public List<T> ListResult { get; set; }
    }
}