using System;
using System.Collections;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;



public sealed class UIController : MonoBehaviour
{

    ChannelsController channelsController = null;
    Device device = null;
    public List<ContactsPackage> peopleList { get; set; }

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


    [Header("== Contacts UI ==")]
    public GameObject contactsOutput;
    public string thePhone;
    public string theEmail;
    public GameObject inputField;
    public GameObject inputField2;
    public GameObject textDisplay;

    //private InputField InputField;

    [Header("== EEG UI ==")]
    public GameObject eegOutput;
    public Graph eegO1Graph;
    public Graph eegO2Graph;
    public Graph eegT3Graph;
    public Graph eegT4Graph;
    public Text timeUpdate;
    public bool update;
    public Text newTextLLE;
    public int framecount;
    public GameObject Seizure;
    public GameObject Seizure_Notdet;
    public int LLEplaceholder = 0;

    public bool test = false;

    public double[] qual;

    public Queue<double> LLEQueue = new Queue<double>();

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

        LLEplaceholder = 1;



        //DSP Block
        bool detect = true;
        //DSP Block


        //Send Messages
        NetOut.SignalWatch(peopleList, detect);


        GetDeviceInfo();
    }

    public void CloseDeviceInfo()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        deviceInfoOutput.SetActive(false);
    }
    #endregion

    #region Contacts
    public void ShowContacts()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        contactsOutput.SetActive(true);

    }

    public void CloseResistance()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        contactsOutput.SetActive(false);







    }
    #endregion

    #region EEG

    int samplesLLE = 0;
    double LLEValue = 0;
    public void ShowEEG()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        eegOutput.SetActive(true);
        update = true;
        timeUpdating();
        Seizure_Notdet.SetActive(true);
        Seizure.SetActive(false);


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

            for(int i = 1; i < samples.Length; i++)
            {
                LLEQueue.Enqueue(samples[i]);
            }
            //qual = samples;
            if(test == true)
            {
                RunLLE();
            }




        });

        eegO1Graph.InitGraph(channelsController.GetEegPlotSize());
        eegO2Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT3Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT4Graph.InitGraph(channelsController.GetEegPlotSize());

    }

    public void CloseEEG()
    {
        test = false;
        framecount = 0;
        modesVariations.SetActive(true);
        Title.SetActive(true);
        eegOutput.SetActive(false);
        channelsController.destroyEeg(device);
        eegO1Graph.Close();
        eegO2Graph.Close();
        eegT3Graph.Close();
        eegT4Graph.Close();
        update = false;
        Seizure.SetActive(false);
        Seizure_Notdet.SetActive(false);
        //test = false;


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
        eegOutput?.SetActive(false);



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

    public void StoreInfo()
    {
        thePhone = inputField.GetComponent<Text>().text;
        theEmail = inputField2.GetComponent<Text>().text;
        textDisplay.GetComponent<Text>().text = "Welcome " + thePhone + ", " + theEmail;

        //Create Contact
        ContactsPackage contact = new ContactsPackage();

        //Determine if email is entered, must check as MailAddress will crap out unity if invalid
        if (inputField2.GetComponent<Text>().text == "")
        {
            contact.phone = thePhone;
            contact.nameAddress = new MailAddress("null@hotmail.net", "Auto Sender");
        }
        else
        {
            contact.phone = thePhone;
            contact.nameAddress = new MailAddress(theEmail, "Auto Sender");
        }

        //See if Contacts list valid
        if (peopleList == null)
        {
            peopleList = new List<ContactsPackage>();
            peopleList.Add(contact);
        }
        else
        {
            peopleList.Add(contact);
        }
    }

    public async void RunLLE()
    {
        Rosenstein rosenstein = new Rosenstein();
        rosenstein.SetData1D(LLEQueue.ToArray());
        LLEValue = rosenstein.RunAlgorithm();

        newTextLLE.text = string.Format("LLE: {0:F2} ", LLEValue);

        Seizure_Notdet.SetActive(false);
        Seizure.SetActive(true);
        test = false;
        await System.Threading.Tasks.Task.Delay(5000);
        //test = false;
        //Array.Clear(qual, 0, qual.Length);
        
    }


    //43200 is 12 hours in seconds. since update every 2 seconds change to 21600
    public async void timeUpdating()
    {
        if (update == true)
        {
            for (int i = 1; i < 21600; i++)
            {
                timeUpdate.text = string.Format("{0}", i);
                await System.Threading.Tasks.Task.Delay(2000);
                if(i % 5 == 0)
                {
                    test = true;
                    
                }
                else
                {
                    test = false;
                }
                if (!update)
                {
                    return;
                }
            }

        }
        else
        {
            return;
        }



    }
}







