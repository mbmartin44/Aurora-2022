using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendSMS : MonoBehaviour
{
    AndroidJavaObject currentActivity;

    public void Send(string phone)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
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
        Debug.Log("Running on UI thread");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        // SMS Information

        string phone = "93131987"; 
        string text = "Hello World. This SMS is sent using Android SMS Manager.";
        string alert;

        try
        {
            // SMS Manager

            AndroidJavaClass SMSManagerClass = new AndroidJavaClass("android.telephony.SmsManager");
            AndroidJavaObject SMSManagerObject = SMSManagerClass.CallStatic<AndroidJavaObject>("getDefault");
            SMSManagerObject.Call("sendTextMessage", phone, null, text, null, null);

            alert = "Message sent successfully.";
        }
        catch (System.Exception e)
        {
            Debug.Log("Error : " + e.StackTrace.ToString());

            alert = "Error has been Occurred. Fail to send message.";
        }

        // Show Toast

        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", alert);
        AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
        toast.Call("show");
    }
}