/*
 * Name: Keaton Shelton
 * Date: November 8th, 2022
 * Arguments: A List<ContactsPackage>
 * Returns: A List<ContactsPackage>
 * 
 * Abstract:
 *      This class is an intermidate class for use with ContactsIO and ContactsPackage.
 *      This class is a Serializable List of ContactsPackages' to make IO easier.
 *      
 * Revisions:
 * 01ks - November 8th, 2022 - Original
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class ContactsList
{
    public List<ContactsPackage> List;
}

