import json
import pandas as pd
import requests
import sys
from urllib.parse import quote
sys.path.append("..")
import login

# uppdate one or several companies to the database, using JSON
# It is recommended to export this data from Jexpo

jsonfile = '../jsonTemplate/arkad_20241310_105055.json'
#url = 'https://www.nexpo.arkadtlth.se/api/companies/update/'
url = 'http://localhost:5000/api/companies/update/'
s3BucketUrl = 'https://nexpo-bucket.s3.eu-north-1.amazonaws.com/'

token = login.get_token()

# Load JSON data
with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)

df = pd.DataFrame(dictData)
print(len(df))

# Process each company in the DataFrame
for row in range(len(df)):
    pr = df.iloc[row]
    prof = pr['profile']
    companyHost = pr.get('companyHosts', [])

    # Process each company in the DataFrame
for row in range(len(df)):
    pr = df.iloc[row]
    prof = pr['profile']
    companyHost = pr.get('companyHosts', [])

    if isinstance(prof, dict):
        name = prof.get('name', '')
        description = prof.get('aboutUs', '')
        didYouKnow = prof.get('didYouKnow', '')
        website = prof.get('urlWebsite', '')

        # Get logotype URL
        if 'logotype' in prof and prof['logotype'] is not None:
            logo_file_name = prof['logotype']['name']
            logoUrl = s3BucketUrl + logo_file_name.replace('eps', 'jpg')
        else:
            logoUrl = '"' +""+ '"' 


        # Prepare the email for company hosts
        companyHostsEmail = '""'
        if isinstance(companyHost, list) and len(companyHost) > 0:
            if 'email' in companyHost[0]:
                companyHostsEmail = f'"{companyHost[0]["email"]}"'

        # Prepare the JSON data for the request
        data = {
            "description": description,
            "didYouKnow": didYouKnow,
            "website": website,
            "logoUrl": logoUrl,
        }

        # Convert the data dictionary to JSON string
        data_json = json.dumps(data)

        # Set up headers
        headers = {
            'accept': 'text/plain',
            'Content-Type': 'application/json',
            'Authorization': token,
        }

        encoded_name = quote(name)

        # Send the request
        r = requests.put(f"{url}{encoded_name}", data=data_json, headers=headers)

        # Log response
        if r.status_code == 200:
            print(f"{r.status_code}, \"{name}\"; Request succeeded.")
        else:
            print(f"Encoded URL: {url}{encoded_name}")
            try:
                # Attempt to print JSON error response
                error_json = r.json()
                print(f"Failed to update {name}. Status code: {r.status_code}, Error: {json.dumps(error_json, indent=4)}")
            except json.JSONDecodeError:
                # Fallback to plain text if JSON decoding fails
                print(f"Failed to update {name}. Status code: {r.status_code}, Response: {r.text}")

      