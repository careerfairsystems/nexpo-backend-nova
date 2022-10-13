import json
from re import I
from typing import Dict
import pandas as pd

import requests

jsonfile = 'ArkadIndent2.json'
url = 'http://localhost/api/companies/add'
s3BucketUrl = 'https://nexpo-bucket.s3.eu-north-1.amazonaws.com/'

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)

df = pd.DataFrame(dictData)

for row in range(len(df)):
    pr = df.iloc[row]
    prof = pr['profile']
    if type(prof) == dict:
    
        if 'name' in prof:
            name: str = '"' + prof['name'] + '"'
        else: name = '"' +"" + '"' +'"' 

        if 'weOffer' in prof:
            weOffer: str = prof['weOffer'] 
        else:
            weOffer = '"' + "" + '"' 

        if 'desiredDegree' in prof:
            desiredDegree: str = prof['desiredDegree']
        else:
            desiredDegree = ""

        if 'industry' in prof:
            industry: str = prof['industry']
        else:
            industry = ""

        if 'desiredProgramme' in prof:
            desiredProgramme: str = prof['desiredProgramme']
        else:
            desiredProgramme  = ""

        if 'didYouKnow' in prof:
            did: str = prof['didYouKnow'].replace('"','\"') .replace('\n', '').replace(chr(0x0B), '').replace(chr(0x09),'')
            didYouKnow: str = '"' + did + '"'
        else:
            didYouKnow = '"' +"" + '"' 

        if 'aboutUs' in prof:
            desc = prof['aboutUs'].replace('"','\"').replace('\n', '').replace(chr(0x0B), '').replace(chr(0x09),'')
            description: str = '"' + desc + '"'
        else:
            description = '"' +"" + '"' 

        if 'urlWebsite' in prof:
            website:str = '"' + prof['urlWebsite'] + '"'
        else:
            website = '"' +"" + '"' 

        if 'logotype' in prof and prof['logotype'] != None:

            logoUrl  = prof['logotype']['name']
            logoUrl: str  = '"' + s3BucketUrl + logoUrl.replace('eps','jpg') + '"'

        else:
            logoUrl  =  '"' +""+ '"' 

  
        headers = {
            'accept': 'text/plain',
            'Content-Type': 'application/json'
        }

        
        data = '{ "name":' + name + ', "description":' + description +', "didYouKnow":' + didYouKnow + ', "website":' + website + ', "logoUrl":' + logoUrl + ' }'
        
        r = requests.put(url, data=data.encode('utf-8'), headers=headers)
        print(r)
        print(r.content)