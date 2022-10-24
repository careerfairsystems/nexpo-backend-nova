import json
import login
import pandas as pd
import requests

jsonfile = 'example.json'
url = 'http://{localhost}/api/timeslots/add'
loginUrl = 'http://{localhost}/api/session/signin'

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

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)
df = pd.DataFrame(dictData)
for row in range(len(df)):
    studentSession = df.iloc[row]

    location = '"' + studentSession[1] + '"' 
    companyId = studentSession[2]
    date = studentSession [0]

    for x in range(12):
        
        hour = 10 + int(x/2) 
        miniute = findMinute(x)
        start = '"' + date + "T" + str(hour) +":" + str(miniute) + 'Z"'
        hour = 10 + int((x + 1)/2) 
        miniute = findMinute(x + 1)
        end = '"' + str(date) + "T" + str(hour) +":" + str(miniute) + 'Z"'
        data = '{ "start":'+ start + ', "end":'+ end + ', "location":' + location + ', "companyId": ' + str(companyId)  + '}'
        r = requests.post(url, data=data.encode('utf-8'), headers=headers)
        print(r)
        #print(r.content)

