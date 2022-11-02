using System;
using System.Collections;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;
public class ChannelsController
{
    #region Battery
    Battery batteryController = null;

    public void createBattery(Device device, Action<int> onPowerChanged)
    {
        batteryController = new Battery(device);
        batteryController.onPowerChanged = onPowerChanged;
    }

    public void destroyBattery() {
        batteryController.CloseChannel();
    }
    #endregion

    #region EEG
    EegController eegController = null;
    public void createEeg(Device device, EventHandler<double[]> onEegChanged)
    {
        eegController = new EegController(device);
        eegController.onEegChanged = onEegChanged;
    }

    public int GetEegPlotSize()
    {
        return eegController.plotSize;
    }

    public void destroyEeg(Device device)
    {
        eegController.CloseChannel(device);
    }
    #endregion

}
