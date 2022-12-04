///--------------------------------------------------------------------------------------
/// <file>    ChannelsController.cs                                </file>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
/// This class is responsible for managing the channels.
/// </summary>
/// -------------------------------------------------------------------------------------
/// <remarks>
///     This script is attached to the UIController object in the scene.
/// </remarks>
/// -------------------------------------------------------------------------------------

using System;
using Neuro;

/// <summary>
/// This class is responsible for managing the Brainbit channels.
/// </summary>
public class ChannelsController
{
    #region Battery
    Battery batteryController = null;

    /// <summary>
    /// Creates a new instance of the battery controller class.
    /// </summary>
    public void createBattery(Device device, Action<int> onPowerChanged)
    {
        batteryController = new Battery(device);
        batteryController.onPowerChanged = onPowerChanged;
    }

    /// <summary>
    /// Destroys the battery controller.
    /// </summary>
    public void destroyBattery()
    {
        if (batteryController != null)
        {
            batteryController.CloseChannel();
        }
    }
    #endregion

    #region EEG
    EegController eegController = null;
    /// <summary>
    /// Creates a new instance of the EEG controller class,
    /// registering the eegChanged callback function.
    /// </summary>
    public void createEeg(Device device, EventHandler<double[]> onEegChanged)
    {
        eegController = new EegController(device);
        eegController.onEegChanged = onEegChanged;
    }

    /// <summary>
    /// Get the size of the EEG plot.
    /// </summary>
    public int GetEegPlotSize()
    {
        return eegController.plotSize;
    }

    /// <summary>
    /// Destroys the EEG controller.
    /// </summary>
    public void destroyEeg(Device device)
    {
        eegController.CloseChannel(device);
    }
    #endregion

}
