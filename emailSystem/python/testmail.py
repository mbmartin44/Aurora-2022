from __future__ import print_function
from email.mime.audio import MIMEAudio
from email.mime.base import MIMEBase
from email.mime.multipart import MIMEMultipart
from fileinput import filename

from email.mime.text import MIMEText
from email import encoders
import base64
import mimetypes

import os

import os.path

from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError

# If modifying these scopes, delete the file token.json. This sets the Scope of what the email system can access
SCOPES = ['https://www.googleapis.com/auth/gmail.send']

#Global Variables

#DONT CHANGE fromEMAIL
fromEmail = "brainanalytics2022@gmail.com"
testTo = "keaton.shelton2@gmail.com"
testSubject = "Epileptic Event"
TestBody = "Their has been a reported event"



#Create Message
def create_message(sender, to, subject, message_text):
  """Create a message for an email.

  Args:
    sender: Email address of the sender.
    to: Email address of the receiver.
    subject: The subject of the email message.
    message_text: The text of the email message.

  Returns:
    An object containing a base64url encoded email object.
  """
  #Message object that holds Sender, To, Subject, and Body of message
  message = MIMEText(message_text)
  message['to'] = to
  message['from'] = sender
  message['subject'] = subject
  raw_message = base64.urlsafe_b64encode(message.as_string().encode("utf-8")) #Encode message to be accepted by Google API
  return {'raw': raw_message.decode("utf-8")} #Return base64 shifted data of all previously mentioned message contents

def create_message_with_attachment(sender, to, subject, message_text, file):
  message = MIMEMultipart()
  message['to'] = to
  message['from'] = sender
  message['subject'] = subject

  msg = MIMEText(message_text)
  message.attach(msg)

  content_type, encoding = mimetypes.guess_type(file)
  main_type, sub_type = content_type.split('/',1)

  file_name = os.path.basename(file)

  f = open(file, 'rb')

  myFile = MIMEBase(main_type, sub_type)
  myFile.set_payload(f.read())
  myFile.add_header('Content-Disposition', 'attachment', filename=file_name)
  encoders.encode_base64(myFile)
  f.close()

  message.attach(myFile)
 

  raw_message = base64.urlsafe_b64encode(message.as_string().encode("utf-8"))
  return {'raw': raw_message.decode("utf-8")}


#Send Message
def send_message(service, user_id, message):
  """Send an email message.

  Args:
    service: Authorized Gmail API service instance.
    user_id: User's email address. The special value "me"
    can be used to indicate the authenticated user.
    message: Message to be sent.

  Returns:
    Sent Message.
  """
  #Send Message Main Function
  try:
    message = service.users().messages().send(userId=user_id, body=message).execute() #Google API call to send Email
    print('Message Id: %s' % message['id']) #Print message ID
    return message
  except Exception as e:
    print('An error occurred: %s' % e) #Print Error if Fail
    return None


def main():
    """Shows basic usage of the Gmail API.
    Lists the user's Gmail labels.
    Now Sends Emails, this took way too long to get working :)
    """
    creds = None
    # The file token.json stores the user's access and refresh tokens, and is
    # created automatically when the authorization flow completes for the first
    # time.
    if os.path.exists('token.json'):
        creds = Credentials.from_authorized_user_file('token.json', SCOPES)
    # If there are no (valid) credentials available, let the user log in.
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            here = os.path.dirname(os.path.abspath(__file__)) #Find path of credentials.json
            fileN = os.path.join(here, 'credentials.json')
            flow = InstalledAppFlow.from_client_secrets_file(fileN, SCOPES) #Open credentials.json and create intermediate flow
            creds = flow.run_local_server(port=0) #Create creds (internal token.json)
        # Save the credentials for the next run
        with open('token.json', 'w') as token:
            token.write(creds.to_json()) #Write token.json to PC

    try:
        # Call the Gmail API and build active Service
        service = build('gmail', 'v1', credentials=creds)

        #Given Google API Test
        #results = service.users().labels().list(userId='me').execute()
        #labels = results.get('labels', [])
        #if not labels:
            #print('No labels found.')
            #return
        #print('Labels:')
        #for label in labels:
            #print(label['name'])


        img_data = r'.\eeg.png'

        #Send Message with picture
        messageBody = create_message_with_attachment('me',testTo,testSubject,TestBody, img_data) #Create a message object
        #send message with no picture
        #messageBody = create_message(fromEmail,testTo,testSubject,TestBody)
        send_message(service,"me",messageBody) #Call Google API to send message object

    except HttpError as error:
        # cTODO(developer) - Handle errors from gmail API.
        print(f'An error occurred: {error}')

#Call Main function
if __name__ == '__main__':
    main()