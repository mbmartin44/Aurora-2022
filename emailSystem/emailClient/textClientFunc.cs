/*
 * Author: Keaton Shelton
 * Date: August 25th, 2022
 * Arguments: n/a
 * Returns: n/a
 * 
 * Abstract:
 *      This class contains the various functions and routines
 *  that are needed for the texting client to work.
 *  
 *  Revisions:
 *  01ks - August 25th, 2022 - Original
 *  02ks - August 29th, 2022 - Add in MMS support
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using Email_Client;

namespace Text_Client
{
    class TextPackage
    {
        //Normal Text Message Test Function
        public static void sendText(MailAddress from, string number, string password, string subject, string body, string host, int port)
        {
            string cricket = "@mms.cricketwireless.net";
            MailAddress too = new MailAddress(number + cricket, "Keaton Shelton");
            //Test Send to me on Cricket
            MailPackage.sendMail(from, too, password, subject, body, host, port);
        }

        //MMS Test Function
        public static void sendMMS(MailAddress from, string number, string password, string subject, string body, string host, int port, Attachment attachment)
        {
            string cricket = "@mms.cricketwireless.net";
            MailAddress too = new MailAddress(number + cricket, "Keaton Shelton");

            //Test Send MMS on Cricket
            MailPackage.sendMail(from, too, password, subject, body, host, port, attachment);
        }
    }
}
