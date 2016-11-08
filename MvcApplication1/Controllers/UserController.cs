using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using DotyAppServer.DataAccess;
using DotyAppServer.Models;
using DotyAppServer.Models.Request;
using DotyAppServer.Models.Response;
using DotyAppServer.Utility;

namespace DotyAppServer.Controllers
{
    public class UserController : BaseApiController
    {       
        [HttpPost]
        [ActionName("Login")]
        public LoginUserResponse Login(LoginUserRequest request)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var user = db.Users.Where(u => u.Email == request.Email && u.Password == request.Password).FirstOrDefault();
                    if (user != null)
                    {
                        return new LoginUserResponse { UserID = user.Id, FirstName = user.FirstName, LastName = user.LastName, Image = user.Image, IsAdmin = user.IsAdmin, Success = true };
                    }
                    else
                        return new LoginUserResponse { ErrorMessage = "Email or password incorrect." };
                }                
            }
            catch (Exception ex)
            {
                return new LoginUserResponse { ErrorMessage = ex.ToString() };
            }
        }

        [HttpPost]
        [ActionName("CreateUser")]
        public IDResponse CreateUser(CreateUserRequest request)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var any = db.Users.Any(u => u.Email == request.Email);
                    if (any)
                        return new IDResponse { ErrorMessage = "User with such email already exist.", Success = false };
                    
                    var userDb = new User();
                    userDb.FirstName = request.FirstName;
                    userDb.LastName = request.LastName;
                    userDb.Password = request.Password;
                    userDb.Email = request.Email;
                    userDb.Platform = request.Platform;
                    userDb.DeviceToken = request.DeviceToken;                    
                    db.Users.Add(userDb);
                    db.SaveChanges();

                    return new IDResponse { ID = userDb.Id, Success = true };
                }
            }
            catch (Exception ex)
            {
                return new IDResponse { ErrorMessage = ex.ToString(), Success = false };
            }
        }

        [HttpPost]
        [ActionName("UpdateUser")]
        public ModelWithStatus UpdateUser(UpdateUserRequest request)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var userDb = db.Users.Single(u => u.Id == request.ID);
                    userDb.FirstName = request.FirstName;
                    userDb.LastName = request.LastName;
                    userDb.Email = request.Email;
                    userDb.Image = request.Image;
                    db.SaveChanges();                    
                }

                return this.ReturnSuccess();
            }
            catch (Exception ex)
            {
                return this.ReturnFailed(ex);
            }
        }        


        //[HttpGet]
        //[ActionName("GetChatUsers")]
        //public ListResponse<UserResponse> GetChatUsers(int userID)
        //{
        //    try
        //    {
        //        using (var db = new DataBaseContext())
        //        {
        //            var list = db.Users.Where(u => u.Id != userID).Select(s => new UserResponse
        //            {
        //                UserID = s.Id,
        //                FirstName = s.FirstName,
        //                LastName = s.LastName,
        //                Email = s.Email,
        //                Image = s.Image
        //            }).ToList();

        //            return new ListResponse<UserResponse> { ListResult = list, Success = true };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ListResponse<UserResponse> { ErrorMessage = ex.ToString(), Success = false };
        //    }
        //}


        [MimeMultipart]
        [HttpPost]
        [ActionName("UploadUserImage")]
        public async Task<ImageResponse> AddUserImage()
        {
            try
            {
                var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);


                var kv = this.Request.Headers.FirstOrDefault(k => k.Key == "ownerEmail");
                var ownerEmail = kv.Value.First();
                // Read the MIME multipart asynchronously 
                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                var _localFileName = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                
                using (DataBaseContext db = new DataBaseContext())
                {                    
                    var user = db.Users.Single(u => u.Email == ownerEmail);
                    user.Image = this.ToUrl(_localFileName);                    
                    db.SaveChanges();
                }
                // Create response
                return new ImageResponse() { Image = this.ToUrl(_localFileName), Success = true };
            }
            catch (Exception ex)
            {
                return new ImageResponse { ErrorMessage = ex.ToString() };
            }
        }


        [HttpPost]
        [ActionName("ChangePassword")]
        public ModelWithStatus ChangePassword(ChangePasswordRequest requset)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var userDb = db.Users.FirstOrDefault(u => u.Id == requset.UserID);
                    if (!userDb.Password.Equals(requset.CurrentPassword))
                        throw new Exception("The password is incorrect.");
                    else
                    {
                        userDb.Password = requset.NewPassword;
                        db.SaveChanges();
                    }

                    return ReturnSuccess();
                }
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }

        [HttpPost]
        [ActionName("RecoverPassword")]
        public ModelWithStatus RecoverPassword([FromBody]string email)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var userDb = db.Users.FirstOrDefault(u => u.Email == email);
                    if (userDb == null)
                        throw new Exception("The email that you tried to use does not exist.");
                    //var newPassword = System.Web.Security.Membership.GeneratePassword(8, 4);
                    //userDb.Password = newPassword;
                    //db.SaveChanges();
                    InvokeAfterSec(1500, () =>
                     {
                         MailerHelper mailHelper = new MailerHelper();
                         var body = string.Format("Hi {0},\nWe received a request to recover your password. Your password is {1} \n If you didn't request password recovery then you can just ignore this email. \n\nRegards\nAppGuys Team", userDb.FirstName, userDb.Password);
                         mailHelper.SendEmail(email, body, "ASA Password recovery result", "noreply@appguys.com");
                     });
                    return ReturnSuccess();
                }
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }


        [HttpPost]
        [ActionName("UpdateDeviceToken")]
        public ModelWithStatus UpdateDeviceToken(UpdateUserTokenRequest updateRequest)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == updateRequest.UserID);
                    if (user != null)
                    {
                        user.DeviceToken = updateRequest.DeviceToken;
                        db.SaveChanges();
                    }

                    return ReturnSuccess();
                }
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }
    }
}