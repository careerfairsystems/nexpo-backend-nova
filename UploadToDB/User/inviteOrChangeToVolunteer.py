import requests
import json
import sys

sys.path.append("..")
import login

# NOT WORKING CURRENTLTY

def read_json_file(filename):
    with open(filename, 'r') as f:
        return json.load(f)

def send_invitations(auth_token, volunteers):
    url = "https://www.nexpo.arkadtlth.se/api/signup/volunteer"
    headers = {
        'Authorization': auth_token,
        'Content-Type': 'application/json'
    }

    for volunteer in volunteers:
        response = requests.post(url, headers=headers, json=volunteer)

        if response.status_code == 204:
            print(f"Successfully invited volunteer {volunteer['email']}.")
        elif response.status_code == 409:  # Conflict, user already exists
            print(f"User with email {volunteer['email']} already exists. Changing role...")
            update_user_role(auth_token, volunteer['email'], "new_role_here")  # Replace "new_role_here" with the desired role
        else:
            print(f"Failed to invite volunteer {volunteer['email']}. Status code: {response.status_code}")
            print(response.text)

def update_user_role(auth_token, user_email, new_role):
    url_user = "https://www.nexpo.arkadtlth.se/api/users"
    headers = {
        'Authorization': auth_token,
        'Content-Type': 'application/json'
    }

    response_user = requests.get(url_user, headers=headers)

    if response_user.status_code == 200:
        users = response_user.json()
        for user in users:
            if user.get('email') == user_email:
                user_id = user['id']
                url_role = f"https://www.nexpo.arkadtlth.se/api/role/{user_id}"

                response_role = requests.put(url_role, headers=headers, json={"role": new_role})

                if response_role.status_code == 200:
                    print(f"Successfully updated user {user_email} with new role.")
                else:
                    print(f"Failed to update user {user_email} with new role. Status code: {response_role.status_code}")
                    print(response_role.content)
    else:
        print(f"Failed to get users. Status code: {response_user.status_code}")
        print(response_user.text)

if __name__ == "__main__":
    auth_token = login.get_token()
    filename = "../jsonTemplate/inviteVolunteers.json"

    volunteers = read_json_file(filename)
    send_invitations(auth_token, volunteers)
