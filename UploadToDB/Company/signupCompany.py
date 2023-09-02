import requests
import json

import sys
sys.path.append("..")
import login

"""
Create a company representative login, using JSON
"""

auth_token = login.get_token()

url = "https://www.nexpo.arkadtlth.se/api/signup/representative"

headers = {
    'Authorization': auth_token,
    'Content-Type': 'application/json'
}

data_to_send = {
    "email": "backend.arkad@box.tlth.se",
    "firstname": "Backend",
    "lastname": "Arkad",
    "companyid": 180
}

response = requests.post(url, headers=headers, json=data_to_send)

if response.status_code == 204:
    print(f"Successfully invited representative.")
else:
    print(f"Failed to invite representative. Status code: {response.status_code}")
    print(response.text)

