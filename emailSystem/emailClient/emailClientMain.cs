using System;
using System.Net;
using System.Net.Mail;

namespace Email_Client
{
    class MailPackage
    {
        static void Main()
        {
            Console.WriteLine("Initializing Mail Client");

            //Test Run

            var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Auto Sender");
            var tooAddress = new MailAddress("keaton.shelton2@gmail.com", "Keaton Shelton");
            const string fromPassword = "xgfxsygrzzcenjlv";
            const string subject = "Test Subject";
            const string body = "Why Hello There General Kenobi";
            //Attachment Example below (image type)
            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment("C:\\Users\\kmshelton\\Downloads\\image004.png");
            const string host = "smtp.gmail.com";
            const int port = 587;

            sendMail(fromAddress, tooAddress, fromPassword, subject, body, host, port, attach);
        }

        //Email with attachment
        static void sendMail(MailAddress from, MailAddress too, string password, string subject, string body, string host, int port, Attachment attach)
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
        static void sendMail(MailAddress from, MailAddress too, string password, string subject, string body, string host, int port)
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
