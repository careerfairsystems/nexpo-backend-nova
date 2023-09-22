import json
import requests
import sys
sys.path.append("..")
import login

api_endpoint = "https://nexpo.arkadtlth.se/api/SendManyTicketsToMailAsync"

auth_token = login.get_token()

jsonfile = 'sendMail.json'

headers = {
    "Authorization": auth_token, 
    "Content-Type": "application/json"
}

def send_tickets_to_email(email, ticket_info):
    payload = {
        "email": email,
        "tickets": ticket_info
    }

    response = requests.post(api_endpoint, json=payload, headers=headers)

    if response.status_code == 200:
        print("Tickets sent successfully.")
    else:
        print(f"Failed to send tickets. Status code: {response.status_code}")
        print(response.text)

with open(jsonfile, encoding="utf-8") as d:
    data = json.load(d)

    for entry in data:
        email = entry['mail']
        ticket_info = {
            "lunch_tickets_day1": entry['lunch_tickets_day1'],
            "lunch_tickets_day2": entry['lunch_tickets_day2'],
            "banquet_tickets": entry['banquet_tickets']
        }
        send_tickets_to_email(email, ticket_info)
