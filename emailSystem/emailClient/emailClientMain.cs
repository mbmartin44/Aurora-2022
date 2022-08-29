/*
 * Author: Keaton Shelton
 * Date: August 25th, 2022
 * Arguments: none
 * Returns: n/a
 * 
 * Abstract:
 *      This is the main testing program for the various
 *  different subfunctions / routines that will be deployed
 *  in the final detection app.
 *  
 *  Revisions:
 *  01ks - ? - Original
 *  02ks - August 25th, 2022 - Update Structure
 *  03ks - August 25th, 2022 - Add in texting test function
 *  04ks - August 29th, 2022 - Add in MMS test function
 */
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
            try
            {
                Console.WriteLine("Initializing Mail Client");

                //Test Run

                var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Auto Sender");
                var tooAddress = new MailAddress("keaton.shelton2@gmail.com", "Keaton Shelton");
                string number = "8652366111";
                const string fromPassword = "xgfxsygrzzcenjlv";
                const string subject = "Test Subject";
                const string body = "Why Hello There General Kenobi";
                //Attachment Example below (image type)
                //System.Net.Mail.Attachment attach1 = new System.Net.Mail.Attachment("C:\\Users\\kmshelton\\Downloads\\image004.png");
                Attachment attach = new Attachment(".\\eeg.png");
                const string host = "smtp.gmail.com";
                const int port = 587;

                //Send email with picture
                //MailPackage.sendMail(fromAddress, tooAddress, fromPassword, subject, body, host, port, attach);
                //Works

                //Send normal text
                //TextPackage.sendText(fromAddress, number, fromPassword, subject, body, host, port);
                //Works

                //Send MMS
                TextPackage.sendMMS(fromAddress, number, fromPassword, subject, body, host, port, attach);
                //Works
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }
    }
}
