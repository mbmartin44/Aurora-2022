using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;
using UnityEngine.UI;
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

    public bool test = false;

    public double[] qual;

    public Dictionary<string, Queue<double>> LLEQueue = new Dictionary<string, Queue<double>>();

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
        timeUpdate.text = string.Format("{0}", framecount);
        newTextLLE.text = string.Format("LLE: {0:F2} ", LLEValue);
        //Seizure_Notdet.SetActive(true);
        //Seizure.SetActive(false);
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

        //Send Messages
        //NetOut.SignalWatch(peopleList, detect);
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
    // ****************************** MULTITHREAD SYNCHING
    object syncObj = new object();
    bool runLLE = false;
    void ThreadSafeToggleBool()
    {
        lock (syncObj)
        {
            runLLE = !runLLE;
        }
    }
    bool ThreadSafeReadBool()
    {
        lock (syncObj)
        {
            return runLLE;
        }
    }

    void ThreadSafeSetBool(bool val)
    {
        lock (syncObj)
        {
            runLLE = val;
        }
        return;
    }

    //***************************************************

    int samplesLLE = 0;
    double LLEValue = 0;
    int samplesLength = 0;
    public void ShowEEG()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        eegOutput.SetActive(true);
        update = true;
        TimeUpdating();

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
            lock (LLEQueue)
            {
                samplesLength += samples.Length;
                if (!LLEQueue.ContainsKey(anyChannel.Info.Name))
                {
                    Queue<double> tempQue = new Queue<double>();
                    for (int i = 0; i < samples.Length; i++)
                    {
                        tempQue.Enqueue(samples[i]);
                    }
                    LLEQueue.Add(anyChannel.Info.Name, tempQue);
                }
                else
                {
                    for (int i = 0; i < samples.Length; i++)
                    {
                        LLEQueue[anyChannel.Info.Name].Enqueue(samples[i]);
                    }
                    if (runLLE)
                    {
                        RunLLE();
                    }
                }

            }

        });
        eegO1Graph.InitGraph(channelsController.GetEegPlotSize());
        eegO2Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT3Graph.InitGraph(channelsController.GetEegPlotSize());
        eegT4Graph.InitGraph(channelsController.GetEegPlotSize());

    }

    public void CloseEEG()
    {
        ThreadSafeSetBool(false);
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
    string[] keys = { "O1", "O2", "T3", "T4" };
    public  void RunLLE()
    {
        Rosenstein rosenstein = new Rosenstein();
        int length = 0;
        bool stop = false;
        foreach (var chann in LLEQueue)
        {
            length += chann.Value.Count;
        }
        double[] tempBuff = new double[length];
        int j = 0, empty = 0;
        lock (LLEQueue)
        {
            for (int i = 0; i < length; ++i)
            {
                if (LLEQueue[keys[j % 4]].Count > 0)
                {
                    tempBuff[i] = LLEQueue[keys[j % 4]].Dequeue();
                    samplesLength--;
                    j++;
                }
                else
                {
                    while (LLEQueue[keys[j % 4]].Count == 0 && !stop)
                    {
                        empty++;
                        j++;
                        if (empty == 4)
                        {
                            stop = true;
                            break;
                        }
                    }
                }
            }
            rosenstein.SetData1D(tempBuff);
            LLEValue = rosenstein.RunAlgorithm();
        }
        ThreadSafeSetBool(false);
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



    public async void TimeUpdating()
    {
        if (update == true)
        {
            for (framecount = 1; framecount < 21600; framecount++)
            {
                await System.Threading.Tasks.Task.Delay(2000);
                if (LLEQueue["O1"].Count > 1250 && LLEQueue["O2"].Count > 1250 && LLEQueue["T3"].Count > 1250 && LLEQueue["T4"].Count > 1250)
                {
                    ThreadSafeSetBool(true);
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







