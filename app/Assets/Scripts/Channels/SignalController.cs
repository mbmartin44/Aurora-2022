using System;
using System.Collections;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;

public class SignalController
{
    private Dictionary<string, SignalChannel> signalChannels = new Dictionary<string, SignalChannel>();
    private Dictionary<string, int> signalOffsets = new Dictionary<string, int>();

    public EventHandler<double[]> onSignalRecieved;

    int windowDuration = 5;
    public int plotSize = 0;

    public SignalController(Device device)
    {
        foreach (ChannelInfo chInfo in device.Channels)
        {
            if (chInfo.Type == ChannelType.Signal)
            {
                if (!signalChannels.ContainsKey(chInfo.Name))
                {
                    SignalChannel signalChannel = new SignalChannel(device, chInfo);
                    signalChannel.LengthChanged += OnSignalChanged;
                    signalChannels.Add(chInfo.Name, signalChannel);
                    signalOffsets.Add(chInfo.Name, 0);
                    plotSize = (int)Mathf.Ceil(signalChannel.SamplingFrequency * windowDuration);
                }
            }
        }
        device.Execute(Command.StartSignal);
    }

    private void OnSignalChanged(object sender, int e)
    {
        AnyChannel anyChannel = (AnyChannel)sender;
        SignalChannel signalChannel = signalChannels[anyChannel.Info.Name];
        int totalLength = signalChannel.TotalLength;
        int readLength = totalLength - signalOffsets[signalChannel.Info.Name];
        double[] signalSamples = signalChannel.ReadData(signalOffsets[signalChannel.Info.Name], readLength);
        signalOffsets[signalChannel.Info.Name] += readLength;
        onSignalRecieved?.Invoke(sender, signalSamples);
    }

    public void CloseChannels(Device device)
    {
        device.Execute(Command.StopSignal);
        foreach (SignalChannel signalChannel in signalChannels.Values)
        {
            signalChannel.LengthChanged -= OnSignalChanged;
            signalChannel.Dispose();
        }
        signalChannels.Clear();
        signalOffsets.Clear();
        onSignalRecieved = null;
    }

}
