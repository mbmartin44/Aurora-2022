using System;
using System.Collections;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;

public class EegController
{
    private Dictionary<string, EegChannel> eegChannels = new Dictionary<string, EegChannel>();
    private Dictionary<string, int> signalOffsets = new Dictionary<string, int>();

    public EventHandler<double[]> onEegChanged;

    int windowDuration = 5;
    public int plotSize = 0;

    public EegController(Device device)
    {
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
            device.Execute(Command.StartSignal);
        }
    }

    private void OnSignalChanged(object sender, int length)
    {
        //lock (synchObj)
        //{
            AnyChannel anyChannel = (AnyChannel)sender;
            EegChannel signalChannel = eegChannels[anyChannel.Info.Name];
            int totalLength = signalChannel.TotalLength;
            int readLength = totalLength - signalOffsets[signalChannel.Info.Name];
            double[] signalSamples = signalChannel.ReadData(signalOffsets[signalChannel.Info.Name], readLength);
            signalOffsets[signalChannel.Info.Name] += readLength;

            onEegChanged?.Invoke(sender, signalSamples);
        //}
    }

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
