using System;
using System.Collections;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using PrimaryInterface;


public sealed class UIController : MonoBehaviour
{
    
    


    ChannelsController channelsController = null;
    Device device = null;

    public GameObject modesVariations;
    public GameObject deviceSearchLabel;
    public GameObject Title;

    [Header("== DeviceState UI ==")]
    public Text deviceConnectionState;
    public Text devicePowerState;
    private int devicePower = 0;

    [Header("== DeviceInfo UI ==")]
    public GameObject deviceInfoOutput;
    public Text deviceInfoText;

    [Header("== Signal UI ==")]
    public GameObject signalOutput;
    public Graph signalO1Graph;
    public Graph signalO2Graph;
    public Graph signalT3Graph;
    public Graph signalT4Graph;

    [Header("== Resistance UI ==")]
    public GameObject resistOutput;
    public Text O1Resist;
    private double rawO1Resist = 0;
    public Text O2Resist;
    private double rawO2Resist = 0;
    public Text T3Resist;
    private double rawT3Resist = 0;
    public Text T4Resist;
    private double rawT4Resist = 0;

    [Header("== EEG UI ==")]
    public GameObject eegOutput;
    public Graph eegO1Graph;
    public Graph eegO2Graph;
    public Graph eegT3Graph;
    public Graph eegT4Graph;

    [Header("== EEG Index UI ==")]
    public GameObject eegIndexOutput;
    public Text alphaIdx;
    public Text betaIdx;
    public Text thetaIdx;
    public Text deltaIdx;
    private EegIndexValues indexValues = new EegIndexValues();

    [Header("== EmotionAnalyzer UI ==")]
    public GameObject emotionsOutput;
    private float artefactRate = 0.0f;
    private float calibrationProgress = 0.0f;
    private SignalSource source;
    private DataQuality quality;
    private float deltaRate;
    private float thetaRate;
    private float alphaRate;
    private float betaRate;
    private float relaxationRate;
    private float concentrationRate;
    private float meditationProgress;
    public Text artefactRateText;
    public Text calibrationProgressText;
    public Text stateText;
    public Text sourceText;
    public Text qualityText;
    public Text deltaRateText;
    public Text thetaRateText;
    public Text alphaRateText;
    public Text betaRateText;
    public Text relaxationRateText;
    public Text concentrationRateText;
    public Text meditationProgressText;

    [Header("== Spectrum UI ==")]
    public GameObject spectrumOutput;
    public Graph spectrumO1Graph;
    public Graph spectrumO2Graph;
    public Graph spectrumT3Graph;
    public Graph spectrumT4Graph;

    [Header("== Spectrum Power UI ==")]
    public GameObject specrtumPowerOutput;
    public Text spectrumPowerO1Text;
    public Text spectrumPowerO2Text;
    public Text spectrumPowerT3Text;
    public Text spectrumPowerT4Text;
    private double spectrumPowerO1Value = 0;
    private double spectrumPowerO2Value = 0;
    private double spectrumPowerT3Value = 0;
    private double spectrumPowerT4Value = 0;





    private void Awake()
    {
        channelsController = new ChannelsController();
    }

    private void Start()
    {
        ShowMenu(false);
    }

    private void FixedUpdate()
    {
        devicePowerState.text = string.Format("Power: {0}%", devicePower);

        alphaIdx.text = string.Format("Alpha: {0:F4}", indexValues.AlphaRate);
        betaIdx.text = string.Format("Beta: {0:F4}", indexValues.BetaRate);
        thetaIdx.text = string.Format("Theta: {0:F4}", indexValues.ThetaRate);
        deltaIdx.text = string.Format("Delta: {0:F4}", indexValues.DeltaRate);

        artefactRateText.text = string.Format("Artefact rate: {0:F2}", artefactRate);
        calibrationProgressText.text = string.Format("Calibration progress: {0:F2}", calibrationProgress);
        sourceText.text = string.Format("Signal source: {0}", source.ToString());
        qualityText.text = string.Format("Data quality: {0}", quality.ToString());
        deltaRateText.text = string.Format("Delta rate: {0:F2}", deltaRate);
        thetaRateText.text = string.Format("Theta rate: {0:F2}", thetaRate);
        alphaRateText.text = string.Format("Alpha rate: {0:F2}", alphaRate);
        betaRateText.text = string.Format("Beta rate: {0:F2}", betaRate);
        relaxationRateText.text = string.Format("Relaxation rate: {0:F2}", relaxationRate);
        concentrationRateText.text = string.Format("Concentration rate: {0:F2}", concentrationRate);
        meditationProgressText.text = string.Format("Meditation progress: {0:F2}", meditationProgress);


        spectrumPowerO1Text.text = string.Format("O1:  {0:F4}", spectrumPowerO1Value);
        spectrumPowerO2Text.text = string.Format("O2:  {0:F4}", spectrumPowerO2Value);
        spectrumPowerT3Text.text = string.Format("T3:  {0:F4}", spectrumPowerT3Value);
        spectrumPowerT4Text.text = string.Format("T4:  {0:F4}", spectrumPowerT4Value);

        O1Resist.text = string.Format("O1: {0:F2} Om", rawO1Resist);
        O2Resist.text = string.Format("O2: {0:F2} Om", rawO2Resist);
        T3Resist.text = string.Format("T3: {0:F2} Om", rawT3Resist);
        T4Resist.text = string.Format("T4: {0:F2} Om", rawT4Resist);
    }

    public void SaveDevice(Device device)
    {
        this.device = device;
    }
    
    

    #region DeviceInfo
    public void ShowDeviceInfo()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        deviceInfoOutput.SetActive(true);
        //Sending();
        //string phone = "9313191687";
        //var anInstanceofSMS = new SendSMS();
        //anInstanceofSMS.Send(phone);
        ContactsPackage contact = new ContactsPackage();
        contact.phone = "9313355335";
        List<ContactsPackage> people = new List<ContactsPackage>();
        people.Add(contact);
        PrimaryInterface.clientInterface.SignalWatch(people, null, true, true);
        GetDeviceInfo();
        
        
        






    }

    public void CloseDeviceInfo()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        deviceInfoOutput.SetActive(false);
    }
    #endregion

    #region Signal
    public void ShowSignal()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        signalOutput.SetActive(true);
        channelsController.createSignal(device, (channel, samples) =>
        {
            AnyChannel anyChannel = (AnyChannel)channel;
            switch (anyChannel.Info.Name)
            {
                case "O1":
                    signalO1Graph.UpdateGraph(samples);
                    break;
                case "O2":
                    signalO2Graph.UpdateGraph(samples);
                    break;
                case "T3":
                    signalT3Graph.UpdateGraph(samples);
                    break;
                case "T4":
                    signalT4Graph.UpdateGraph(samples);
                    break;
            }
        });
        signalO1Graph.InitGraph(channelsController.GetSignalPlotSize());
        signalO2Graph.InitGraph(channelsController.GetSignalPlotSize());
        signalT3Graph.InitGraph(channelsController.GetSignalPlotSize());
        signalT4Graph.InitGraph(channelsController.GetSignalPlotSize());
    }

    public void CloseSignal()
    {
        modesVariations.SetActive(true);
        signalOutput.SetActive(false);
        channelsController.destroySignal(device);
        signalO1Graph.Close();
        signalO2Graph.Close();
        signalT3Graph.Close();
        signalT4Graph.Close();
    }
    #endregion

    #region Resistance
    public void ShowResistance()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        resistOutput.SetActive(true);
        channelsController.createResistance(device, (channel, lastsample) =>
        {
            AnyChannel anyChannel = (AnyChannel)channel;
            switch (anyChannel.Info.Name)
            {
                case "O1":
                    rawO1Resist = lastsample;
                    break;
                case "O2":
                    rawO2Resist = lastsample;
                    break;
                case "T3":
                    rawT3Resist = lastsample;
                    break;
                case "T4":
                    rawT4Resist = lastsample;
                    break;
            }
        });
    }

    public void CloseResistance()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        resistOutput.SetActive(false);
        channelsController.destroyResistance(device);
    }
    #endregion

    #region EEG
    public void ShowEEG()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        eegOutput.SetActive(true);
        channelsController.createEeg(device, (channel, samples) =>
        
        {
            AnyChannel anyChannel = (AnyChannel)channel;
            switch (anyChannel.Info.Name)
            {
                case "O1":
                    eegO1Graph.UpdateGraph(samples);
                    break;
                case "O2":
                    eegO2Graph.UpdateGraph(samples);
                    break;
                case "T3":
                    eegT3Graph.UpdateGraph(samples);
                    break;
                case "T4":
                    eegT4Graph.UpdateGraph(samples);
                    break;
            }
        });
        eegO1Graph.InitGraph(channelsController.GetEegPlotSize());
        eegO2Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT3Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT4Graph.InitGraph(channelsController.GetEegPlotSize());
    }

    public void CloseEEG()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        eegOutput.SetActive(false);
        channelsController.destroyEeg(device);
        eegO1Graph.Close();
        eegO2Graph.Close();
        eegT3Graph.Close();
        eegT4Graph.Close();
    }
    #endregion

    #region EegIndex
    public void ShowEegIndex()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        eegIndexOutput.SetActive(true);
        channelsController.createEegIdx(device, (ids) =>
        {
            indexValues = ids;
        });
    }

    public void CloseEegIndex()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        eegIndexOutput.SetActive(false);
        channelsController.destroyEegIdx(device);
    }
    #endregion

    #region Spectrum
    public void ShowSpectrum()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        spectrumOutput.SetActive(true);
        channelsController.createSpectrum(device, (channel, samples) =>
        {
            AnyChannel anyChannel = (AnyChannel)channel;
            if (anyChannel.Info.Name.Contains("T3"))
            {
                spectrumT4Graph.UpdateGraph(samples);
            }
            if (anyChannel.Info.Name.Contains("T4"))
            {
                spectrumT3Graph.UpdateGraph(samples);
            }
            if (anyChannel.Info.Name.Contains("O1"))
            {
                spectrumO1Graph.UpdateGraph(samples);
            }
            if (anyChannel.Info.Name.Contains("O2"))
            {
                spectrumO2Graph.UpdateGraph(samples);
            }
        });
        spectrumO1Graph.InitGraph(channelsController.GetSpectrumPlotSize());
        spectrumO2Graph.InitGraph(channelsController.GetSpectrumPlotSize());
        spectrumT3Graph.InitGraph(channelsController.GetSpectrumPlotSize());
        spectrumT4Graph.InitGraph(channelsController.GetSpectrumPlotSize());
    }

    public void CloseSpectrum()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        spectrumOutput.SetActive(false);
        channelsController.destroySpectrum(device);
        spectrumO1Graph.Close();
        spectrumO2Graph.Close();
        spectrumT3Graph.Close();
        spectrumT4Graph.Close();
    }
    #endregion

    #region SpectrumPower
    public void ShowSpectrumPower()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        specrtumPowerOutput.SetActive(true);
        channelsController.createSpectrumPower(device, (channel, power) =>
        {
            AnyChannel anyChannel = (AnyChannel)channel;
            Debug.Log($"Channel name = {anyChannel.Info.Name}");
            switch (anyChannel.Info.Name)
            {
                case "O1":
                    spectrumPowerO1Value = power * 1e3;
                    break;
                case "O2":
                    spectrumPowerO2Value = power * 1e3;
                    break;
                case "T3":
                    spectrumPowerT3Value = power * 1e3;
                    break;
                case "T4":
                    spectrumPowerT4Value = power * 1e3;
                    break;
            }
        });
    }

    public void CloseSpectrumPower()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        specrtumPowerOutput.SetActive(false);
        channelsController.destroySpectrumPower(device);
    }
    #endregion

    #region Emotions
    public void ShowEmotions()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        emotionsOutput.SetActive(true);
        channelsController.createEmotions(device,
            (sample) =>
            {
                source = sample.Source;
                quality = sample.Quality;
                deltaRate = sample.DeltaRate;
                thetaRate = sample.ThetaRate;
                alphaRate = sample.AlphaRate;
                betaRate = sample.BetaRate;
                relaxationRate = sample.RelaxationRate;
                concentrationRate = sample.ConcentrationRate;
                meditationProgress = sample.MeditationProgress;
            },
        (state) =>
        {
            if (state is StateCalibrating calibration)
            {
                artefactRate = calibration.ArtifactRate;
                calibrationProgress = calibration.Progress;
            }
            else if (state is StateWorking working)
            {

            }

        });
    }

    public void CloseEmotions()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        emotionsOutput.SetActive(false);
        channelsController.destroyEmotions(device);
    }
    #endregion

    public void ShowMenu(bool enabled)
    {
        modesVariations.SetActive(enabled);
    }

    public void OnDeviceStateChange(bool disconnected)
    {
        modesVariations?.SetActive(!disconnected);
        Title?.SetActive(!disconnected);
        deviceSearchLabel?.SetActive(disconnected);
        deviceConnectionState.text = disconnected ? "Disconnected" : "Connected";

        deviceInfoOutput?.SetActive(false);
        signalOutput?.SetActive(false);
        resistOutput?.SetActive(false);
        eegOutput?.SetActive(false);
        eegIndexOutput?.SetActive(false);
        spectrumOutput?.SetActive(false);
        emotionsOutput?.SetActive(false);

        if (disconnected)
        {
            channelsController?.destroyBattery();
            devicePower = 0;
        }
        else
        {
            channelsController?.createBattery(device, (power) =>
            {
                devicePower = power;
            });
        }

    }




    private void GetDeviceInfo()
    {

        string info = "";
        info += "*Common params*\n";
        info += string.Format("Name: [{0}]\n", device.ReadParam<string>(Neuro.Native.Parameter.Name));
        //info += string.Format("Name: [{0}]\n", input_params.ip_address);
        info += string.Format("Address: [{0}]\n", device.ReadParam<string>(Neuro.Native.Parameter.Address));
        info += string.Format("Serial Number: [{0}]\n", device.ReadParam<string>(Neuro.Native.Parameter.SerialNumber));

        FirmwareVersion fv = device.ReadParam<FirmwareVersion>(Neuro.Native.Parameter.FirmwareVersion);
        info += string.Format("Version: [{0}.{1}]\n", fv.Version, fv.Build);

        FirmwareMode fm = device.ReadParam<FirmwareMode>(Neuro.Native.Parameter.FirmwareMode);
        info += string.Format("Mode: [{0}]\n", fm.ToString());

        info += "*Supported params*\n";
        foreach (ParamInfo paramInfo in device.Parameters)
        {
            info += string.Format("Name: [{0}] Type: [{1}] Access: [{2}]\n",
                paramInfo.Parameter.ToString(), paramInfo.Parameter.GetType(), paramInfo.Access);
        }

        info += "*Supported device channels*\n";
        foreach (ChannelInfo channelInfo in device.Channels)
        {
            info += string.Format("Name: [{0}] Type: [{1}] Index: [{2}]\n",
                channelInfo.Name, channelInfo.Type, channelInfo.Index);
        }

        info += "*Supported commands*\n";
        foreach (Command command in device.Commands)
        {
            info += string.Format("{0}\n", command);
        }
        deviceInfoText.text = info;

    }


    public void Sending()
    {
        //Password - xgfxsygrzzcenjlv
        //Console.WriteLine("Initializing Mail Client");

        //Test

        var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "THIS IS NOT A DRILL");
        //var tooAddress = new MailAddress("8652366111@txt.att.net", "TAKE COVER IMMEDIATELY");
        var tooAddress = new MailAddress("9313355335@vtext.com", "TAKE COVER IMMEDIATELY");
        const string fromPassword = "xgfxsygrzzcenjlv";
        const string subject = "TESTING_APP_TEXT";
        const string body = "Why Hello There General Kenobi";
        //System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment("C:\\Users\\reese\\OneDrive\\Documents\\KeatonEmail\\Email System\\eeg.png");
        const string host = "smtp.gmail.com";
        const int port = 587;






        sendMail(fromAddress, tooAddress, fromPassword, subject, body, host, port);
    }






    static void sendMail(MailAddress from, MailAddress too, string password, string subject, string body, string host, int port)
    {
        using (var smtp = new SmtpClient(host))
        {
            smtp.Host = host;
            smtp.Port = port;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(from.Address, password);

            using (var message = new MailMessage(from, too))
            {
                message.Subject = subject;
                message.Body = body;
                //message.Attachments.Add(attach);
                smtp.Send(message);
            };
        };
    }
}

