import json
import pandas as pd
import requests

import sys
sys.path.append("..")
import login

"""
Creates a student session timeslot for a company, using JSON
"""

jsonfile = '/home/alexander/uni/Arkad/json objects/SSTest.json'
# url = 'http://{localhost}/api/timeslots/add'
url = 'https://www.nexpo.arkadtlth.se/api/timeslots/add'

token = login.get_token()

headers = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
        'Authorization' : token,
    }

def findMinute(x):
    if (x%2) == 0:
        miniute = "00"
    else:
        miniute = "30"
    return miniute

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)
df = pd.DataFrame(dictData)
for row in range(len(df)):
    studentSession = df.iloc[row]

    location = '"' + studentSession[1] + '"' 
    companyId = studentSession[2]
    date = studentSession [0]

    for x in range(2):
        hour = 10 + int(x/2) 
        miniute = findMinute(x)
        start = '"' + date + "T" + str(hour) +":" + str(miniute) + 'Z"'
        hour = 10 + int((x + 1)/2) 
        miniute = findMinute(x + 1)
        end = '"' + str(date) + "T" + str(hour) +":" + str(miniute) + 'Z"'
        data = '{ "start":'+ start + ', "end":'+ end + ', "location":' + location + ', "companyId": ' + str(companyId)  + '}'
        r = requests.post(url, data=data.encode('utf-8'), headers=headers)
        print("got " + str(r))
        #print(r.content)

