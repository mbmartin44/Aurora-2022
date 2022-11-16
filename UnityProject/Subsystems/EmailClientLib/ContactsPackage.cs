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
 *  02ks - 11/16/2022 - Remove MailAddress and replace with string equivalents / change defaults. Make Serializable
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

[System.Serializable]
public class ContactsPackage
{
    public string name;
    public string phone;
    public string address;
    
    public ContactsPackage()
    {
        //02ks
        this.phone = "";
        this.name = "Default";
        this.address = "";
    }
    //02ks
    public ContactsPackage(string name, string phone, string address)
    {
        this.phone = phone;
        this.name = name;
        this.address = address;
    }
}

