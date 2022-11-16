/*
 * Name: Keaton Shelton
 * Date: November 8th, 2022
 * Arguments: A ContactList
 * Returns: A ContactList
 * 
 * Abstract:
 *      This is the IO class for the Contacts list. This
 *      class will serve to save and read in contacts from a 
 *      specified json file.
 * 
 * Revisions:
 * 01ks - November 8th, 2022 - Original
 * 02ks - November 16th, 2022 - Fix issues / Final Revision
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ContactsIO : MonoBehaviour
{
    private ContactsList tempList;
    public string filePath;
    public string dirPath;
    public string json;
    public int status;
    
    public ContactsIO()
    {
        //Dont Use unless loading in Contacts
        this.tempList = new ContactsList();
        this.filePath = Path.Combine(Application.persistentDataPath, "Contacts", "contacts.json");
        this.dirPath = Path.Combine(Application.persistentDataPath, "Contacts");
        this.status = 0;
    }
    public ContactsIO(ContactsList input)
    {
        this.tempList = new ContactsList();
        this.tempList = input;
        this.json = JsonUtility.ToJson(this.tempList);
        this.filePath = Path.Combine(Application.persistentDataPath, "Contacts", "contacts.json");
        this.dirPath = Path.Combine(Application.persistentDataPath, "Contacts");
        this.status = 0;
    }

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
        catch(Exception e)
        {
            Debug.Log("Error: " + e.ToString());
            this.status = -1;
        }
    }

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
        catch(Exception e)
        {
            Debug.LogError("Error: " + e.ToString());
            this.status = -1;
            return null;
        }
    }
}
