import json
import pandas as pd
import requests

import sys
sys.path.append("..")
import login

"""
Add a one or several FAQ cards to the database, using JSON
"""

jsonfile = '../jsonTemplate/contact.json'
url = 'https://www.nexpo.arkadtlth.se/api/contacts/add'


token = login.get_token()

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)

df = pd.DataFrame(dictData)

for row in range(len(df)):
    contact = df.iloc[row]
    id = contact['id']
    firstName = contact['first_name']
    lastName = contact['last_name']
    roleInArkad = contact['role_in_arkad']
    email = contact['email']
    phoneNumber = contact['phone_number']

headers = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
        'Authorization' : token,}

data = '{ "id":' + str(id) + ', "first_name":' + firstName +', "last_name":' + lastName +', "role_in_arkad":' + roleInArkad +', "email":' + email +', "phone_number":' + phoneNumber + '}'
r = requests.post(url, data=data.encode('utf-8'), headers=headers)
print(r)