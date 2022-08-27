using System;
using System.Collections;
using System.Collections.Generic;
using Neuro;
using UnityEngine;

public class Battery
{
    BatteryChannel batteryChannel;
    public Action<int> onPowerChanged;

    public Battery(Device device) {
        batteryChannel = new BatteryChannel(device);
        batteryChannel.LengthChanged += OnBatteryRecieved;
    }

    private void OnBatteryRecieved(object sender, int length)
    {
        int power = batteryChannel.ReadData(batteryChannel.TotalLength - 1, 1)[0];
        onPowerChanged?.Invoke(power);
    }

    public void CloseChannel()
    {
        batteryChannel.LengthChanged -= OnBatteryRecieved;
        batteryChannel.Dispose();
        batteryChannel = null;
        onPowerChanged = null;
    }
}
