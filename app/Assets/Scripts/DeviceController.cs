using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Neuro;
using Neuro.Native;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class DeviceController : MonoBehaviour
{
    public UIController uiController;
    private DeviceEnumerator deviceEnumerator = null;
    private Device device;

    bool devicefounded = false;
    DeviceInfo deviceInfo;

    bool deviceStateChanged = false;
    
    void Start()
    {
#if UNITY_ANDROID
        Permission.RequestUserPermission("android.permission.BLUETOOTH");
        Permission.RequestUserPermission("android.permission.BLUETOOTH_ADMIN");
        Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");
        Permission.RequestUserPermission("android.permission.ACCESS_COARSE_LOCATION");
        Permission.RequestUserPermission("android.permission.ACCESS_BACKGROUND_LOCATION");
#endif
        createDeviceEnumerator();
    }

    private void createDeviceEnumerator()
    {
#if UNITY_IOS
        deviceEnumerator = new DeviceEnumerator(Neuro.Native.DeviceType.BrainbitAny);
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        deviceEnumerator = new DeviceEnumerator(Neuro.Native.DeviceType.BrainbitAny, GetJniEnv(), context.GetRawObject());
#endif
        deviceEnumerator.DeviceListChanged += OnDeviceFound;
        uiController.OnDeviceStateChange(true);
    }

    private void OnDeviceFound(object sender, System.EventArgs e)
    {
        List<DeviceInfo> deviceList = new List<DeviceInfo>(deviceEnumerator.Devices);
        Debug.Log("Devices found: " + deviceList.Count);

        deviceInfo = deviceList[0];
        devicefounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (devicefounded) {
            devicefounded = false;
            ConnectToDevice(deviceInfo);
        }

        if (deviceStateChanged)
        {
            deviceStateChanged = false;
            DeviceState state = device.ReadParam<DeviceState>(Parameter.State);
            if (state == DeviceState.Connected)
            {
                Debug.Log("Device connected");
                uiController.SaveDevice(device);
            }
            else
            {
                Debug.Log("Device disconnected");
                createDeviceEnumerator();
            }
            uiController.ShowMenu(state == DeviceState.Connected);
            uiController.OnDeviceStateChange(state != DeviceState.Connected);
        }

    }

    private void ConnectToDevice(DeviceInfo deviceInfo)
    {
        deviceEnumerator.DeviceListChanged -= OnDeviceFound;

        if (deviceInfo.Name.ToLower().Contains("black") && BoundDevice(deviceInfo.Id ?? "") || !deviceInfo.Name.ToLower().Contains("black"))
        {
            device = deviceEnumerator.CreateDevice(deviceInfo);
            device.ParameterChanged += OnDeviceParamChanged;
            device.Connect();
        }
    }

    private bool BoundDevice(string deviceId)
    {
        AndroidJavaClass clsBluetothAdapter = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
        AndroidJavaObject bAdapter = clsBluetothAdapter.CallStatic<AndroidJavaObject>("getDefaultAdapter");
        AndroidJavaObject devBounded = bAdapter.Call<AndroidJavaObject>("getBondedDevices");
        bool ready = false;
        var itr = devBounded.Call<AndroidJavaObject>("iterator");
        while (itr.Call<bool>("hasNext"))
        {
            var it = itr.Call<AndroidJavaObject>("next");
            if (it.Call<string>("getAddress").Equals(deviceId))
            {
                ready = true;
                break;
            }
        }
        if (!ready)
        {
            try
            {
                var dev = bAdapter.Call<AndroidJavaObject>("getRemoteDevice", deviceId);
                if (dev != null)
                {
                    AndroidJavaClass clsBluetoothDevice = new AndroidJavaClass("android.bluetooth.BluetoothDevice");
                    if (dev.Call<bool>("createBond"))
                    {
                        Thread.Sleep(300);
                        while (dev.Call<int>("getBondState") == clsBluetoothDevice.GetStatic<int>("BOND_BONDING"))
                        {
                            Thread.Sleep(15);
                        }
                        ready = dev.Call<int>("getBondState") == clsBluetoothDevice.GetStatic<int>("BOND_BONDED");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                // TODO: Log error
            }
        }
        return ready;
    }


    private void OnDeviceParamChanged(object sender, Parameter param)
    {
        if (param == Parameter.State) {
            deviceStateChanged = true;
        }
    }
    
    private void OnDestroy()
    {
        device.Disconnect();
    }

#if UNITY_ANDROID
    [DllImport("jnihelper-lib")]
    private static extern IntPtr GetJniEnv();
#endif
}
