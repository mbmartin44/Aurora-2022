///--------------------------------------------------------------------------------------
/// <file>    UIController.cs                                      </file>
/// <author>  Reese Meadows                                        </author>
/// <author>  Keaton Shelton                                       </author>
/// <author>  Blake Martin                                         </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     This is the UI controller for the game. It handles the UI elements and the
///     functionality of the UI menu, buttons, and nested pages.
/// </summary>
/// -------------------------------------------------------------------------------------
/// <remarks>
///     This script is attached to the UIController object in the scene.
/// </remarks>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Neuro;
using Neuro.Native;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Mail;

public sealed class UIController : MonoBehaviour
{

    #region UI Elements **********************************************************************************************************************

    ChannelsController channelsController = null;
    Device device = null;
    public GameObject modesVariations;
    public GameObject deviceSearchLabel;
    public GameObject Title;

    /// <summary>
    ///The device state UI elements
    /// </summary>
    [Header("== DeviceState UI ==")]
    public Text deviceConnectionState;
    public Text devicePowerState;
    private int devicePower = 0;

    /// <summary>
    /// The device connection state.
    /// </summary>
    [Header("== DeviceInfo UI ==")]
    public GameObject deviceInfoOutput;
    public Text deviceInfoText;


    /// <summary>
    /// These are the Contacts Page UI elements
    /// </summary>
    [Header("== Contacts UI ==")]
    public GameObject contactsOutput;
    public Text O1Resist;
    public Text O2Resist;
    public Text T3Resist;
    public Text T4Resist;
    public float[] mainColor = { 0f, 0.882353f, 0.2035342f, 1f };
    public string theName;
    public string thePhone;
    public string theEmail;
    public GameObject inputField;
    public GameObject inputField2;
    public GameObject inputField3;
    public GameObject inputField4;
    public GameObject textDisplay;

    /// <summary>
    /// These are the EEG UI elements
    /// </summary>
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


    // LLE variables
    private int samplesLLE = 0;
    private double LLEValue = 0;
    private int samplesLength = 0;

    /// <summary>
    /// Enables the menu that allows the user to choose between different screens.
    /// </summary>
    public void ShowMenu(bool enabled)
    {
        modesVariations.SetActive(enabled);
    }

    #endregion

    #region Unity Methods ********************************************************************************************************************
    /// <summary>
    /// Called automatically by UnityEngine when the application starts.
    /// This is a property of MonoBehaviour objects.
    /// </summary>
    private void Awake()
    {
        channelsController = new ChannelsController();
    }

    /// <summary>
    /// This function is called when the Unity game object becomes enabled and active.
    /// </summary>
    private void Start()
    {
        ShowMenu(false);
    }

    /// <summary>
    // This method is called every frame by the UnityEngine.
    /// </summary>
    private void FixedUpdate()
    {
        devicePowerState.text = string.Format("Power: {0}%", devicePower);
        timeUpdate.text = string.Format("{0}", framecount);
        newTextLLE.text = "";
        newTextLLE.text += string.Format("*     LLE: {0:F2} \n", output.LLE);
        newTextLLE.text += string.Format("*     SNR: {0:F2} \n*     ", output.SNR);
        newTextLLE.text += output.txt;
        newTextLLE.color = output.color;
        NetOut.SignalWatch(peopleList, output.detection);

    }

    #endregion

    #region DeviceInfo ******************************************************************************************************************

    /// <summary>
    /// This function is called when the user clicks the "Get Device Info" button.
    /// </summary>
    /// remarks>
    /// This method is NOT currently used in the application.
    /// </remarks>
    public void ShowDeviceInfo()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        deviceInfoOutput.SetActive(true);
        GetDeviceInfo();
    }

    /// <summary>
    /// This function is called when the user exits the "Get Device Info" page.
    /// </summary>
    /// remarks>
    /// This method is NOT currently used in the application.
    /// </remarks>
    public void CloseDeviceInfo()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        deviceInfoOutput.SetActive(false);
    }
    #endregion

    #region Contacts *********************************************************************************************************************

    public List<ContactsPackage> peopleList;

    /// <summary>
    /// This function is called when the user clicks the "Contacts Page" button to enable the contacts page.
    /// </summary>
    public void ShowContacts()
    {
        modesVariations.SetActive(false);
        Title.SetActive(false);
        contactsOutput.SetActive(true);
    }

    /// <summary>
    /// This function is called when the user exits the "Contacts Page" page.
    /// </summary>
    public void CloseContacts()
    {
        modesVariations.SetActive(true);
        Title.SetActive(true);
        contactsOutput.SetActive(false);
    }
    #endregion

    #region EEG ***************************************************************************************************************************************

    /// <summary>
    /// This method displays the EEG data, and is where the data is added to the LLEQueue.
    /// </summary>
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

    /// <summary>
    /// This method is called when the user exits the "EEG Page" page.
    /// Several operational variables are reset to their default values here.
    /// </summary>
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

    #region Device Methods ********************************************************************************************************************

    /// <summary>
    /// This function is called when the device's state changes.
    /// It updates the UI to show the device's connection state, and determines whether the battery should be shown or not.
    /// </summary>
    /// <param name="disconnected">A boolean value that is true if the device has disconnected, and false if it has connected.</param>
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
        //Check for Contacts, if so load them
        if (!disconnected)
        {
            LoadContacts();
        }
    }

    /// <summary>
    /// Get the device info
    /// </summary>
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

    /// <summary>
    /// This function saves the device.
    /// </summary>
    /// <param name="device">The device to be saved.</param>
    public void SaveDevice(Device device)
    {
        this.device = device;
    }

    #endregion

    #region Contacts UI Methods ********************************************************************************************************************

    /// <summary>
    /// This function is called when the user saves a new contact.
    /// </summary>
    public void StoreInfo()
    {
        try
        {
            theName = inputField.GetComponent<Text>().text;
            theEmail = inputField2.GetComponent<Text>().text;
            thePhone = inputField3.GetComponent<Text>().text;

            //Create Contact
            ContactsPackage contact = new ContactsPackage();

            //Determine if email is entered
            if (inputField.GetComponent<Text>().text == "")
            {
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "Enter a name";
                return;
            }
            if (inputField2.GetComponent<Text>().text == "")
            {
                contact.phone = thePhone;
                contact.name = theName;
                contact.address = "";
            }
            if (inputField3.GetComponent<Text>().text == "")
            {
                contact.phone = "";
                contact.name = theName;
                contact.address = theEmail;
            }
            if (inputField2.GetComponent<Text>().text == "" && inputField3.GetComponent<Text>().text == "")
            {
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "Enter a phone number or email";
                return;
            }
            else
            {
                contact.phone = thePhone;
                contact.name = theName;
                contact.address = theEmail;
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
            //Write to File
            SaveContacts();
            //List Contacts
            ListContacts();
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.ToString());
        }
    }

    /// <summary>
    /// This function is used to list all the contacts in the list
    /// and display them in the text display
    /// </summary>
    public void ListContacts()
    {
        try
        {
            string temp = "";
            int i = 1;
            if (peopleList == null)
            {
                //No Contacts to list
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "No Contacts are saved";
                return;
            }
            else
            {
                foreach (var x in peopleList)
                {
                    temp = temp + i.ToString() + ".)  \t\t" + x.name + "\n"
                        + "    \t\t" + x.address + "\n"
                        + "    \t\t" + x.phone + "\n\n";
                    i++;
                }
                textDisplay.GetComponent<Text>().color = Color.green;
                textDisplay.GetComponent<Text>().text = temp;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    /// <summary>
    /// This function takes the list of contacts and writes them to a file.
    /// </summary>
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
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "Enter a real number";
                return;
            }
            if (peopleList == null)
            {
                //Nothing to remove
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "There are no contacts";
                return;
            }
            if (peopleList.Count < i)
            {
                //Invalid index
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "Invalid contact number";
                return;
            }
            //Finally, remove contact
            peopleList.RemoveAt(i - 1);


            //Write to File
            SaveContacts();
            //Print New List
            ListContacts();
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
        }
    }

    /// <summary>
    /// Saves the contacts to a file
    /// </summary>
    /// <param name="peopleList">The people list.</param>
    /// <param name="fileName">Name of the file.</param>
    public void SaveContacts()
    {
        ContactsList tempList = new ContactsList();
        tempList.List = peopleList;
        ContactsIO outFile = new ContactsIO(tempList);
        outFile.SaveContacts();
    }

    /// <summary>
    /// Loads contacts from the saved file
    /// </summary>
    public void LoadContacts()
    {
        ContactsIO inFile = new ContactsIO();
        if (peopleList == null)
        {
            //Initialize
            peopleList = new List<ContactsPackage>();

            //Attempt Load
            ContactsList tempList = new ContactsList();
            tempList = inFile.LoadContacts();
            if (inFile.status == 1)
            {
                //No Contacts Saved
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "No Contacts have been saved before";
                return;
            }
            //Import
            peopleList = tempList.List;

            //Show
            ListContacts();
        }
        else
        {
            //Attempt Load
            ContactsList tempList = new ContactsList();
            tempList = inFile.LoadContacts();
            if (inFile.status == 1)
            {
                //No Contacts Saved
                textDisplay.GetComponent<Text>().color = Color.red;
                textDisplay.GetComponent<Text>().text = "No Contacts have been saved before";
                return;
            }
            //Remove Anything in list
            peopleList.Clear();
            //Import
            peopleList = tempList.List;

            //Show
            ListContacts();
        }
    }

    #endregion

    #region LLE Functions ********************************************************************************************************************

    string[] keys = { "O1", "O2", "T3", "T4" };

    Rosenstein.Output output = new Rosenstein.Output();

    /// <summary>
    /// RunLLE method is used to run the LLE algorithm on the queue
    /// </summary>
    /// <remarks>
    /// The algorithm is run on the data that is stored in the queue and the output is stored in the output variable.
    /// </remarks>
    public void RunLLE()
    {
        //initialize rosenstein object
        Rosenstein rosenstein = new Rosenstein();
        //initialize a buffer of length equal to the sum of the lengths of all 4 queues
        int length = 0;
        bool stop = false;
        foreach (var chann in LLEQueue)
        {
            length += chann.Value.Count;
        }
        double[] tempBuff = new double[length];
        //j is used to keep track of which queue we are reading from
        int j = 0, empty = 0;
        //lock the queue to make sure the data is not being modified while we are reading from it
        lock (LLEQueue)
        {
            //iterate through the whole buffer
            for (int i = 0; i < length; ++i)
            {
                //if the current queue has data, add it to the buffer
                if (LLEQueue[keys[j % 4]].Count > 0)
                {
                    tempBuff[i] = LLEQueue[keys[j % 4]].Dequeue();
                    samplesLength--;
                    j++;
                }
                //if the current queue is empty, look for the next queue that has data
                else
                {
                    //while the current queue is empty and we have not found a queue with data, look for the next queue
                    while (LLEQueue[keys[j % 4]].Count == 0 && !stop)
                    {
                        empty++;
                        j++;
                        //if we have looked at all queues and none have data, stop looking and break out of the for loop
                        if (empty == 4)
                        {
                            stop = true;
                            break;
                        }
                    }
                }
            }
            //set the buffer to the rosenstein object
            rosenstein.SetData1D(tempBuff);
            //run the rosenstein algorithm
            output = (Rosenstein.Output)rosenstein.RunAlgorithm();
        }
        //set the running variable to false
        ThreadSafeSetBool(false);
    }

    #endregion

    #region Thread Synchronization Methods ********************************************************************************************************************

    // synchObject is used to synchronize access to the EEG data between the LLE algorithm and showEEG data method where
    // it is added to the LLEQueue.
    object syncObj = new object();
    bool runLLE = false;

    /// <summary>
    /// Toggles the runLLE boolean value.
    /// </summary>
    void ThreadSafeToggleBool()
    {
        lock (syncObj)
        {
            runLLE = !runLLE;
        }
    }

    /// <summary>
    /// Returns the current state of the runLLE variable.
    /// </summary>
    /// <returns>runLLE</returns>
    bool ThreadSafeReadBool()
    {
        lock (syncObj)
        {
            return runLLE;
        }
    }

    /// <summary>
    /// This function sets the value of a boolean variable in a thread safe manner.
    /// </summary>
    /// <param name="val">The value to be set</param>
    void ThreadSafeSetBool(bool val)
    {
        lock (syncObj)
        {
            runLLE = val;
        }
        return;
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

    #endregion

}