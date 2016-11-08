using DotyAppServer.DataAccess;
using DotyAppServer.Models;
using DotyAppServer.Models.Request;
using DotyAppServer.Models.Response;
using DotyAppServer.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DotyAppServer.Controllers
{
    public class EventController : BaseApiController
    {
        [HttpGet]
        [ActionName("GetEvents")]
        public ListResponse<EventResponse> GetEvents(string fordate)
        {
            try
            {
                var date = DateTime.Parse(fordate, new CultureInfo("en-US"));
                using (DataBaseContext db = new DataBaseContext())
                {
                    var events = db.Events.Where(m => m.EndTime.Year == date.Year && m.EndTime.Month == date.Month && m.EndTime.Day == date.Day);
                    var list = events.ToList().Select(ev => new EventResponse
                    {
                        ID = ev.ID,
                        Description = ev.Description,
                        Title = ev.Title,
                        StartTimeStringUtc = ev.StartTime.ToString(new CultureInfo("en-US")),
                        EndTimeStringUtc = ev.EndTime.ToString(new CultureInfo("en-US")),
                        EventColor = ev.EventColor
                    }).ToList();

                    var listresponse = new ListResponse<EventResponse>();
                    listresponse.ListResult = list;
                    listresponse.Success = true;
                    return listresponse;
                }
            }
            catch (Exception ex)
            {
                return this.ReturnFailed<ListResponse<EventResponse>>(ex);
            }
        }


        [HttpGet]
        [ActionName("GetEvents")]
        public ListResponse<EventResponse> GetEventsNew(string startUTCString, string endUTCString)
        {
            try
            {
                var startDate = DateTime.Parse(startUTCString, new CultureInfo("en-US"));
                var endDate = DateTime.Parse(endUTCString, new CultureInfo("en-US"));
                using (DataBaseContext db = new DataBaseContext())
                {   
                    var events = db.Events.Where(m => m.StartTime >= startDate && m.EndTime <= endDate).ToList();
                    //(m.StartTime.Year <= startDate.Year && m.EndTime.Year <= endDate.Year) && 
                    //(m.StartTime.Month <= startDate.Month && m.EndTime.Month <= endDate.Month) &&
                    //(m.StartTime.Month <= startDate.Day && m.EndTime.Month <= endDate.Day)); //m.EndTime.Year == date.Year && m.EndTime.Month == date.Month && m.EndTime.Day == date.Day);
                    var list = events.ToList().Select(ev => new EventResponse
                    {
                        ID = ev.ID,
                        Description = ev.Description,
                        Title = ev.Title,
                        StartTimeStringUtc = ev.StartTime.ToString(new CultureInfo("en-US")),
                        EndTimeStringUtc = ev.EndTime.ToString(new CultureInfo("en-US")),
                        EventColor = ev.EventColor
                    }).ToList();

                    var listresponse = new ListResponse<EventResponse>();
                    listresponse.ListResult = list;
                    listresponse.Success = true;
                    return listresponse;
                }
            }
            catch (Exception ex)
            {
                return this.ReturnFailed<ListResponse<EventResponse>>(ex);
            }
        }



        [HttpPost]
        [ActionName("CreateEvent")]
        public IDResponse CreateEvent(NewEventRequest request)
        {
            try
            {
                var eve = new Event();
                using (DataBaseContext db = new DataBaseContext())
                {                    
                    eve.Title = request.Title;
                    eve.Description = request.Description;
                    eve.StartTime = DateTime.Parse(request.StartTimeStringUtc, new CultureInfo("en-US"));
                    eve.EndTime = DateTime.Parse(request.EndTimeStringUtc, new CultureInfo("en-US"));
                    eve.EventColor = request.EventColor;
                    db.Events.Add(eve);
                    db.SaveChanges();
                }

                //send Push Notification
                InvokeAfterSec(1500, () =>
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {
                        var users = db.Users.Where(u => !string.IsNullOrEmpty(u.Platform) && !string.IsNullOrEmpty(u.DeviceToken)).ToList();                        
                        users = users.Where(u => !u.IsAdmin).ToList();
                        var pushMessage = new PushMessage() { Alert = "Hi, a new appointment created for " + eve.StartTime.ToString("dd MMM") };
                        PushNotification(users, pushMessage);
                    }
                });

                return new IDResponse { ID = eve.ID, Success = true };
            }
            catch (Exception ex)
            {
                return new IDResponse { ErrorMessage=ex.ToString()};
            }
        }

        

        [HttpPost]
        [ActionName("UpdateEvent")]
        public ModelWithStatus UpdateEvent(UpdateEventRequest request)
        {
            try
            {
                string previousTitle = null;
                using (DataBaseContext db = new DataBaseContext())
                {
                    var eve = db.Events.First(e => e.ID == request.Id);
                    previousTitle = eve.Title;
                    eve.Title = request.Title;
                    eve.Description = request.Description;
                    eve.StartTime = DateTime.Parse(request.StartTimeStringUtc, new CultureInfo("en-US"));
                    eve.EndTime = DateTime.Parse(request.EndTimeStringUtc, new CultureInfo("en-US"));
                    eve.EventColor = request.EventColor;
                    db.SaveChanges();
                }

                //send Push Notification
                InvokeAfterSec(1500, () =>
                {
                    using (DataBaseContext db = new DataBaseContext())
                    {   
                        var users = db.Users.Where(u => !string.IsNullOrEmpty(u.Platform) && !string.IsNullOrEmpty(u.DeviceToken)).ToList();
                        users = users.Where(u => !u.IsAdmin).ToList();
                        string alert = string.Empty;
                        if (string.IsNullOrEmpty(previousTitle))
                            alert = "Hi, The one of the appointment has been updated.";
                        else alert = "Hi, The " + previousTitle + " appointment has been updated.";
                        var pushMessage = new PushMessage() { Alert =  alert};
                        PushNotification(users, pushMessage);
                    }
                });

                return ReturnSuccess();
            }
            catch (Exception ex)
            {
                return ReturnFailed(ex);
            }
        }


        [HttpPost]
        [ActionName("DeleteEvent")]
        public ModelWithStatus DeleteEvent(int eventID)
        {
            try
            {
                using (DataBaseContext db = new DataBaseContext())
                {
                    var eve = db.Events.First(e => e.ID == eventID);
                    db.Events.Remove(eve);
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