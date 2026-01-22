import requests
import json
import sys

sys.path.append("..")
import login

def read_json_file(filename):
    with open(filename, 'r') as f:
        return json.load(f)

def update_users(auth_token, users):
    url_template = "https://www.nexpo.arkadtlth.se/api/users/{user_id}"
    headers = {
        'Authorization': auth_token,
        'Content-Type': 'application/json'
    }
    
    for user in users:
        user_id = user.get("user_id")
        url = url_template.format(user_id=user_id)
        
        fields_to_update = {key: value for key, value in user.items() if key != "user_id"}
        
        response = requests.put(url, headers=headers, json=fields_to_update)
        
        if response.status_code == 200:
            print(f"Successfully updated user with ID {user_id}")
            print(json.dumps(response.json(), indent=4))
        else:
            print(f"Failed to update user with ID {user_id}. Status code: {response.status_code}")
            print(response.text)

if __name__ == "__main__":
    auth_token = login.get_token()
    filename = "../jsonTemplate/updateUser.json"
    
    users = read_json_file(filename)
    update_users(auth_token, users)
