using System;
using System.Web.Http;
using DotyAppServer.Models.Response;


namespace DotyAppServer.Controllers
{
    public class BaseApiController : ApiController
    {
        [NonAction]
        protected virtual ModelWithStatus ReturnFailed(Exception ex)
        {
            return new ModelWithStatus()
            {
                ErrorMessage = ex.ToString(),
                Success = false 
            };
        }

        [NonAction]
        protected virtual T ReturnFailed<T>(Exception ex) where T : ModelWithStatus
        {
            T result = Activator.CreateInstance<T>();
            result.ErrorMessage = ex.ToString();
            result.Success = false;
            return result;
        }

        [NonAction]
        protected virtual ModelWithStatus ReturnFailed(string error)
        {
            return new ModelWithStatus()
            {
                ErrorMessage = error,
                Success = false
            };
        }

        [NonAction]
        protected virtual T ReturnFailed<T>(string error) where T : ModelWithStatus
        {
            T result = Activator.CreateInstance<T>();
            result.ErrorMessage = error;
            result.Success = false;
            return result;
        }

        [NonAction]
        protected virtual ModelWithStatus ReturnSuccess()
        {
            return new ModelWithStatus()
            {
                Success = true
            };
        }

       

        
    }

   
}