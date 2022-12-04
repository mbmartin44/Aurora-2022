///--------------------------------------------------------------------------------------
/// <file>    ContactsPackage.cs                                   </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     This class contains the various functions and routines
///     that are needed for the email client to work.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using UnityEngine;

/// <summary>
/// This class contains various functions and routines
/// that are needed for the email client to work.
/// </summary>
class MailPackage
{
    /// <summary>
    /// This code is used to send an email with an attachment to a provided email address
    /// </summary>
    /// <param name="too">ContactsPackage that contains the email address and name of the recipient</param>
    /// <param name="attach">Attachment that is to be sent</param>
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


    /// <summary>
    /// This function sends an email to the contact that has been selected by the user
    /// </summary>
    /// <param name="too">This is the contact object that the user has selected</param>
    /// <remarks>
    /// This method does not currently support attachments.
    /// </remarks>
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