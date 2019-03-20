using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace UsedGoodApp.Infrastructure
{
    public class GMailer
    {
        private static string GmailUsername { get; set; }
        private static string GmailPassword { get; set; }
        private static string GmailHost { get; set; }
        private static int GmailPort { get; set; }
        private static bool GmailSSL { get; set; }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }

        static GMailer()
        {
            GmailHost = "smtp.gmail.com";
            GmailPort = 587;
            GmailSSL = true;
            GmailUsername = "";
            GmailPassword = "";
        }

        public void Send()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = GmailHost;
            smtp.Port = GmailPort;
            smtp.EnableSsl = GmailSSL;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new NetworkCredential(GmailUsername, GmailPassword);

            using (var message = new MailMessage(GmailUsername, ToEmail))
            {
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = IsHtml;
                smtp.Send(message);
            }
        }
    }
}