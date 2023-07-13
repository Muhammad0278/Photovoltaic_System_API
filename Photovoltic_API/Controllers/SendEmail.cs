using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using Google.Apis.Gmail.v1.Data;

using System.IO;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Mime;

namespace Photovoltic_API.Controllers
{
    public class SendEmail
    {

        public async static void Send_Email(string To, string Subject, string Body, string filepath)
        {
            try
            {


                string smtpServer = "smtp.mailtrap.io";
                int smtpPort = 587;

                string attachmentPath = filepath; // Update with the path to your file

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("7a6953d774b837", "087d6898347875");

                    using (var message = new MailMessage("marslankhan1985@gmail.com", To, Subject, Body))
                    {
                        try
                        {
                            // Create the attachment
                            Attachment attachment = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = attachment.ContentDisposition;
                            disposition.CreationDate = File.GetCreationTime(attachmentPath);
                            disposition.ModificationDate = File.GetLastWriteTime(attachmentPath);
                            disposition.ReadDate = File.GetLastAccessTime(attachmentPath);

                            // Add the attachment to the email
                            message.Attachments.Add(attachment);

                            // Send the email
                            client.Send(message);

                            Console.WriteLine("Email sent successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to send email: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
