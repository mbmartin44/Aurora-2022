///--------------------------------------------------------------------------------------
/// <file>    TextDriver.cs                                        </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     This class contains the various functions and routines
///     that are needed for the texting client to work.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using UnityEngine;

/// <summary>
/// This class contains various functions and routines
/// that are needed for the texting client to work.
/// </summary>
public class TextDriver : MonoBehaviour
{
    AndroidJavaObject currentActivity;
    ContactsPackage current;

    /// <summary>
    /// This is the default constructor for the TextDriver class.
    /// </summary>
    public void Send(ContactsPackage person)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            current = person;
            //Check for blank phone
            //05ks
            if (current.phone == "")
            {
                return;
            }
            RunAndroidUiThread();
        }
    }

    /// <summary>
    /// Evokes SendProcess(), a method that runs on the main thread of the Android application.
    /// It is run by RunAndroidUiThread() by calling the currentActivity.Call() method.
    /// It is used to send a message to the main thread of the application.
    /// </summary>
    void RunAndroidUiThread()
    {
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(SendProcess));
    }

    /// <summary>
    /// This method is used to send a text message to the contact.
    /// </summary>
    void SendProcess()
    {
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        string phone = current.phone;
        string text = "This is an automated message from Seizure Detection App that an event has been recorded";

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