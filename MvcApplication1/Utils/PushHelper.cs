using Newtonsoft.Json;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotyAppServer.Utility
{
    public class PushHelper
    {   
        private string ANDROID_API_KEY = ConfigurationManager.AppSettings["ANDROID_API_KEY"];

        PushBroker push;
        public PushHelper()
        {
            //Create our push services broker
            push = new PushBroker();

            //Wire up the events for all the services that the broker registers
            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;
        }

        public void PushApple(IEnumerable<string> deviceTokens, PushMessage pushmessage)
        {
            //if (ConfigurationManager.AppSettings["EnableApplePushNotification"].ToLower() == "no")
            //    return;
            if (deviceTokens == null)
                return;
            if (deviceTokens != null && !deviceTokens.Any())
                return;
            //try
            //{
            //-------------------------
            // APPLE NOTIFICATIONS
            //-------------------------
            //Configure and start Apple APNS
            // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to generate one for connecting to Sandbox,
            //   and one for connecting to Production.  You must use the right one, to match the provisioning profile you build your
            //   app with!
            //So we will use dev certificate if server run under debug mode otherwise use Production certificate
            var APPLE_CERTIFICATE_PASSWORD = "";
            byte[] appleCert = new byte[0];
//#if DEBUG
//            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Certificates\\apsDev.p12"))
//                return;
//            APPLE_CERTIFICATE_PASSWORD = "123";
//            appleCert = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Certificates\\apsDev.p12");
//#else
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Certificates\\apsPro.p12"))
                return;
            appleCert = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\Certificates\\apsPro.p12");
            APPLE_CERTIFICATE_PASSWORD = "doty2016";
//#endif
            //IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server
            //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
            //  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
            //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
//#if DEBUG
//            push.RegisterAppleService(new ApplePushChannelSettings(appleCert, APPLE_CERTIFICATE_PASSWORD, true));
//#else
            push.RegisterAppleService(new ApplePushChannelSettings(true, appleCert, APPLE_CERTIFICATE_PASSWORD, true));
//#endif
            //Fluent construction of an iOS notification
            //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
            //  for registered for remote notifications is called, and the device token is passed back to you
            //var deviceToken = "fa8a40bdaae917919a0965c3f2292f790891cf6d7d3d0973bfdd1b5625c097c1";
            foreach (var token in deviceTokens)
            {
                var notification = new AppleNotification()
                                           .ForDeviceToken(token)
                                           .WithAlert(pushmessage.Alert)
                                           .WithBadge(pushmessage.Badge)
                                           .WithSound(pushmessage.Sound);
                push.QueueNotification(notification);
            }
            push.StopAllServices();
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("Push error: " + ex.ToString());
            //}
        }


        public void PushAndroid(List<string> registrationIds, PushMessage msg)
        {
            //if (ConfigurationManager.AppSettings["EnableGooglePushNotification"].ToLower() == "no")
            //    return;

            if (!registrationIds.Any())
                return;

            //try
            //{
                var jsonMsg = JsonConvert.SerializeObject(msg);
                //---------------------------
                // ANDROID GCM NOTIFICATIONS
                //---------------------------
                //Configure and start Android GCM
                //IMPORTANT: The API KEY comes from your Google APIs Console App, under the API Access section, 
                //  by choosing 'Create new Server key...'
                //  You must ensure the 'Google Cloud Messaging for Android' service is enabled in your APIs Console
                push.RegisterGcmService(new GcmPushChannelSettings(ANDROID_API_KEY));
                //Fluent construction of an Android GCM Notification
                //IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!
                push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(registrationIds)
                                      //.WithJson(string.Format("{\"alert\":\"{0} \",\"badge\":{1},\"sound\":\"sound.caf\"}", message, badge)));
                                      .WithJson(jsonMsg));
                push.StopAllServices();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Push error: " + ex.ToString());
            //}
        }
        



#region event handlers
        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Service Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }
#endregion
    }

    public class PushMessage
    {   
        public string Alert { get; set; }
        public int Badge { get { return 1; } }
        public string Sound { get { return "sound.caf"; } }        
    }    
}
