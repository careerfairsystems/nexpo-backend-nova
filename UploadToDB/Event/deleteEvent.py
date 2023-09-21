import requests

import sys
sys.path.append("..")
import login

"""
Delete a Event by using its id
"""

if len(sys.argv) > 1:
    contact_id = sys.argv[1]
else:
    event_id = input("Enter the event id to delete: ")

token = login.get_token()
    
url = f"https://www.nexpo.arkadtlth.se/api/event/{event_id}"

headers = {
    'Authorization': token,
}

r = requests.delete(url, headers=headers)

print(r.status_code)

if r.status_code == 200 or r.status_code == 204:
    print(f"Successfully deleted event with id {event_id}.")
else:
    print(f"Failed to delete event with id {event_id}. Error: {r.text}")