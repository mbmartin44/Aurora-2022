using Neuro;
using Neuro.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EegIndexController
{
    private Dictionary<string, EegChannel> eegChannels = new Dictionary<string, EegChannel>();
    EegIndexChannel eegIndexChannel = null;

    public Action<EegIndexValues> onEegIdxChanged;

    public EegIndexController(Device device)
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
                        eegChannels.Add(chInfo.Name, eegChannel);
                    }
                }
            }
            eegIndexChannel = new EegIndexChannel(
                eegChannels["T3"],
                eegChannels["T4"],
                eegChannels["O1"],
                eegChannels["O2"]
            );
            eegIndexChannel.LengthChanged += OnEegIdxRecieved;
            device.Execute(Command.StartSignal);
        }
        
    
    }

    private void OnEegIdxRecieved(object sender, int e)
    {
        EegIndexValues values = eegIndexChannel.ReadData(eegIndexChannel.TotalLength - 1, 1)[0];
        onEegIdxChanged?.Invoke(values);
    }

    public void CloseChannel(Device device)
    {
        device.Execute(Command.StopSignal);

        foreach (EegChannel eegChannel in eegChannels.Values)
        {
            eegChannel.Dispose();
        }
        eegChannels.Clear();

        eegIndexChannel.LengthChanged -= OnEegIdxRecieved;
        eegIndexChannel.Dispose();
        eegIndexChannel = null;
        onEegIdxChanged = null;
    }
}


