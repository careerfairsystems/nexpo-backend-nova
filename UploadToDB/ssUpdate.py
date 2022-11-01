import json
import string
import login
import pandas as pd
import requests

jsonfile = 'ssupdatecomp.json'
url = 'http://nexpo.arkadtlth.se/api/companies'
loginUrl = 'http://nexpo.arkadtlth.se/api/session/signin'

token = login.login(loginUrl)

headers = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
        'Authorization' : token,
    }

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)
df = pd.DataFrame(dictData)
for row in range(len(df)):
    desc = df.iloc[row]

    id = desc[0]
    motivation = '"' + desc[1].replace("\n", "\\n") + '"'
    data = '{"studentSessionMotivation": ' + motivation + ' }'
    urlid = url + "/" + str(id)
    print(urlid)
    r = requests.put(urlid, data=data.encode('utf-8'), headers=headers)
    print(r)
    #print(r.content)