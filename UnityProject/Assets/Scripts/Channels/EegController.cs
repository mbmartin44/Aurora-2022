///--------------------------------------------------------------------------------------
/// <file>    EegController.cs                                </file>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
/// This class is used to control the EEG channels.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;

/// <summary>
/// This class is used to control the EEG channels.
/// </summary>
public class EegController
{
    private Dictionary<string, EegChannel> eegChannels = new Dictionary<string, EegChannel>();
    private Dictionary<string, int> signalOffsets = new Dictionary<string, int>();
    public EventHandler<double[]> onEegChanged;
    int windowDuration = 2;
    public int plotSize = 0;
    public double LLEtest = 0;


    public double[] signalSamples;


    /// <summary>
    /// This function initializes the EEG channels.
    /// </summary>
    public EegController(Device device)
    {

        // If the device has a channel of the specified type, then create a new instance of the EegChannel class.
        if (DeviceTraits.HasChannelsWithType(device, ChannelType.Signal))
        {
            foreach (ChannelInfo chInfo in device.Channels)
            {
                if (chInfo.Type == ChannelType.Signal)
                {
                    if (!eegChannels.ContainsKey(chInfo.Name))
                    {
                        EegChannel eegChannel = new EegChannel(device, chInfo);
                        eegChannel.LengthChanged += OnSignalChanged;
                        eegChannels.Add(chInfo.Name, eegChannel);
                        signalOffsets.Add(chInfo.Name, 0);

                        plotSize = (int)Mathf.Ceil(eegChannel.SamplingFrequency * windowDuration);

                    }
                }
            }
            // Execute the start signal command.
            device.Execute(Command.StartSignal);
        }
    }

    /// <summary>
    /// This eventHandler is triggered when new EEG values are received.
    /// </summary>
    private void OnSignalChanged(object sender, int length)
    {

        AnyChannel anyChannel = (AnyChannel)sender;
        EegChannel signalChannel = eegChannels[anyChannel.Info.Name];
        int totalLength = signalChannel.TotalLength;
        int readLength = totalLength - signalOffsets[signalChannel.Info.Name];
        // Here, the data is read from the channel.
        // We then apply a low-pass filter to the data.
        double[] signalSamples = signalChannel.ReadData(signalOffsets[signalChannel.Info.Name], readLength).ApplyLPF(true, (int)signalChannel.SamplingFrequency, 2);
        signalOffsets[signalChannel.Info.Name] += readLength;

        // Finally, we send the data to the event handler.
        onEegChanged?.Invoke(sender, signalSamples);

    }

    /// <summary>
    /// This method closes the EEG channels.
    /// </summary>
    public void CloseChannel(Device device)
    {
        device.Execute(Command.StopSignal);
        foreach (EegChannel eegChannel in eegChannels.Values)
        {

            eegChannel.LengthChanged -= OnSignalChanged;
            eegChannel.Dispose();
        }
        eegChannels.Clear();
        signalOffsets.Clear();
        onEegChanged = null;
    }

}
