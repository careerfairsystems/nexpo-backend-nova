import json
import pandas as pd
import requests
import sys
sys.path.append("..")
import login as login

"""
Creates a student session timeslot for a company, using JSON
"""

jsonfile = './test.json'
# url = 'http://{localhost}/api/timeslots/add'
url = 'https://www.nexpo.arkadtlth.se/api/timeslots/add'

token = login.get_token()

headers = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
        'Authorization': token,
    }

def findMinute(x):
    if (x % 2) == 0:
        minute = "00"
    else:
        minute = "30"
    return minute

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)

df = pd.DataFrame(dictData)

for row in range(len(df)):
    studentSession = df.iloc[row]

    location = '"' + studentSession[1] + '"'
    companyId = studentSession[2]
    date = studentSession[0]

    end_hour = 15 if date == '2023-11-15' else 16  # Set different end times based on the date
    for hour in range(10, end_hour):
        for minute in [0, 30]:
            start = f'"{date}T{hour:02d}:{minute:02d}Z"'
            end_hour = hour
            end_minute = minute + 30
            if end_minute == 60:
                end_hour += 1
                end_minute = 0
            end = f'"{date}T{end_hour:02d}:{end_minute:02d}Z"'

            data = f'{{ "start": {start}, "end": {end}, "location": {location}, "companyId": {companyId} }}'
            print(data)
            r = requests.post(url, data=data.encode('utf-8'), headers=headers)
            print(f"Got {r}")
