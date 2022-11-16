/*
 * Author: Keaton Shelton
 * Date: August 25th, 2022
 * Arguments: ContactsPackage
 * Returns: n/a
 *
 * Abstract:
 *      This class contains the various functions and routines
 *  that are needed for the texting client to work.
 *
 *  Revisions:
 *  01ks - August 25th, 2022 - Original
 *  02ks - August 29th, 2022 - Add in MMS support
 *  03ks - August 31st, 2022 - Begin Gateway Array Setup
 *  04ks - October 17th, 2022 - Name Fix
 *  05ks - November 16th, 2022 - Check for blank phone;
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using UnityEngine;

public class TextDriver : MonoBehaviour
{
    AndroidJavaObject currentActivity;
    ContactsPackage current;

    public void Send(ContactsPackage person)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            current = person;
            //Check for blank phone
            //05ks
            if(current.phone == "")
            {
                return;
            }
            RunAndroidUiThread();
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