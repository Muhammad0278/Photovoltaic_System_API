using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace Photovoltic_API.Controllers
{
    public class SendEmail
    {
        public static void Send_Email( string To, string Subject, string Body,string attachment)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("");
                    mail.To.Add(To);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    mail.Attachments.Add(new Attachment(attachment));//--Uncomment this to send any attachment  
                    using (SmtpClient smtp = new SmtpClient("", 578))
                    {
                        smtp.Credentials = new NetworkCredential("smtp.gmail.com", "");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
    