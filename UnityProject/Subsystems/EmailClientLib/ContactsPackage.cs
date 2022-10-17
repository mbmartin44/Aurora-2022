/*
 * Author: Keaton Shelton
 * Date: September 1st, 2022
 * Arguments: n/a
 * Returns: n/a
 * 
 * Abstract:
 *   This class contains the contacts class
 *  that are needed for the email / text client to work.
 *  
 *  Revisions:
 *  01ks - 10/17/2022 - Fix naming, add constructors
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;


class ContactsPackage
{
    public MailAddress nameAddress { get; set; }
    public string phone { get; set; }
    
    public ContactsPackage()
    {
        this.nameAddress = new MailAddress("null@hotmail.net", "Default");
        this.phone = "0000000000";
    }

    public ContactsPackage(MailAddress nameAddress, string phone)
    {
        this.nameAddress = nameAddress;
        this.phone = phone;
    }
}

