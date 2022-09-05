/*
 * Author: Keaton Shelton
 * Date: August 25th, 2022
 * Arguments: none
 * Returns: n/a
 * 
 * Abstract:
 *      This is the main testing program for the various
 *  different subfunctions / routines that will be deployed
 *  in the final detection app.
 *  
 *  Revisions:
 *  01ks - ? - Original
 *  02ks - August 25th, 2022 - Update Structure
 *  03ks - August 25th, 2022 - Add in texting test function
 *  04ks - August 29th, 2022 - Add in MMS test function
 *  05ks - September 5th, 2022 - Revamp in progress of Test Program
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Email_Client;
using Text_Client;
using Contacts;

namespace Network_Dev
{
    class NetworkTest
    {
        static void Main()
        {
            try
            {
                bool run = true;
                Console.WriteLine("Initializing Client");
                //Required Initialization
                var fromAddress = new MailAddress("brainanalytics2022@gmail.com", "Auto Sender");
                var tooAddress = new MailAddress("keaton.shelton2@gmail.com", "Keaton Shelton");
                const string fromPassword = "xgfxsygrzzcenjlv";

                while (run)
                {
                    jmp:
                    //Attachment Examples below (image type)
                    //System.Net.Mail.Attachment attach1 = new System.Net.Mail.Attachment("C:\\Users\\kmshelton\\Downloads\\image004.png");
                    //Attachment attach = new Attachment(".\\eeg.png");
                    string number, answer, subject, body;
                    bool isNum = false;
                    const string host = "smtp.gmail.com";
                    const int port = 587;
                    int select;
                    List<ContactsPackage> people = new List<ContactsPackage>();

                    //Menu
                    Console.WriteLine("");
                    Console.WriteLine("Menu Selection:");
                    Console.WriteLine("1. Add Contact");
                    Console.WriteLine("2. List Contacts");
                    Console.WriteLine("3. Remove Contacts");
                    Console.WriteLine("4. Write Subject");
                    Console.WriteLine("5. List Subject");
                    Console.WriteLine("6. Write Body");
                    Console.WriteLine("7. List Body");
                    Console.WriteLine("8. Send E-Mail");
                    Console.WriteLine("9. Send Text Message");
                    Console.WriteLine("0. Exit");

                    //Take User Input
                    answer = Console.ReadLine();
                    isNum = int.TryParse(answer, out select);
                    //Error Check
                    if(!isNum) {
                        Console.WriteLine("Bad Selection, Resetting Menu");
                        Console.WriteLine("");
                        goto jmp;
                    }

                    //Menu Logic
                    switch(select) {
                        case 1:
                            string name, email, phone, choice;
                            int selector;
                            bool numVer = false;
                            ContactsPackage newContact = new ContactsPackage ();
                            Console.WriteLine("What is the name of this person?:");
                            name = Console.ReadLine();
                            Console.WriteLine("What is " + name + "'s E-Mail Address?:");
                            email = Console.ReadLine();
                            newContact.nameAddress = new MailAddress(email, name);
                            Console.WriteLine("What is the phone number of " + name + "?");
                            phone = Console.ReadLine();
                            newContact.phone = phone;
                            Console.WriteLine("Finally, what is the users carrier ID from this list?");
                            Console.WriteLine("1. T-Mobile");
                            Console.WriteLine("2. AT&T");
                            Console.WriteLine("3. Verizon");
                            Console.WriteLine("4. Virgin");
                            Console.WriteLine("5. Boost");
                            Console.WriteLine("6. Xfinity");
                            Console.WriteLine("7. Sprint");
                            Console.WriteLine("8. U.S. Cellular");
                            Console.WriteLine("9. Cricket");
                            Console.WriteLine("10. Tracfone");
                            Console.WriteLine("11. Metro");
                            Console.WriteLine("12. Ting");
                            Console.WriteLine("0. Not Listed / No Phone");
                            Console.WriteLine("(If user does not have a phone please hit option 0)");
                            choice = Console.ReadLine();
                            numVer = int.TryParse(choice, out selector);
                            newContact.carrierID = selector;
                            break;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }
    }
}
