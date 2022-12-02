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
        bool detect = false;
        //Use of Contacts class
        //New Defined Contacts
        ContactsPackage person1 = new ContactsPackage("Auto Sender", "123456789", "example@hotmail.net");
        ContactsPackage person2 = new ContactsPackage();
        person2.name = "James Bond";
        person2.phone = "1234567890";
        person2.address = "james@spy.net";

        //Create List of Contacts
        List<ContactsPackage> people = new List<ContactsPackage>();
        people.Add(person1);
        people.Add(person2);

        //Define Attachment
        //If using attachment, attachment ->MUST<- be valid or it will crash
        Attachment attach = new Attachment("Documentation.txt");




        //DSP BLOCK
        //detect flag true / false
        detect = true;
        //DSP BLOCK



        //Using NetOut Intermediate Class
        //Call SignalWatchBasic (for attachment support don't use basic)
        NetOut.SignalWatch(people, detect);

        //With Attachments
        NetOut.SignalWatch(people, detect, attach);
    }


}