using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotyAppServer.Models.Response
{
    public class LoginUserResponse : ModelWithStatus
    {
        public int UserID { get; set; }        
        public string Image { get; set; }
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UserResponse : ModelWithStatus
    {
        public int UserID { get; set; }
        public string Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }        
    }

    public class ImageResponse : ModelWithStatus
    {
        public string Image { get; set; }
    }
}