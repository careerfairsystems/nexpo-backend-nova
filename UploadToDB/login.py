import requests

def login(loginUrl: str):
    loginHeaders = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
    }
    dataLogin = '{ "email": "admin@example.com", "password": "password" }'
    login = requests.post(loginUrl, headers=loginHeaders, data=dataLogin)
    token =login.text.replace('"token":', 'Bearer ').replace('{','').replace('}', ''). replace('"' , '')
    print("lopgin response " , login)
    return token