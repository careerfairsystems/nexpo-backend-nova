import json
import requests
import sys
sys.path.append("..")
import login

api_endpoint = "https://nexpo.arkadtlth.se/api/Tickets/send"

auth_token = login.get_token()

jsonfile = 'sendMail.json'

headers = {
    "Authorization": auth_token, 
    "Content-Type": "application/json"
}

def send(mail, eventID, numberOfTickets):
    payload = {
        "mail": mail,
        "eventid": eventID,
        "numberOfTickets": numberOfTickets
        
    }
    
    response = requests.post(api_endpoint, json=payload, headers=headers)
    
    if response.status_code == 200:
        print("Tickets sent successfully.")
    else:
        print(f"Failed to send tickets. Status code: {response.status_code}")
        print(response.content)


    

with open(jsonfile, encoding="utf-8") as d:
    data = json.load(d)

    for entry in data:
        info = entry['tickets']
        
        send(info['mail'], info['eventid'], info['numberOfTickets'])
        #send(info['mail'], 22, info['lunch_tickets_day2'])
        #send(info['mail'], 22, info['banquet_tickets'])

        
