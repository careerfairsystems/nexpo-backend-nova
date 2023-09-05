import requests
import json
import sys

sys.path.append("..")
import login

def read_json_file(filename):
    with open(filename, 'r') as f:
        return json.load(f)

def send_invitations(auth_token, representatives):
    url = "https://www.nexpo.arkadtlth.se/api/signup/representative"
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

if __name__ == "__main__":
    auth_token = login.get_token()
    filename = "../jsonTemplate/signupRep.json"  # Or any filename you choose
    
    representatives = read_json_file(filename)
    send_invitations(auth_token, representatives)
