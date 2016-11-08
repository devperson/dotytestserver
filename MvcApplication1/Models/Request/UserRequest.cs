
namespace DotyAppServer.Models.Request
{
    public class LoginUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }         
    }

    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Platform { get; set; }
        public string DeviceToken { get; set; }
    }

    public class UpdateUserRequest
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }        
        /// <summary>
        /// Url to Image
        /// </summary>
        public string Image { get; set; }
        //public string PictureUrl { get; set; }
        public string SocialToken { get; set; }
        public string Platform { get; set; }
        public string DeviceToken { get; set; }
    }

    public class UpdateUserTokenRequest
    {
        public int UserID { get; set; }
        public string DeviceToken { get; set; }
    }

    public class ChangePasswordRequest
    {
        public int UserID { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}