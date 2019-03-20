using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace UsedGoodApp.App_Start
{
    //public class EmailService : IIdentityMessageService
    //{
    //    public Task SendAsync(IdentityMessage message)
    //    {   
    //        var username = "Iridezzzcent@gmail.com";
    //        var password = "#Ez1oSFamily#02";

    //        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

    //        smtp.EnableSsl = true;
    //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
    //        smtp.UseDefaultCredentials = true;
    //        smtp.Credentials = new NetworkCredential(username, password);

    //        var mail = new MailMessage(username, message.Destination);
    //        mail.Subject = message.Subject;
    //        mail.Body = message.Body;
    //        mail.IsBodyHtml = true;

    //        return smtp.SendMailAsync(mail);
    //    }
    //}
}