/*
 * Author: Keaton Shelton
 * Date: August 25th, 2022
 * Arguments: ContactsPackage and Attachment
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
 *  05ks - October 17th, 2022 - Name Fix and Remove Test Features
 *  06ks - November 16th, 2022 - Changes to support new ContactsPackage, check for blank email
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using UnityEngine;

class MailPackage
{
    //Email with attachment
    public static void SendMail(ContactsPackage too, Attachment attach)
    {
        var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Seizure App Auto Sender");
        const string fromPassword = "xgfxsygrzzcenjlv";
        const string host = "smtp.gmail.com";
        const int port = 587;
        string subject = "Event Detected";
        string body = "This is an automated message from Seizure Detection App that an event has been recorded";
        try
        {
            //06ks
            if(too.address == "")
            {
                return;
            }
            using (var smtp = new SmtpClient(host))
            {
                smtp.Host = host;
                smtp.Port = port;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
                //06ks
                using (var message = new MailMessage(fromAddress, new MailAddress(too.address, too.name)))
                {
                    message.Subject = subject;
                    message.Body = body;
                    message.Attachments.Add(attach);
                    smtp.Send(message);
                };
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e.ToString());
        }
    }


    //Email with no attachment
    public static void SendMail(ContactsPackage too)
    {
        var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Seizure App Auto Sender");
        const string fromPassword = "xgfxsygrzzcenjlv";
        const string host = "smtp.gmail.com";
        const int port = 587;
        string subject = "Event Detected";
        string body = "This is an automated message from Seizure Detection App that an event has been recorded";
        try
        {
            //06ks
            if (too.address == "")
            {
                return;
            }
            using (var smtp = new SmtpClient(host))
            {
                smtp.Host = host;
                smtp.Port = port;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
                //06ks
                using (var message = new MailMessage(fromAddress, new MailAddress(too.address, too.name)))
                {
                    message.Subject = subject;
                    message.Body = body;
                    smtp.Send(message);
                };
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e.ToString());
        }
    }
}