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
 */
using System;
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
                while (run)
                {
                    string number, answer;
                    const string fromPassword = "xgfxsygrzzcenjlv";
                    string subject;
                    string body;
                    //Attachment Examples below (image type)
                    //System.Net.Mail.Attachment attach1 = new System.Net.Mail.Attachment("C:\\Users\\kmshelton\\Downloads\\image004.png");
                    //Attachment attach = new Attachment(".\\eeg.png");
                    const string host = "smtp.gmail.com";
                    const int port = 587;

                    Console.WriteLine("");
                    Console.WriteLine("Menu Selection:");
                    Console.WriteLine("1. Add Contact");
                    Console.WriteLine("2. List Contacts");
                    Console.WriteLine("3. Write Subject");
                    Console.WriteLine("4. List Subject");
                    Console.WriteLine("5. Write Body");

                    answer = Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }
    }
}
