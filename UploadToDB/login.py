import requests
import json
import getpass  # For secure password input
import login

'''
Used to retreive an admin token.
This token can then be used, in for example other scripts,
to make admin HTTP requests. 

WARNING!!! Update this file with an account with admin credentials. 
Make sure to keep the password out of version control if you enter it.
'''

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
