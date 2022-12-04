///--------------------------------------------------------------------------------------
/// <file>    NetOut.cs                                            </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 09/19/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///    This class will act as an interface for the various related email
///    and text functions used in the final deployment of the Unity App.
///    The primary function will execute the bulk work of the email client and text
///    client. A small started function will be used to watch for a seizure positive signal
///    and execute the primary function on a true condition.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;

/// <summary>
/// This class will act as an interface for the various related email
/// and text functions used in the final deployment of the Unity App.
/// The primary function will execute the bulk work of the email client and text
/// client. A small started function will be used to watch for a seizure positive signal
/// and execute the primary function on a true condition.
/// </summary>
public class NetOut : MonoBehaviour
{

    /// <summary>
    /// This function is called when the user presses the "Send" button. It will send the text to the user's contacts in the list.
    /// </summary>
    /// <param name="people">A list of contacts</param>
    /// <param name="detect">A boolean value indicating if the user has pressed the "Send" button</param>
    /// <param name="attach">An attachment to be sent with the text message</param>
    /// <summary>
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    /// <summary>
    /// This is a method that when called, will send a text message and email to a list of people.
    /// </summary>
    /// <param name="detect">A boolean that will determine whether or not a message will be sent.</param>
    /// <param name="attach">The attachment that will be sent with the email.</param>
    /// <returns>Nothing</returns>
    public static async void SignalWatch(bool detect, Attachment attach)
    {
        try
        {
            List<ContactsPackage> people = new List<ContactsPackage>();
            //Required Setup Variables (Contacts)
            //05ks
            var contact1 = new ContactsPackage("Auto Sender", "123456789", "example@hotmail.net");
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

    /// <summary>
    /// This method enables the user to send a text message and email
    /// to an individual in the list of people. It also attaches a file
    /// to the email message.
    /// </summary>
    /// <param name="people"> People is a list of contacts, and is used to
    ///                       determine which contact to send the message to.
    /// </param>
    /// <param name="detect"> Detect is a boolean value that is used to determine
    ///                       whether or not to send the message. </param>
    ///
    /// <param name="attach"> Attach is an attachment that is sent with the message.
    /// </param>
    /// <param name="select"> Select is an integer that is used to determine which
    ///                       contact to send the message to.
    /// </param>
    /// <returns> This method returns nothing. </returns>
    /// <summary>
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

    /// <summary>
    /// This function is used to send text messages and emails to the contacts list passed to it. It waits two seconds between sending each message.
    /// </summary>
    /// <param name="people">A list of contacts to send messages to.</param>
    /// <param name="detect">A boolean variable that determines whether to send messages or not. True will send messages.</param>
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    /// <summary>
    /// This code will send a message to the list of people when a detection occurs.
    /// </summary>
    /// <param name="detect">bool - detection result</param>
    /// <returns>void</returns>
    public static async void SignalWatch(bool detect)
    {
        try
        {
            List<ContactsPackage> people = new List<ContactsPackage>();
            //Required Setup Variables (Contacts)
            //05ks
            var contact1 = new ContactsPackage("Auto Sender", "123456789", "example@hotmail.net");
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
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    /// <summary>
    /// This method is used to send a message to a single contact in the list of people.
    /// </summary>
    /// <param name="people">List of people</param>
    /// <param name="detect">Boolean based on detection result</param>
    /// <param name="select">The index in the contacts list of the desired recipient</param>
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