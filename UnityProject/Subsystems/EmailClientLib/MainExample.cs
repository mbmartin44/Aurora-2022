/*
 * Author(s): Keaton Shelton
 * Date: October 17th, 2022
 * Arguments: List of contacts and attachment
 * 
 *
 * Abstract:
 *     Small program to show the use of the various networking classes,
 *     not meant to be run.
 *    
 * Revisions:
 * 01ks - October 17th, 2022 - Original
*/

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;


class MainExample 
{
    void Main() 
    {
        bool seizure = false;
        //Use of Contacts class
        //New Defined Contacts
        ContactsPackage person1 = new ContactsPackage(new MailAddress("john@example.com", "John Smith"), "1234567890");
        ContactsPackage person2 = new ContactsPackage();
        person2.nameAddress = new MailAddress("James Bond", "james@spy.net");
        person2.phone = "1234567890";

        //Create List of Contacts
        List<ContactsPackage> people = new List<ContactsPackage>();
        people.Add(person1);
        people.Add(person2);

        //Define Attachment
        //If using attachment, attachment ->MUST<- be valid or it will crash, same thing with a misdefined MailAddress [something@something.something]
        Attachment attach = new Attachment("Documentation.txt");




        //DSP BLOCK
        //Seizure flag true / false
        seizure = true;
        //DSP BLOCK



        //Using NetOut Intermediate Class
        //Call SignalWatchBasic (for attachment support don't use basic)
        NetOut.SignalWatch(people, seizure);

        //With Attachments
        NetOut.SignalWatch(people, seizure, attach);
    }


}