///--------------------------------------------------------------------------------------
/// <file>    DeviceController.cs                                </file>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
/// This class is responsible for managing the channels.
/// </summary>
/// -------------------------------------------------------------------------------------
/// <remarks>
///     This script is attached to the UIController object in the scene.
/// </remarks>
/// -------------------------------------------------------------------------------------

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

/// <summary>
/// This class is used to control the BrainBit device.
/// </summary>
public class DeviceController : MonoBehaviour
{
    public UIController uiController;
    private DeviceEnumerator deviceEnumerator = null;
    private Device device;

    bool devicefounded = false;
    DeviceInfo deviceInfo;

    bool deviceStateChanged = false;

    /// <summary>
    /// When the application starts, this function is called.
    /// </summary>
    void Start()
    {
#if UNITY_ANDROID
        // Request permissions for Android 6.0+
        Permission.RequestUserPermission("android.permission.BLUETOOTH");
        Permission.RequestUserPermission("android.permission.BLUETOOTH_ADMIN");
        Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");
        Permission.RequestUserPermission("android.permission.ACCESS_COARSE_LOCATION");
        Permission.RequestUserPermission("android.permission.ACCESS_BACKGROUND_LOCATION");
#endif
        // Create a new instance of the device enumerator class.
        createDeviceEnumerator();
    }

    private void createDeviceEnumerator()
    {
#if UNITY_EDITOR
        deviceEnumerator = new DeviceEnumerator(Neuro.Native.DeviceType.BrainbitAny);
#elif UNITY_ANDROID
        // The AndroidJavaClass class is used to call static java methods.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        deviceEnumerator = new DeviceEnumerator(Neuro.Native.DeviceType.BrainbitAny, GetJniEnv(), context.GetRawObject());
#endif
        deviceEnumerator.DeviceListChanged += OnDeviceFound;
        uiController.OnDeviceStateChange(true);
    }

    /// <summary>
    /// When the device is found, the device enumerator calls this function,
    /// which in turn calls the UI controller to update the device list.
    /// </summary>
    private void OnDeviceFound(object sender, System.EventArgs e)
    {
        List<DeviceInfo> deviceList = new List<DeviceInfo>(deviceEnumerator.Devices);
        Debug.Log("Devices found: " + deviceList.Count);

        deviceInfo = deviceList[0];
        devicefounded = true;
    }

    /// <summary>
    /// The update function is called once per frame.
    /// </summary>
    void Update()
    {
        // If the device is not connected, try to connect.
        if (devicefounded)
        {
            devicefounded = false;
            ConnectToDevice(deviceInfo);
        }

        // If the device state has changed, update the UI.
        if (deviceStateChanged)
        {
            deviceStateChanged = false;
            // Get the device state.
            DeviceState state = device.ReadParam<DeviceState>(Parameter.State);
            // If the device is connected, start the data stream.
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
            // Update the UI with the connection status.
            uiController.ShowMenu(state == DeviceState.Connected);
            uiController.OnDeviceStateChange(state != DeviceState.Connected);
        }

    }

    /// <summary>
    /// Connect to the device.
    /// </summary>
    private void ConnectToDevice(DeviceInfo deviceInfo)
    {
        // Register the device state changed event handler.
        deviceEnumerator.DeviceListChanged -= OnDeviceFound;

        // Special connection parameters for the Brainbit Black device (ignore these for other devices).
        if (deviceInfo.Name.ToLower().Contains("black") && BoundDevice(deviceInfo.Id ?? "") || !deviceInfo.Name.ToLower().Contains("black"))
        {
            device = deviceEnumerator.CreateDevice(deviceInfo);
            device.ParameterChanged += OnDeviceParamChanged;
            device.Connect();
        }
    }

    /// <summary>
    /// This method is used to establish a bluetooth connection with the Brainbit headband.
    /// </summary>
    private bool BoundDevice(string deviceId)
    {
        AndroidJavaClass clsBluetothAdapter = new AndroidJavaClass("android.bluetooth.BluetoothAdapter");
        AndroidJavaObject bAdapter = clsBluetothAdapter.CallStatic<AndroidJavaObject>("getDefaultAdapter");
        AndroidJavaObject devBounded = bAdapter.Call<AndroidJavaObject>("getBondedDevices");
        bool ready = false;
        var itr = devBounded.Call<AndroidJavaObject>("iterator");

        // Iterate through the list of paired devices.
        while (itr.Call<bool>("hasNext"))
        {
            var it = itr.Call<AndroidJavaObject>("next");
            // If the device is found, connect to it.
            if (it.Call<string>("getAddress").Equals(deviceId))
            {
                ready = true;
                break;
            }
        }
        // If the device is not paired, try to pair it.
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
                            Thread.Sleep(10);
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
        if (param == Parameter.State)
        {
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
