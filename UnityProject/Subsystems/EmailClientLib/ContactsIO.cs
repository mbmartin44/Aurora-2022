///--------------------------------------------------------------------------------------
/// <file>    ContactsIO.cs                                        </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///  This is the IO class for the Contacts list. This
///  class will serve to save and read in contacts from a
///  specified json file.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class is used to read and write contacts to a json file.
/// </summary>
public class ContactsIO : MonoBehaviour
{
    private ContactsList tempList;
    public string filePath;
    public string dirPath;
    public string json;
    public int status;

    /// <summary>
    /// Loads in the list of contacts from the file
    /// </summary>
    /// <returns></returns>
    public ContactsIO()
    {
        //Dont Use unless loading in Contacts
        this.tempList = new ContactsList();
        this.filePath = Path.Combine(Application.persistentDataPath, "Contacts", "contacts.json");
        this.dirPath = Path.Combine(Application.persistentDataPath, "Contacts");
        this.status = 0;
    }


    /// <summary>
    /// This is the constructor for the ContactsIO class. It takes a ContactsList object as an argument,
    /// and sets the attributes of the class to reflect the data in the ContactsList object.
    /// </summary>
    /// <param name="input">The ContactsList object that is used to initialize the class</param>
    public ContactsIO(ContactsList input)
    {
        this.tempList = new ContactsList();
        this.tempList = input;
        this.json = JsonUtility.ToJson(this.tempList);
        this.filePath = Path.Combine(Application.persistentDataPath, "Contacts", "contacts.json");
        this.dirPath = Path.Combine(Application.persistentDataPath, "Contacts");
        this.status = 0;
    }

    /// <summary>
    /// Code for saving contacts in json format. It checks if the directory exists, if not it creates the directory.
    /// It then checks if the file exists, if not it creates the file. It then converts the list to json format and writes the json to the file.
    /// If there is any exception, the status is set to -1 which is an error.
    /// </summary>
    public void SaveContacts()
    {
        try
        {
            //Check Directory
            if (!Directory.Exists(dirPath))
            {
                //Create Directory
                DirectoryInfo folder = Directory.CreateDirectory(dirPath);
            }
            //Check File
            if (!File.Exists(filePath))
            {
                //Create File
                FileStream file = File.Create(filePath);
            }
            string json = JsonUtility.ToJson(tempList);
            //Write to File
            File.WriteAllText(filePath, json);
            Debug.Log("Contacts Written to contacts.json");
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.ToString());
            this.status = -1;
        }
    }

    /// <summary>
    /// Code for loading contacts from a json file. It checks if the directory exists, if not it creates the directory.
    /// It then checks if the file exists, if not it creates the file. It then reads the json from the file and converts it to a list.
    /// If there is any exception, the status is set to -1 which is an error.
    /// </summary>
    public ContactsList LoadContacts()
    {
        try
        {
            //Check Directory
            if (!Directory.Exists(dirPath))
            {
                //No Directory
                this.status = 1;
                return tempList;
            }
            //Check File
            if (!File.Exists(filePath))
            {
                //No File
                this.status = 1;
                return tempList;
            }
            //Read from file
            string tempString = File.ReadAllText(filePath);
            tempList = JsonUtility.FromJson<ContactsList>(tempString);
            return tempList;
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.ToString());
            this.status = -1;
            return null;
        }
    }
}
