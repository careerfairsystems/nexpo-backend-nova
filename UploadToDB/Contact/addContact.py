import json
import pandas as pd
import requests

import sys
sys.path.append("..")
import login

"""
Add Contact to the database, using JSON
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
    firstName = contact['firstName']
    lastName = contact['lastName']
    roleInArkad = contact['roleInArkad']
    email = contact['email']
    phoneNumber = contact['phoneNumber']

headers = {'Content-Type': 'application/json',
        'Authorization' : token,}

data = { 'id' : id,
        'firstName': firstName,
        'lastName': lastName,
        'roleInArkad': roleInArkad,
        'email': email,
        'phoneNumber': phoneNumber
        }

r = requests.post(url, data, headers)
print(r)