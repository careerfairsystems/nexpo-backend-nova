import requests

import sys
sys.path.append("..")
import login

"""
Delete a company by using its name
"""

if len(sys.argv) > 1:
    company_name = sys.argv[1]
else:
    company_name = input("Enter the company name to delete: ")

token = login.get_token()
    
url = f"https://www.nexpo.arkadtlth.se/api/companies/{company_name}"

headers = {
    'Authorization': token,
}

r = requests.delete(url, headers=headers)

if r.status_code == 200 or r.status_code == 204:
    print(f"Successfully deleted company with name {company_name}.")
else:
    print(f"Failed to delete company with name {company_name}. Error: {r.text}")


