import requests
import sys
sys.path.append("..")
import login


token = login.get_token()

# Retrieve a list of all companies first
list_url = "https://localhost:5000/api/companies"
headers = {
    'Authorization': token,
}

response = requests.get(list_url, headers=headers)

# Check if the request was successful
if response.status_code != 200:
    print(f"Failed to retrieve the list of companies. Error: {response.text}")
    sys.exit(1)

companies = response.json()

for company in companies:
    company_name = company['name']
    delete_url = f"https://www.nexpo.arkadtlth.se/api/companies/{company_name}"

    r = requests.delete(delete_url, headers=headers)



