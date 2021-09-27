using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailNotificationService
{
    public class ClassModel
    {
        public string MailRecipient { get; set; }
        public string MailSubject { get; set; }
        public string Body { get; set; }

        public long ID { get; set; }
    }

    public class EmailCalls
    {
        public void SendHtmlFormattedEmail(string subject, string body, string recipient, string username, string emailfrom, string host, string password, bool sslval, int portno)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(emailfrom);

                mailMessage.Subject = subject;

                mailMessage.Body = body;

                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(new MailAddress(recipient));

                SmtpClient smtp = new SmtpClient();

                smtp.Host = host;

                smtp.EnableSsl = sslval;

                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                NetworkCred.UserName = username; //reading from web.config  

                NetworkCred.Password = password; //reading from web.config  

                smtp.UseDefaultCredentials = true;

                smtp.Credentials = NetworkCred;

                smtp.Port = portno; //reading from web.config  

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Send(mailMessage);

            }

        }
    }
}
