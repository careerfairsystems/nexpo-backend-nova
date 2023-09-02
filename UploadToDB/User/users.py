import requests
import json

import sys
sys.path.append("..")
import login

"""
Updates a User, using their id as input

If you do not know a users id, only their email,
it is reccomended to use the getUser.py script.
"""

if len(sys.argv) > 1:
    user_id = sys.argv[1]
else:
    user_id = input("Enter the id of the user to update: ")

auth_token = login.get_token()

url = f"https://www.nexpo.arkadtlth.se/api/users/{user_id}"

headers = {
    'Authorization': auth_token,
    'Content-Type': 'application/json'
}

fields_to_update = {
    "phonenr": "111111111"  
}

response = requests.put(url, headers=headers, json=fields_to_update)

if response.status_code == 200:
    print(f"Successfully updated user with ID {user_id}")
    print(json.dumps(response.json(), indent=4)) 
else:
    print(f"Failed to update user. Status code: {response.status_code}")
    print(response.text)

