using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textDriverFunc : MonoBehaviour
{
    AndroidJavaObject currentActivity;

    public void Send(bool detect)
    {
        if((Application.platform == RuntimePlatform.Android) && detect)
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
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        string phone = "8652366111";
        string text = "Hello there general kenobi";
        string alert;

        try
        {
            AndroidJavaClass SMSManagerClass = new AndroidJavaClass("android.telephony.SmsManager");
            AndroidJavaObject SMSManagerObject = SMSManagerClass.CallStatic<AndroidJavaObject>("getDefault");
            SMSManagerObject.Call("sendTextMessage", phone, null, text, null, null);
        }
        catch(Exception e)
        {
            Debug.Log("Error : " + e.StackTrace.ToString());
        }
    }
}
