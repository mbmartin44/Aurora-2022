using Neuro;
using Neuro.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ResistanceController
{
    private Dictionary<string, ResistanceChannel> resistChannels = new Dictionary<string, ResistanceChannel>();

    public EventHandler<double> onResistChanged;

    public ResistanceController(Device device)
    {
        if (DeviceTraits.HasChannelsWithType(device, ChannelType.Resistance))
        {
            foreach (ChannelInfo chInfo in device.Channels)
            {
                if (chInfo.Type == ChannelType.Resistance)
                {
                    if (!resistChannels.ContainsKey(chInfo.Name))
                    {
                        ResistanceChannel resistChannel = new ResistanceChannel(device, chInfo);
                        resistChannel.LengthChanged += onResistRecieved;
                        resistChannels.Add(chInfo.Name, resistChannel);
                    }
                }
            }
            device.Execute(Command.StartResist);
        }
    }

    private void onResistRecieved(object sender, int e)
    {
        AnyChannel anyChannel = (AnyChannel)sender;
        ResistanceChannel resistChannel = resistChannels[anyChannel.Info.Name];
        double lastSample = resistChannel.ReadData(resistChannel.TotalLength - 1, 1)[0];
        onResistChanged?.Invoke(sender, lastSample);
    }

    public void CloseChannel(Device device)
    {
        device.Execute(Command.StopResist);
        foreach (ResistanceChannel resistChannel in resistChannels.Values)
        {
            resistChannel.LengthChanged -= onResistRecieved;
            resistChannel.Dispose();
        }
        resistChannels.Clear();
        onResistChanged = null;
    }
}
