import requests
import json

import sys
sys.path.append("..")
import login

"""
Invite volunteers to the Arkad App
**Needs to be tested in production**
"""

auth_token = login.get_token()

# Hardcoded JSON file path
jsonfile = '../jsonTemplate/InviteVolunteers.json'

url = "https://www.nexpo.arkadtlth.se/api/session/volunteer"

with open(jsonfile, 'r') as json_file:
    data = json.load(json_file)

emails = data.get('emails', [])

headers = {
    'Authorization': auth_token,
    'Content-Type': 'application/json'
}

for email in emails:
    payload = {
        'Email': email,
        'FirstName': 'Name',  # Change to Firstname if known
        'LastName': 'Name'    # Change to Lastname if known
    }

    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 204:
        print(f"Invitation sent to {email}. Status Code: {response.status_code}")
    elif response.status_code == 409:
        print(f"Conflict for {email}: User already exists. Status Code: {response.status_code}")
    else:
        print(f"Unexpected error for {email}. Status Code: {response.status_code}")
