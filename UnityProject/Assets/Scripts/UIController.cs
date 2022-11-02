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
    public Text O1Resist;
    private double rawO1Resist = 0;
    public Text O2Resist;
    private double rawO2Resist = 0;
    public Text T3Resist;
    private double rawT3Resist = 0;
    public Text T4Resist;
    private double rawT4Resist = 0;

    public string theName;
    public string thePhone;
    public string theEmail;
    public GameObject inputField;
    public GameObject inputField2;
    public GameObject inputField3;
    public GameObject inputField4;
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

    private double[] LLEBuffer;
    private int sampleCountLLE = 0;
    private int LLEWindowSize = 250;
    private double LLEValue = 0;
    private bool[] incrementIndex = { false, false, false, false };
    private int samplesPlotted = 0;
    bool initCmd = true;
    bool lockBool = false;
    public void ShowEEG()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        eegOutput.SetActive(true);
        update = true;
        timeUpdating();

        
        
        //newTextLLE.text = string.Format("LLE: {0:F2} ", );

        channelsController.createEeg(device, (channel, samples) =>
        {
            //timeUpdate.text = string.Format("{0}", samples.Length);
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
            if (LLEplaceholder > 0)
            {
                Seizure.SetActive(true);
                Seizure_Notdet.SetActive(false);
            }
            else
            {
                Seizure.SetActive(false);
                Seizure_Notdet.SetActive(true);
            }


        });
        
        eegO1Graph.InitGraph(channelsController.GetEegPlotSize());
        eegO2Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT3Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT4Graph.InitGraph(channelsController.GetEegPlotSize());

    }

    public void CloseEEG()
    {
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
        LLEplaceholder = 0;


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
        theName = inputField.GetComponent<Text>().text;
        theEmail = inputField2.GetComponent<Text>().text;
        thePhone = inputField3.GetComponent<Text>().text;

        //Create Contact
        ContactsPackage contact = new ContactsPackage();

        //Determine if email is entered, must check as MailAddress will crap out unity if invalid
        if (inputField2.GetComponent<Text>().text == "")
        {
            contact.phone = thePhone;
            contact.nameAddress = new MailAddress("null@hotmail.com", theName);
        }
        else if (inputField3.GetComponent<Text>().text == "")
        {
            contact.phone = "";
            contact.nameAddress = new MailAddress(theEmail, theName);
        }
        else
        {
            contact.phone = thePhone;
            contact.nameAddress = new MailAddress(theEmail, theName);
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

        textDisplay.GetComponent<Text>().text = "Entered: \nName: " + contact.nameAddress.DisplayName + "\nEmail Address: " + contact.nameAddress.Address
            + "\nPhone Number: " + contact.phone;
    }

    public void ListContacts()
    {
        try
        {
            string temp = "";
            int i = 1;
            if (peopleList == null)
            {
                //No Contacts to list
                return;
            }
            else
            {
                foreach (var x in peopleList)
                {
                    temp = temp + i.ToString() + ". Name: " + x.nameAddress.DisplayName + "\n"
                        + "Email Address: " + x.nameAddress.Address + "\n"
                        + "Phone Number: " + x.phone + "\n";
                    i++;
                }
                textDisplay.GetComponent<Text>().text = temp;
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    public void RemoveContact()
    {
        try
        {
            string index = inputField4.GetComponent<Text>().text;
            int i = 0;

            //BREAK CONDITIONS
            if (!int.TryParse(index, out i))
            {
                //Not an int
                textDisplay.GetComponent<Text>().text = "Enter a real number";
                return;
            }
            if (peopleList == null)
            {
                //Nothing to remove
                textDisplay.GetComponent<Text>().text = "There are no contacts";
                return;
            }
            if(peopleList.Count < i)
            {
                //Invalid index
                textDisplay.GetComponent<Text>().text = "Invalid contact number";
                return;
            }
            //Finally, remove contact
            peopleList.RemoveAt(i - 1);
            textDisplay.GetComponent<Text>().text = "Conact Number: " + (i).ToString() + " has been removed";
        }
        catch(Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }



    public async void timeUpdating()
    {
        if (update == true)
        {
            for (int i = 1; i < 21600; i++)
            {
                timeUpdate.text = string.Format("{0}", i);
                await System.Threading.Tasks.Task.Delay(2000);
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







