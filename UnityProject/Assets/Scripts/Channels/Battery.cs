///--------------------------------------------------------------------------------------
/// <file>    Battery.cs                                </file>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
/// This class is used to control the device's battery
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using Neuro;

/// <summary>
/// This class is used to control the battery.
/// </summary>
public class Battery
{

    BatteryChannel batteryChannel;
    public Action<int> onPowerChanged;

    /// <summary>
    /// This function initializes the battery object.
    /// </summary>
    public Battery(Device device)
    {
        batteryChannel = new BatteryChannel(device);
        batteryChannel.LengthChanged += OnBatteryRecieved;
    }

    /// <summary>
    /// This eventHandler is triggered when the battery value is received.
    /// </summary>
    private void OnBatteryRecieved(object sender, int length)
    {
        int power = batteryChannel.ReadData(batteryChannel.TotalLength - 1, 1)[0];
        onPowerChanged?.Invoke(power);
    }

    /// <summary>
    /// This methods closes the battery channel.
    /// </summary>
    public void CloseChannel()
    {
        batteryChannel.LengthChanged -= OnBatteryRecieved;
        batteryChannel.Dispose();
        batteryChannel = null;
        onPowerChanged = null;
    }
}
