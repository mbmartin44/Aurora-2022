///--------------------------------------------------------------------------------------
/// <file>    MainExample.cs                                       </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     This is a small program intended to demonstrate the use of the
///     various networking classes, and not meant to be used in practice.
/// </summary>
/// -------------------------------------------------------------------------------------

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




        // This flag is used to determine if the event has been detected
        // If the event has been detected, the code will send an email / SMS
        // to the contacts in the list.
        detect = true;

        //Using NetOut Intermediate Class
        //Call SignalWatchBasic (for attachment support don't use basic)
        NetOut.SignalWatch(people, detect);

        //With Attachments
        NetOut.SignalWatch(people, detect, attach);
    }


}