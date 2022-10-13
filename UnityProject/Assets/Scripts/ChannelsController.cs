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

    #region Signal
    SignalController signalController = null;
    public void createSignal(Device device, EventHandler<double[]> onSignalRecieved)
    {
        signalController = new SignalController(device);
        signalController.onSignalRecieved = onSignalRecieved;
    }

    public int GetSignalPlotSize()
    {
        return signalController.plotSize;
    }

    public void destroySignal(Device device)
    {
        signalController.CloseChannels(device);
    }
    #endregion

    #region Resistance
    ResistanceController resistanceController = null;
    public void createResistance(Device device, EventHandler<double> onResistRecieved)
    {
        resistanceController = new ResistanceController(device);
        resistanceController.onResistChanged = onResistRecieved;
    }

    public void destroyResistance(Device device)
    {
        resistanceController.CloseChannel(device);
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

    #region EegIndex
    EegIndexController indexController = null;
    public void createEegIdx(Device device, Action<EegIndexValues> onEegIdxChanged)
    {
        indexController = new EegIndexController(device);
        indexController.onEegIdxChanged = onEegIdxChanged;
    }

    public void destroyEegIdx(Device device)
    {
        indexController.CloseChannel(device);
    }
    #endregion

    #region Spectrum
    SpectrumController spectrumController = null;
    public void createSpectrum(Device device, EventHandler<double[]> onSpectrumChanged)
    {
        spectrumController = new SpectrumController(device);
        spectrumController.onSpectrumChanged = onSpectrumChanged;
    }

    public int GetSpectrumPlotSize()
    {
        return spectrumController.plotSize;
    }

    public void destroySpectrum(Device device)
    {
        spectrumController.CloseChannel(device);
    }
    #endregion

    #region SpectrumPower
    SpectrumPowerController spectrumPowerController = null;
    public void createSpectrumPower(Device device, EventHandler<double> onSpectrumPowerChanged)
    {
        spectrumPowerController = new SpectrumPowerController(device);
        spectrumPowerController.onSpectrumPowerChanged += onSpectrumPowerChanged;
    }

    public void destroySpectrumPower(Device device)
    {
        spectrumPowerController.CloseChannel(device);
    }
    #endregion

    #region EmotionAnalyzer
    EmotionsController emotionsController = null;
    public void createEmotions(Device device, Action<EmotionsSample> onEmotionalStateChanged, Action<IEmotionsAnalyzerState> onAnalyzerStateChanged)
    {
        emotionsController = new EmotionsController(device);
        emotionsController.onEmotionalStateChanged += onEmotionalStateChanged;
        emotionsController.onAnalyzerStateChanged += onAnalyzerStateChanged;
    }

    public void destroyEmotions(Device device)
    {
        emotionsController.CloseChannel(device);
    }
    #endregion
}
