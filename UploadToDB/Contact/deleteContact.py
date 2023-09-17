import requests

import sys
sys.path.append("..")
import login

"""
Delete a Contact by using its id
"""

if len(sys.argv) > 1:
    contact_id = sys.argv[1]
else:
    contact_id = input("Enter the contact id to delete: ")

token = login.get_token()
    
url = f"https://www.nexpo.arkadtlth.se/api/contacts/{contact_id}"

headers = {
    'Authorization': token,
}

r = requests.delete(url, headers=headers)

print(r.status_code)

if r.status_code == 200 or r.status_code == 204:
    print(f"Successfully deleted contact with id {contact_id}.")
else:
    print(f"Failed to delete contact with id {contact_id}. Error: {r.text}")