import requests
import json

import sys
sys.path.append("..")
import login

"""
Get a company, by its name

Note that the function is case sensitive
"""

if len(sys.argv) > 1:
    company_name_to_search = sys.argv[1]
else:
    company_name_to_search = input("Enter the company name to search for: ")

auth_token = login.get_token()

url = "https://www.nexpo.arkadtlth.se/api/companies"

headers = {
    'Authorization': auth_token,
    'Content-Type': 'application/json'
}

response = requests.get(url, headers=headers)

if response.status_code == 200:
    companies = response.json()
        
    matching_companies = [company for company in companies if company_name_to_search.lower() in company['name'].lower()]

    if matching_companies:
        print(f"List of companies matching {company_name_to_search}:")
        print(json.dumps(matching_companies, indent=4))  # pretty print JSON
    else:
        print(f"No companies match the name {company_name_to_search}")
else:
    print(f"Failed to get companies. Status code: {response.status_code}")
    print(response.text)



