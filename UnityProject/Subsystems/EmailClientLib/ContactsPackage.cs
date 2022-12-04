///--------------------------------------------------------------------------------------
/// <file>    ContactsPackage.cs                                   </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     This class contains the contacts class
///     that are needed for the email / text client to work.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

/// <summary>
/// This class contains the contacts class
/// that are needed for the email / text client to work.
/// </summary>
[System.Serializable]
public class ContactsPackage
{
    public string name;
    public string phone;
    public string address;

    /// <summary>
    /// This is the default constructor for the ContactsPackage class.
    /// </summary>
    /// <remarks>
    /// This constructor initializes the phone, name, and address fields
    /// with default values.
    /// </remarks>
    public ContactsPackage()
    {
        //02ks
        this.phone = "";
        this.name = "Default";
        this.address = "";
    }

    /// <summary>
    /// This is a constructor for the ContactsPackage class. It takes in a name, phone, and address and assigns them to the appropriate variables.
    /// </summary>
    /// <param name="name">The name of the contact</param>
    /// <param name="phone">The phone number of the contact</param>
    /// <param name="address">The address of the contact</param>
    public ContactsPackage(string name, string phone, string address)
    {
        this.phone = phone;
        this.name = name;
        this.address = address;
    }
}

