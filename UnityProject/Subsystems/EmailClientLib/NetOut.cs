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
*/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;



class NetOut : MonoBehaviour
{
    //Main Use Function
    public static async void SignalWatch(List<ContactsPackage> people, bool detect, Attachment attach)
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

    public static async void SignalWatch(List<ContactsPackage> people, bool detect)
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
}