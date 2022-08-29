/*
 * Author: Keaton Shelton
 * Date: August 25th, 2022
 * Arguments: n/a
 * Returns: n/a
 * 
 * Abstract:
 *      This class contains the various functions and routines
 *  that are needed for the email client to work.
 *  
 *  Revisions:
 *  01ks - July 16th, 2022 - Convert from Python to C#
 *  02ks - July 16th, 2022 - Remove complexity and shift to single sender
 *  03ks - July 21st, 2022 - Add in attachment support
 *  04ks - August 25th, 2022 - Re-Organization
 */
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
