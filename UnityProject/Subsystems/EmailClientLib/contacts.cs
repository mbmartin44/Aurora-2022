/*
 * Author: Keaton Shelton
 * Date: September 1st, 2022
 * Arguments: n/a
 * Returns: n/a
 *
 * Abstract:
 *   This class contains the contacts class
 *  that are needed for the email / text client to work.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace Contacts
{
    class ContactsPackage
    {
        public MailAddress nameAddress { get; set; }
        public string phone { get; set; }
        public int carrierID { get; set; }
    }
}
