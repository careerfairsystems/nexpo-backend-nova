import requests
headers = {
        'accept': 'text/plain',
}

r =  requests.get(url='https://nexpo.arkadtlth.se/api/companies', headers=headers)

#f = open("id.txt")
#f.write("fe")
#f.close()

print (r.content)