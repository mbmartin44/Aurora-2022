using Neuro;
using Neuro.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpectrumPowerController
{
    private Dictionary<string, SpectrumPowerChannel> spectrumPowerChannels = new Dictionary<string, SpectrumPowerChannel>();

    public EventHandler<double> onSpectrumPowerChanged;

    public SpectrumPowerController(Device device)
    {
        foreach (ChannelInfo chInfo in device.Channels)
        {
            if (chInfo.Type == ChannelType.Signal)
            {
                if (!spectrumPowerChannels.ContainsKey(chInfo.Name))
                {
                    SpectrumPowerChannel spectrumPowerChannel = new SpectrumPowerChannel(new SpectrumChannel[] { new SpectrumChannel(new EegChannel(device, chInfo)) }, 8, 13, chInfo.Name);
                    spectrumPowerChannel.LengthChanged += onSpectrumPowerRecieved;
                    spectrumPowerChannels.Add(chInfo.Name, spectrumPowerChannel);
                }
            }
        }
        device.Execute(Command.StartSignal);
    }

    private void onSpectrumPowerRecieved(object sender, int e)
    {
        AnyChannel anyChannel = (AnyChannel)sender;
        SpectrumPowerChannel spectrumPowerChannel = spectrumPowerChannels[anyChannel.Info.Name];
        double sample = spectrumPowerChannel.ReadData(spectrumPowerChannel.TotalLength - 1, 1)[0];

        onSpectrumPowerChanged?.Invoke(sender, sample);
    }

    public void CloseChannel(Device device)
    {
        device.Execute(Command.StopSignal);
        foreach (SpectrumPowerChannel spectrumPowerChannel in spectrumPowerChannels.Values)
        {
            spectrumPowerChannel.LengthChanged -= onSpectrumPowerRecieved;
            spectrumPowerChannel.Dispose();
        }
        spectrumPowerChannels.Clear();

        onSpectrumPowerChanged = null;
    }
}
