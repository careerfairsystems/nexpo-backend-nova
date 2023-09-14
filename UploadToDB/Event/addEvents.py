import json
import pandas as pd
import requests

import sys
sys.path.append("..")
import login


jsonfile = '../jsonTemplate/event.json' 
url = 'https://www.nexpo.arkadtlth.se/api/events'
loginUrl = 'https://www.nexpo.arkadtlth.se/api/session/signin'

token = login.get_token()

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)
df = pd.DataFrame(dictData)
for row in range(len(df)):
    event = df.iloc[row]

    print(event[0])
    name = '"' + event[0] + '"' 

    description = '"' +event[1]+ '"' 
    date = '"' +event[2] + '"' 
    start = '"' +event[3] + '"' 
    end = '"' +event[4] + '"' 
    location = '"' +event[5] + '"' 
    host = '"' + event[6] + '"' 
    language = '"' + event[7] + '"' 
    capacity =  event[8] 
    

    headers = {
            'accept': 'text/plain',
            'Content-Type': 'application/json',
            'Authorization' : token,
        }
    data = '{ "name":' + name + ', "description":' + description +', "date":' + date + ', "start":' + start + ', "end":' + end + ',"location":' + location + ',"host":' + host + ',"language":' + language + ',"capacity":' + str(capacity)+  '}'
    r = requests.post(url, data=data.encode('utf-8'), headers=headers)
    print(r)
    #print(r.content)