import json
import pandas as pd
import requests

jsonfile = 'example.json'
url = 'http://{url}/api/companies/add'
s3BucketUrl = 's3bucketURL'
loginUrl = 'http://{url}/api/session/signin'


loginHeaders = {
    'accept': 'text/plain',
    'Content-Type': 'application/json',
}
dataLogin = '{ "email": "{adminEmail}", "password": "{adminPassword}" }'

login = requests.post(loginUrl, headers=loginHeaders, data=dataLogin)

token =login.text.replace('"token":', 'Bearer ').replace('{','').replace('}', ''). replace('"' , '')
print(login)

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
            des:str = str(prof['desiredDegree']).replace('Ph.D', 'Phd').replace('Master’s degree (300 ECTS)', 'Masters').replace('Bachelor’s degree (180 ECTS)', 'Bachelor')
            desiredDegree = des.replace("'", '"')
            desiredDegree = desiredDegree
            
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
            'Content-Type': 'application/json',
            'Authorization' : token,
        }
        data = '{ "name":' + name + ', "description":' + description +', "didYouKnow":' + didYouKnow + ', "website":' + website + ', "logoUrl":' + logoUrl +'}'

        #desiredDegree in work
        #data = '{ "name":' + name + ', "description":' + description +', "didYouKnow":' + didYouKnow + ', "website":' + website + ', "logoUrl":' + logoUrl + ', "degrees":'+  desiredDegree +'  }'


        r = requests.put(url, data=data.encode('utf-8'), headers=headers)
        print(r)
        print(r.content)
        
