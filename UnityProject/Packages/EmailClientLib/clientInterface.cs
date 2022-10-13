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
*/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;

namespace PrimaryInterface
{
    public class ClientInterface : MonoBehaviour
    {
        //Main Use Function
        public static async void SignalWatch(List<ContactsPackage> people, Attachment attach, bool detect, bool validAtt)
        {
            if (detect)
            {
                var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Seizure App Auto Sender");
                const string fromPassword = "xgfxsygrzzcenjlv";
                const string host = "smtp.gmail.com";
                const int port = 587;
                string subject = "Event Detected";
                string body = "This is an automated message from Seizure Detection App that an event has been recorded";

                //Send emails and text messages from contacts list
                foreach (var x in people)
                {
                    if (validAtt)
                    {
                        TextDriverFunc texter = new TextDriverFunc();
                        texter.Send(x);
                        MailPackage.sendMailAttach(fromAddress, x, fromPassword, subject, body, host, port, attach);
                        await System.Threading.Tasks.Task.Delay(2000);
                    }
                    else
                    {
                        TextDriverFunc texter = new TextDriverFunc();
                        texter.Send(x);
                        MailPackage.sendMail(fromAddress, x, fromPassword, subject, body, host, port);
                        await System.Threading.Tasks.Task.Delay(2000);

                    }
                }
            }
            else
            {
                return;
            }
        }

        public async void SignalWatchBasic(List<ContactsPackage> people, bool detect)
        {
            if (detect)
            {
                var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Seizure App Auto Sender");
                const string fromPassword = "xgfxsygrzzcenjlv";
                const string host = "smtp.gmail.com";
                const int port = 587;
                string subject = "Event Detected";
                string body = "This is an automated message from Seizure Detection App that an event has been recorded";



                //Send emails and text messages from contacts list
                foreach (var x in people)
                {
                    TextDriverFunc texter = new TextDriverFunc();
                    texter.Send(x);
                    MailPackage.sendMail(fromAddress, x, fromPassword, subject, body, host, port);
                    await System.Threading.Tasks.Task.Delay(2000);

                }
            }
            else
            {
                return;
            }
        }
    }

    //Contacts Class
    public class ContactsPackage
    {
        public MailAddress nameAddress { get; set; }
        public string phone { get; set; }
    }

    //Email Class
    public class MailPackage
    {
        //Email with attachment
        public static void sendMailAttach(MailAddress from, ContactsPackage too, string password, string subject, string body, string host, int port, Attachment attach)
        {
            try
            {
                using (var smtp = new SmtpClient(host))
                {
                    smtp.Host = host;
                    smtp.Port = port;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(from.Address, password);

                    using (var message = new MailMessage(from, too.nameAddress))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.Attachments.Add(attach);
                        smtp.Send(message);
                    };
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error: " + e.ToString());
            }
        }


        //Email with no attachment
        public static void sendMail(MailAddress from, ContactsPackage too, string password, string subject, string body, string host, int port)
        {
            try
            {
                using (var smtp = new SmtpClient(host))
                {
                    smtp.Host = host;
                    smtp.Port = port;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(from.Address, password);

                    using (var message = new MailMessage(from, too.nameAddress))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        smtp.Send(message);
                    };
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error: " + e.ToString());
            }
        }
    }

    //Text Class
    public class TextDriverFunc : MonoBehaviour
    {
        AndroidJavaObject currentActivity;
        ContactsPackage current;

        public void Send(ContactsPackage person)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                RunAndroidUiThread();
                current = person;
            }
        }

        void RunAndroidUiThread()
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(SendProcess));
        }

        void SendProcess()
        {
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            string phone = current.phone;
            string text = "Hello there general kenobi";

            try
            {
                AndroidJavaClass SMSManagerClass = new AndroidJavaClass("android.telephony.SmsManager");
                AndroidJavaObject SMSManagerObject = SMSManagerClass.CallStatic<AndroidJavaObject>("getDefault");
                SMSManagerObject.Call("sendTextMessage", phone, null, text, null, null);
            }
            catch (System.Exception e)
            {
                Debug.Log("Error : " + e.StackTrace.ToString());
            }
        }
    }
}

