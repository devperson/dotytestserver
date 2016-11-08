using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DotyAppServer.Models.Response;
using DotyAppServer.Models;
using DotyAppServer.Utility;

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

        [NonAction]
        protected string ToUrl(string path)
        {
            var relativeUrl = this.ToRelative(path);
            var url = HttpContext.Current.Request.Url;
            var port = (url.AbsoluteUri.Contains("localhost") || url.AbsoluteUri.Contains("pc") || url.AbsoluteUri.Contains("desktop") || url.AbsoluteUri.Contains("192.168")) ? (":" + url.Port) : String.Empty;
            return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }

        [NonAction]
        protected string ToRelative(string path)
        {
            var relativeUrl = string.IsNullOrEmpty(path) ? string.Empty : path.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "/").Replace(@"\", "/");
            return relativeUrl;
        }


        static System.Timers.Timer t;
        [NonAction]
        protected void InvokeAfterSec(double sec, Action action)
        {
            t = new System.Timers.Timer();
            t.Interval = sec;
            t.Elapsed += (s, e) =>
            {
                var timer = s as System.Timers.Timer;
                timer.Stop();
                action();
            };
            t.Start();
        }

        [NonAction]
        public void PushNotification(List<User> users, PushMessage pushMessage)
        {
            PushHelper pushHelper = new PushHelper();
            //push to ios devices
            var iosUsers = users.Where(u => u.Platform.ToLower().Contains("ios")).ToList();
            var iosDevices = iosUsers.Select(u => u.DeviceToken).Distinct().ToList();
            pushHelper.PushApple(iosDevices, pushMessage);
            //push to android devices
            var androidUsers = users.Where(u => !u.Platform.ToLower().Contains("ios")).ToList();
            var androidDevices = androidUsers.Select(u => u.DeviceToken).Distinct().ToList();
            pushHelper = new PushHelper();
            pushHelper.PushAndroid(androidDevices, pushMessage);
        }
    }

   
}