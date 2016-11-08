using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using DotyAppServer.Controllers;
using DotyAppServer.DataAccess;
using DotyAppServer.Models;
using DotyAppServer.Models.Request;
using DotyAppServer.Models.Response;
using DotyAppServer.Utility;

namespace DotyAppServer.Controllers
{
    public class MessagesController : BaseApiController
    {        
        [HttpGet]
        [ActionName("HasNewMessages")]
        public ListResponse<MessageResponse> HasNewMessages(string lastIdentifier)
        {
            try
            {                
                using (DataBaseContext db = new DataBaseContext())
                {
                    var msg = db.Messages.FirstOrDefault(f => f.Identifier == lastIdentifier);
                    int lastId = 0;
                    if (msg != null)
                        lastId = msg.Id;

                    var msges = db.Messages.Include("FromUser").Where(m => m.Id > lastId).ToList();
                    var list = msges.Select(m => new MessageResponse
                    {
                        Identifier = m.Identifier,
                        CreatedDateStringUtc = m.Date.ToString(new CultureInfo("en-US")),
                        Message = m.Text,
                        Sender = new UserResponse
                        {
                            UserID = m.FromUser.Id,
                            Email = m.FromUser.Email,
                            FirstName = m.FromUser.FirstName,
                            LastName = m.FromUser.LastName,
                            Image = m.FromUser.Image
                        }
                    }).ToList();

                    var listresponse = new ListResponse<MessageResponse>();
                    listresponse.ListResult = list;
                    listresponse.Success = true;
                    return listresponse;
                }
            }
            catch (Exception ex)
            {
                return this.ReturnFailed<ListResponse<MessageResponse>>(ex);
            }
        }

        [HttpPost]
        [ActionName("SaveMessage")]
        public ModelWithStatus SaveMessage(MessageRequest request)
        {
            try
            {
                var message = new Message();
                message.Identifier = request.Identifier;
                message.Date = DateTime.UtcNow;
                message.FromUser_Id = request.SenderId;
                message.Text = request.Message;

                using (DataBaseContext db = new DataBaseContext())
                {
                    //save message   
                    db.Messages.Add(message);
                    db.SaveChanges();
                }

                //send Push Notification
                InvokeAfterSec(1500, () =>
                 {
                     using (DataBaseContext db = new DataBaseContext())
                     {
                         var sender = db.Users.FirstOrDefault(s => s.Id == request.SenderId);
                         var users = db.Users.Where(u => !string.IsNullOrEmpty(u.Platform) && !string.IsNullOrEmpty(u.DeviceToken) && u.Id != request.SenderId).ToList();
                         var alert = string.Empty;
                         if (!string.IsNullOrEmpty(sender.FirstName) && !string.IsNullOrEmpty(sender.LastName))
                             alert = "Hi, there is a new message from " + sender.FirstName + " " + sender.LastName;
                         else if (!string.IsNullOrEmpty(sender.FirstName))
                             alert = "Hi, there is a new message from " + sender.FirstName;
                         else alert = "Hi, there is a new message";

                         var pushMessage = new PushMessage() { Alert = alert };
                         PushNotification(users, pushMessage);
                     }
                 });

                return this.ReturnSuccess();
                //return new NewMessageResponse { MessageID = message.Id, CreatedDateStringUtc = message.Date.ToString(new CultureInfo("en-US")), Success = true };
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }


        [HttpPost]
        [ActionName("DeleteMessage")]
        public ModelWithStatus DeleteMessage(string identifier)
        {
            try
            {
                using (DataBaseContext db = new DataBaseContext())
                {
                    var eve = db.Messages.First(e => e.Identifier == identifier);
                    db.Messages.Remove(eve);
                    db.SaveChanges();
                }
                return ReturnSuccess();
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }
    }
}