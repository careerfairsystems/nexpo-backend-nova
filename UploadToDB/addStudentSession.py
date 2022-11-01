import json
import login
import pandas as pd
import requests

jsonfile = 'DeloitteSS.json'
url = 'http://nexpo.arkadtlth.se/api/timeslots/add'
loginUrl = 'http://nexpo.arkadtlth.se/api/session/signin'

token = login.login(loginUrl)

headers = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
        'Authorization' : token,
    }

def findMinute(x):  
    if (x %2) == 0:
        miniute = "00"
    else:
        miniute = "30"
    return miniute

def find20min(x):
    if (x%3) == 0:
        miniute = "00"
    elif (x % 3) == 1:
        miniute ="20"
    else:
        miniute ="40"
    return miniute

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)
df = pd.DataFrame(dictData)
for row in range(len(df)):
    studentSession = df.iloc[row]

    location = '"' + studentSession[1] + '"' 
    companyId = studentSession[2]
    date = studentSession [0]

    min = 0
    for x in range(15):
        
        hour = 10  + int(x/3) 
        miniute = find20min(x)
        
        start = '"' + date + "T" + str(hour) +":" + str(miniute) + 'Z"'
        #print(start)
        hour = 10 + int((x + 1)/3) 
        miniute = find20min(x + 1)
        end = '"' + str(date) + "T" + str(hour) +":" + str(miniute) + 'Z"'
        #print("end" + end)
        data = '{ "start":'+ start + ', "end":'+ end + ', "location":' + location + ', "companyId": ' + str(companyId)  + '}'
        r = requests.post(url, data=data.encode('utf-8'), headers=headers)
        
        print(r)
        print(r.content)

