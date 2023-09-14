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
    

with open(jsonfile, encoding="utf-8") as d:
    try:
        dictData = json.load(d)

        for entry in d:
            email = entry['tickets']['email']
            ticket_info = {
                "lunch_tickets_day1": entry['tickets']['lunch_tickets_day1'],
                "lunch_tickets_day2": entry['tickets']['lunch_tickets_day2'],
                "banquet_tickets": entry['tickets']['banquet_tickets']
            }
            send_tickets_to_email(email, ticket_info)
    except json.JSONDecodeError as e:
        print(f"Error decoding JSON: {e}")
    except Exception as e:
        print(f"An error occurred: {e}")