import requests

import sys
sys.path.append("..")
import login

"""
Delete a FAQ by using it id
"""

if len(sys.argv) > 1:
    contact_id = sys.argv[1]
else:
    faq_id = input("Enter the faq id to delete: ")

token = login.get_token()
    
url = f"https://www.nexpo.arkadtlth.se/api/faq/{faq_id}"

headers = {
    'Authorization': token,
}

r = requests.delete(url, headers=headers)

print(r.status_code)

if r.status_code == 200 or r.status_code == 204:
    print(f"Successfully deleted faq with id {faq_id}.")
else:
    print(f"Failed to delete faq with id {faq_id}. Error: {r.text}")