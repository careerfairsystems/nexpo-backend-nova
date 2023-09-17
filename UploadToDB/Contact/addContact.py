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



data = { 
    'id' : int(id),
    'firstName': str(firstName),
    'lastName': str(lastName),
    'roleInArkad': str(roleInArkad),
    'email': str(email),
    'phoneNumber': str(phoneNumber)
}

headers = {
    'accept': 'text/plain',
    'Content-Type': 'application/json',
    'Authorization': token,
}

r = requests.post(url, json.dumps(data), headers=headers)  # Use json.dumps to serialize the dictionary
#print(r.status_code)
#print(r.content)
