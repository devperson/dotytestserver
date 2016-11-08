using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace DotyAppServer.Utility
{
    public class MailerHelper
    {
        
        public void SendEmail(string to, string body, string subject = null, string from = null, bool isHtml = false, Attachment atachedfile = null)
        {
            var message = new MailMessage();
            if (from == null)
                from = ConfigurationManager.AppSettings["From"];
            message.From = new MailAddress(from);
            message.To.Add(to);
            if(string.IsNullOrEmpty(subject))
                subject = ConfigurationManager.AppSettings["Subject"];
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;
            if (atachedfile != null)
                message.Attachments.Add(atachedfile);

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            var username = smtpSection.Network.UserName;
            var password = smtpSection.Network.Password;

            var client = new SmtpClient { Credentials = new NetworkCredential(username, password) };


            client.Send(message);
        }
    }
}