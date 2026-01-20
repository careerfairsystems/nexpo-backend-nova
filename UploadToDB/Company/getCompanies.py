import requests
import json

import sys
sys.path.append("..")
import login

"""
Get all companies
"""

auth_token = login.get_token()

url = "https://www.nexpo.arkadtlth.se/api/companies"

headers = {
    'Authorization': auth_token,
    'Content-Type': 'application/json'
}

response = requests.get(url, headers=headers)

if response.status_code == 200:
    companies = response.json()
    print("List of all companies:")
    print(json.dumps(companies, indent=4))
else:
    print(f"Failed to get companies. Status code: {response.status_code}")
    