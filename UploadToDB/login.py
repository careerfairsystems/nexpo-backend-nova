import requests
import json
'''
Update this file with account with admin credentials. Make sure to keep this file out of version control.
'''
ADMIN_USER = {
    "email": "admin@admin.com",
    "password": "admin"
}

LOGIN_URL = "https://www.nexpo.arkadtlth.se/api/session/signin"

def get_token():
    response = requests.post(LOGIN_URL, headers={"Content-Type": "application/json"},
        data=json.dumps(ADMIN_USER))
    return f"Bearer {response.json()['token']}"