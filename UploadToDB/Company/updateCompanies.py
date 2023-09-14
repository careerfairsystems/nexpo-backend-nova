print(dir())

import json
import requests
import sys
import getpass

sys.path.append("..")

LOGIN_URL = "https://www.nexpo.arkadtlth.se/api/session/signin"

def get_token():
    while True:
        password = getpass.getpass("Please enter your app admin password: ") 

        ADMIN_USER = {
            "email": "axel.tobieson@gmail.com",
            "password": password
        }

        response = requests.post(LOGIN_URL, headers={"Content-Type": "application/json"},
                                data=json.dumps(ADMIN_USER))
        
        if response.status_code == 200:
            print("Successfully authenticated.")
            return f"Bearer {response.json()['token']}"
        elif response.status_code == 400:
            print("Unauthorized. Incorrect password, please try again.")
        else:
            print(f"Failed to get token. Status code: {response.status_code}")
            print(response.text)
            break

def read_json_file(filename):
    with open(filename, 'r') as f:
        return json.load(f)

def update_companies(auth_token, companies):
    url = "https://www.nexpo.arkadtlth.se/api/companies/"

    headers = {
        'Authorization': auth_token,
        'Content-Type': 'application/json'
    }

    all_companies = requests.get(url, headers=headers).json()

    for company in companies:
        print(company)
        company_name = company.get("name")
        filtered_companies = list(filter(lambda c : (c.get("name") == company_name), all_companies))
        if len(filtered_companies) == 0:
            print(f"Company with name {company_name} not found in DB")
            continue
        company_by_name = filtered_companies[0]
        company_id = company_by_name.get("id")

        fields_to_update = {key: value for key, value in company.items() if key != "id"}
        put_url = url + str(company_id)
        response = requests.put(put_url, headers=headers, json=fields_to_update)

        if response.status_code == 200:
            print(f"Successfully updated company with name {company_name}")
            print(json.dumps(response.json(), indent=4))
        else:
            print(f"Failed to update user with name {company_name}. Status code: {response.status_code}")
            print(response.text)


if __name__ == "__main__":
    auth_token = get_token()
    filename = "../jsonTemplate/updateCompanies.json"
    
    users = read_json_file(filename)
    update_companies(auth_token, users)
