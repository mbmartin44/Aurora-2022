using System;
using System.Net;
using System.Net.Mail;
using Email_Client;
using Text_Client;

namespace Network_Dev
{
    class NetworkTest
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

            MailPackage.sendMail(fromAddress, tooAddress, fromPassword, subject, body, host, port, attach);
        }
    }
}
