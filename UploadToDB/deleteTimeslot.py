import json
import string
import login
import pandas as pd
import requests

jsonfile = 'ssupdatecomp.json'
url = 'http://localhost:5000/api/timeslots'
#http://localhost:5000/api/timeslots/1
loginUrl = 'http://nexpo.arkadtlth.se/api/session/signin'


token = login.login(loginUrl)
headers = {
        'accept': '*/*',
        'Authorization' : token,
    }
r = requests.delete('http://nexpo.arkadtlth.se/api/timeslots/5', headers=headers)
print(r)
print(r.content)

headers = {
    'accept': '*/*',
    'Authorization' : token,
}

response = requests.delete('http://nexpo.arkadtlth.se/api/timeslots/146', headers=headers)
print(response)
print(response.content)


for x in range(145,226):

    urlId = url + "/" + str(x)
    #print(urlId)

    #r = requests.delete(url=urlId, headers=headers)
    #r = requests.delete('http://nexpo.arkadtlth.se/api/timeslots/145', headers=headers)

    #print(r)
    
