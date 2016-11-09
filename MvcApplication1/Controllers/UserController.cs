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
                        return new LoginUserResponse { UserID = user.Id, FirstName = user.FirstName, LastName = user.LastName, Image = user.Image, Success = true };
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

       
    }
}