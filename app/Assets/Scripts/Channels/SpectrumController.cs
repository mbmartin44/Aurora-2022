using Neuro;
using Neuro.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpectrumController
{
    private Dictionary<string, SpectrumChannel> spectrumChannels = new Dictionary<string, SpectrumChannel>();

    public EventHandler<double[]> onSpectrumChanged;

    public int plotSize = 0;

    public SpectrumController(Device device) 
    {
        foreach (ChannelInfo chInfo in device.Channels)
        {
            if (chInfo.Type == ChannelType.Signal)
            {
                if (!spectrumChannels.ContainsKey(chInfo.Name))
                {
                    SpectrumChannel spectrumChannel = new SpectrumChannel(new EegChannel(device, chInfo));
                    spectrumChannel.LengthChanged += OnSpectrumRecieved;
                    spectrumChannels.Add(chInfo.Name, spectrumChannel);
                    plotSize = 1024;
                }
            }
        }
        device.Execute(Command.StartSignal);
    }

    private void OnSpectrumRecieved(object sender, int e)
    {
        try
        {
            AnyChannel anyChannel = (AnyChannel)sender;
            SpectrumChannel spectrumChannel = spectrumChannels[anyChannel.Info.Name.Split(' ')[1]];
            double[] samples = spectrumChannel.ReadData(spectrumChannel.TotalLength - plotSize, plotSize);
            onSpectrumChanged?.Invoke(sender, samples);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void CloseChannel(Device device)
    {
        device.Execute(Command.StopSignal);
        foreach (SpectrumChannel spectrumChannel in spectrumChannels.Values)
        {
            spectrumChannel.LengthChanged -= OnSpectrumRecieved;
        }
        spectrumChannels.Clear();

        onSpectrumChanged = null;
    }
}
