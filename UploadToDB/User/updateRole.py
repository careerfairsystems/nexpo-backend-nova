import requests
import json
import sys

sys.path.append("..")
import login

token = login.get_token()
jsonfile = '../jsonTemplate/updateRole.json'

with open(jsonfile, 'r') as f:
    jsonObjects = json.load(f)

#url = "https://www.nexpo.arkadtlth.se/api/users" 
url_user = "http://localhost:5000/api/users"

token = login.get_token()

headers = {
    'Authorization': token,
    'Content-Type': 'application/json'
}

for jsonObj in jsonObjects:
    user_email = jsonObj.get('email')
    new_role = jsonObj.get('role')

    print(user_email, new_role)
    
    response_user = requests.get(url_user, headers=headers)

    if response_user.status_code == 200:
        users = response_user.json()
        for user in users:
            if user.get('email') == user_email:
                user_id = user['id']
                url_role = f"http://localhost:5000/api/role/{user_id}"

                response_role = requests.put(url_role, headers=headers, json={"role": new_role})
        
                if response_role.status_code == 200:
                    print(f"Successfully updated user with ID {user_id}")
                    print(json.dumps(response_role.json(), indent=4))
                else:
                    print(f"Failed to update user with ID {user_id}. Status code: {response_role.status_code}")
                    print(response_role.text)

    else:
        print(f"Failed to get users. Status code: {response_user.status_code}")
        print(response_user.text)