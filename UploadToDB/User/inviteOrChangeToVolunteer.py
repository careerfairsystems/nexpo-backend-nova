import requests
import json
import sys

sys.path.append("..")
import login

# Not the nicest code you have seen :()
def read_json_file(filename):
    with open(filename, 'r') as f:
        return json.load(f)

def send_invitations(auth_token, representatives):
    url = "https://www.nexpo.arkadtlth.se/api/signup/volunteer"
    headers = {
        'Authorization': auth_token,
        'Content-Type': 'application/json'
    }
    
    for rep in representatives:
        response = requests.post(url, headers=headers, json=rep)
        
        if response.status_code == 204:
            print(f"Successfully invited representative {rep['email']}.")
        else:
            print(f"Failed to invite representative {rep['email']}. Status code: {response.status_code}")
            print(response.text)

def update_user_roles(auth_token, jsonObjects):
    url_user = "https://www.nexpo.arkadtlth.se/api/users" 

    headers = {
        'Authorization': auth_token,
        'Content-Type': 'application/json'
    }

    for jsonObj in jsonObjects:
        user_email = jsonObj.get('email')
        new_role = 3

        print(user_email, new_role)
        
        response_user = requests.get(url_user, headers=headers)

        if response_user.status_code == 200:
            users = response_user.json()
            for user in users:
                if user.get('email') == user_email:
                    user_id = user['id']
                    url_role = f"https://www.nexpo.arkadtlth.se/api/role/{user_id}"

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

if __name__ == "__main__":
    auth_token = login.get_token()

    # First, send invitations
    filename_invitations = "../jsonTemplate/inviteVolunteers.json"
    representatives = read_json_file(filename_invitations)
    send_invitations(auth_token, representatives)

    # Then, update user roles
    filename_roles = "../jsonTemplate/inviteVolunteers.json"
    roles_data = read_json_file(filename_roles)
    update_user_roles(auth_token, roles_data)
