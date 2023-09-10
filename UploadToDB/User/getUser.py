import requests
import json

import sys
sys.path.append("..")
import login

"""
Gets a users information, using their email as input
"""

if len(sys.argv) > 1:
    target_email = sys.argv[1]
else:
    target_email = input("Enter the email to get: ")

auth_token = login.get_token()

url = "https://www.nexpo.arkadtlth.se/api/users" 

headers = {
    'Authorization': auth_token,
    'Content-Type': 'application/json'
}

response = requests.get(url, headers=headers)

if response.status_code == 200:
    users = response.json()
    for user in users:
        if user.get('email') == target_email:
            print(f"User with email {target_email}:")
            print(json.dumps(user, indent=4))
            
else:
    print(f"Failed to get users. Status code: {response.status_code}")
    print(response.text)


