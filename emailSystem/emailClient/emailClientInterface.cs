/*
 * Author(s): Keaton Shelton
 * Date: September 19th, 2022
 * Arguments:
 *  Inputs: A list of contacts and an attachment
 *  Outputs: A sent array of enails and texts
 * Returns: A sent array of emails and/or text messages
 *
 * Abstract:
 *      This class will act as an interface for the various related email
 *    and text functions used in the final deployment of the Unity App.
 *    The primary function will execute the bulk work of the email client and text
 *    client. A small started function will be used to watch for a seizure positive signal
 *    and execute the primary function on a true condition.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using Email_Client;
using Text_Client;
using Contacts;

namespace Email_Interface
{
    class emailClientInterface
    {
        public async void signalWatch(List<ContactsPackage> people, Attachment attach, bool detect, bool validAtt)
        {
            if(detect) {
                var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Seizure App Auto Sender");
                const string fromPassword = "xgfxsygrzzcenjlv";
                const string host = "smtp.gmail.com";
                const int port = 587;
                string subject = "Event Detected";
                string body = "This is an automated message from Seizure Detection App that an event has been recorded";


                //Send emails and text messages from contacts list
                foreach(var x in people)
                {
                    if(validAtt) 
                    {
                        Email_Client.MailPackage.sendMailAttach(fromAddress, x, fromPassword, subject, body, host, port, attach);
                        Text_Client.TextPackage.sendMMS(fromAddress, x, x.carrierID, fromPassword, subject, body, host, port, attach);
                        await System.Threading.Tasks.Task.Delay(2000);
                    }
                    else
                    {
                        Email_Client.MailPackage.sendMail(fromAddress, x, fromPassword, subject, body, host, port);
                        Text_Client.TextPackage.sendText(fromAddress, x, x.carrierID, fromPassword, subject, body, host, port);
                        await System.Threading.Tasks.Task.Delay(2000);

                    }
                }
            }
            else
            {
                return;
            }
        }
    }
}
