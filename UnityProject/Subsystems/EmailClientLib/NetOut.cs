/*
 * Author(s): Keaton Shelton
 * Date: September 19th, 2022
 * Arguments:
 *  Inputs: A list of contacts and an attachment
 *  Outputs: A sent array of enails and texts
 * Returns: A sent array of emails and/or text messages
 *
 * Abstract:
 *      This class will act as an interface for the various related email
 *    and text functions used in the final deployment of the Unity App.
 *    The primary function will execute the bulk work of the email client and text
 *    client. A small started function will be used to watch for a seizure positive signal
 *    and execute the primary function on a true condition.
 *    
 * Revisions:
 * 01ks - October 17th, 2022 - Name Fix, remove namespace, remove supporting classes
 * 02jl - October 18th, 2022 - SignalWatchSingle Creation Start
 * 03jl - October 19th, 2022 - SignalWatchSingle for non-attachment
 * 04ks - October 25th, 2022 - SignalWatch with harcoded contacts list added, minor polish
*/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;



public class NetOut : MonoBehaviour
{
    //Main Use Function with attachment
    public static async void SignalWatch(List<ContactsPackage> people, bool detect, Attachment attach)
    {
        try
        {
            if (detect)
            {
                //Send emails and text messages from contacts list
                foreach (var x in people)
                {
                    TextDriver texter = new TextDriver();
                    texter.Send(x);
                    MailPackage.SendMail(x, attach);
                    await System.Threading.Tasks.Task.Delay(2000);
                }
            }
            else
            {
                return;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    //04ks
    //SignalWatch no attachment with hard coded list of contacts, this must be updated manually before compile to work
    public static async void SignalWatch(bool detect, Attachment attach)
    {
        try
        {
            List<ContactsPackage> people = new List<ContactsPackage>();
            //Required Setup Variables (Contacts)
            var contact1 = new ContactsPackage(new MailAddress("example@hotmail.net", "Auto Sender"), "123456789");
            people.Add(contact1);



            if (detect)
            {
                foreach (var x in people)
                {
                    TextDriver texter = new TextDriver();
                    texter.Send(x);
                    MailPackage.SendMail(x, attach);
                    await System.Threading.Tasks.Task.Delay(2000);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    //02jl
    public static void SignalWatchSingle(List<ContactsPackage> people, bool detect, Attachment attach, int select)
    {
        try
        {
            if (detect)
            {
                var text2 = new TextDriver();
                text2.Send(people[select]);
                MailPackage.SendMail(people[select], attach);
            }
            else
            {
                return;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error " + e.ToString());
        }
    }

    //Main Use function no attachment
    public static async void SignalWatch(List<ContactsPackage> people, bool detect)
    {
        try
        {
            if (detect)
            {
                //Send emails and text messages from contacts list
                foreach (var x in people)
                {
                    TextDriver texter = new TextDriver();
                    texter.Send(x);
                    MailPackage.SendMail(x);
                    await System.Threading.Tasks.Task.Delay(2000);
                }
            }
            else
            {
                return;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    //04ks
    //SignalWatch no attachment with hard coded list of contacts, this must be updated manually before compile to work
    public static async void SignalWatch(bool detect)
    {
        try
        {
            List<ContactsPackage> people = new List<ContactsPackage>();
            //Required Setup Variables (Contacts)
            var contact1 = new ContactsPackage(new MailAddress("example@hotmail.net", "Auto Sender"), "123456789");
            people.Add(contact1);



            if (detect)
            {
                foreach (var x in people)
                {
                    TextDriver texter = new TextDriver();
                    texter.Send(x);
                    MailPackage.SendMail(x);
                    await System.Threading.Tasks.Task.Delay(2000);
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    //03jl
    public static void SignalWatchSingle(List<ContactsPackage> people, bool detect, int select)
    {
        try
        {
            if (detect)
            {
                var text2 = new TextDriver();
                text2.Send(people[select]);
                MailPackage.SendMail(people[select]);
            }
            else
            {
                return;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error " + e.ToString());
        }
    }
}