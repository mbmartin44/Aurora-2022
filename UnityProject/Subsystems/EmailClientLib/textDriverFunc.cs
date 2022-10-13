using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


    public class TextDriverFunc : MonoBehaviour
    {
        AndroidJavaObject currentActivity;

        public void Send(string phone)
        {
            if(Application.platform == RuntimePlatform.Android)
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
