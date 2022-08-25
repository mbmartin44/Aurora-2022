using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace Email_Client
{
    class MailPackage
    {
        //Email with attachment
        public static void sendMail(MailAddress from, MailAddress too, string password, string subject, string body, string host, int port, Attachment attach)
        {
            using (var smtp = new SmtpClient(host))
            {
                smtp.Host = host;
                smtp.Port = port;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from.Address, password);

                using (var message = new MailMessage(from, too))
                {
                    message.Subject = subject;
                    message.Body = body;
                    message.Attachments.Add(attach);
                    smtp.Send(message);
                };
            };
        }

        //Email with no attachment
        public static void sendMail(MailAddress from, MailAddress too, string password, string subject, string body, string host, int port)
        {
            using (var smtp = new SmtpClient(host))
            {
                smtp.Host = host;
                smtp.Port = port;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from.Address, password);

                using (var message = new MailMessage(from, too))
                {
                    message.Subject = subject;
                    message.Body = body;
                    smtp.Send(message);
                };
            };
        }
    }
}
