///--------------------------------------------------------------------------------------
/// <file>    ContactsList.cs                                      </file>
/// <author>  Keaton Shelton                                       </author>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     This class is an intermidate class for use with ContactsIO and ContactsPackage.
///     This class is a Serializable List of ContactsPackages' to make IO easier.
/// </summary>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class is a Serializable List of ContactsPackages' to make IO easier.
/// </summary>
[System.Serializable]
public class ContactsList
{
    public List<ContactsPackage> List;
}

