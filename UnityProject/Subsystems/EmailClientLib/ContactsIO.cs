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
    public ContactsList tempList;
    public string dirPath;
    public string json;
    
    public ContactsIO()
    {
        //Dont Use unless loading in Contacts
        this.tempList = new ContactsList();
        this.dirPath = Path.Combine(Application.persistentDataPath, "Contacts", "contacts.json");
    }
    public ContactsIO(ContactsList input)
    {
        this.tempList = new ContactsList();
        this.tempList = input;
        this.dirPath = Path.Combine(Application.persistentDataPath, "Contacts", "contacts.json");
    }

    public void SaveContacts()
    {

        json = JsonUtility.ToJson(tempList);
        File.WriteAllText(dirPath, json);
        Debug.Log("Contacts Written to contacts.json");
    }

    public ContactsList LoadContacts()
    {
        string tempString = File.ReadAllText(dirPath);
        tempList = JsonUtility.FromJson<ContactsList>(tempString);
        return tempList;
    }

}
