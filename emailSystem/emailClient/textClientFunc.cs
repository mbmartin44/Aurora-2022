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
 *  03ks - August 31st, 2022 - Begin Gateway Array Setup
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
        //Global Variables
        //SMS / MMS String Array
        /*
         *1. T-Mobile: @tmomail.net sms/mms
         *2. AT&T: @txt.att.net (sms) @mms.att.net (mms)
         *3. Verizon: @vtext.com (sms) @vzwpix.com (mms)
         *4. Virgin: @vmobl.com (sms) @vmpix.com (mms)
         *5. Boost: @sms.myboostmobile.com (sms) @myboostmobile.com (mms)
         *6. Xfinity: @vtext.com (sms) @mypixmessages.com (mms)
         *7. Sprint: @messaging.sprintpcs.com (sms) @pm.sprint.com
         *8. U.S. Cell: @email.uscc.net (sms) @mms.uscc.net (mms)
         *9. Cricket: @sms.cricketwireless.net (sms) @mms.cricketwireless.net
         *10. Tracfone: @mmst5.tracfone.com sms/mms
         *11. Metro: @mymetropcs.com sms/mms
         *12. Ting: @message.ting.com sms/mms
         */
        private static readonly string[] sms = {"@tmomail.net", "@txt.att.net", "vtext.com", "@vmobl.com", "@sms.myboostmobile.com", "@vtext.com", "@messaging.sprintpcs.com",
            "@email.uscc.net", "@sms.circketwireless.net", "@mmst5.tracfone.com", "@mymetropcs.com", "@message.ting.com"};
        private static readonly string[] mms = { "@tmomail.net", "@mms.att.net", "@vzwpix.com", "@vmpix.com", "@myboostmobile.com", "@mypixmessages.com", "@pm.sprint.com",
            "@mms.uscc.net", "@mms.cricketwireless.net", "@mmst5.tracfone.com", "@mymetropcs.com", "@message.ting.com"};


        //Normal Text Message Test Function
        public static void sendText(MailAddress from, string number, int carrier, string password, string subject, string body, string host, int port)
        {
            MailAddress too = new MailAddress(number + sms[carrier], from.DisplayName);
            //Test Send SMS
            MailPackage.sendMail(from, too, password, subject, body, host, port);
        }

        //MMS Test Function
        public static void sendMMS(MailAddress from, string number, int carrier, string password, string subject, string body, string host, int port, Attachment attachment)
        {
            MailAddress too = new MailAddress(number + mms[carrier], "Keaton Shelton");
            //Test Send MMS
            MailPackage.sendMail(from, too, password, subject, body, host, port, attachment);
        }
    }
}
