import requests
import csv
import sys
sys.path.append("..")
import login

token = login.get_token()

url = "https://www.nexpo.arkadtlth.se/api/events"

headers = {
    "Authorization": f"Bearer {token}"
}

my_array = [] 

response = requests.get(url, headers=headers)

if response.status_code == 200:
    data = response.json()
    for event in data:
        my_array.append(event['id'])
else:
    print(f"Failed to retrieve data. Status code: {response.status_code}")


if __name__ == "__main__":
    print("available events: " + "\n")
    for i in my_array:
        print(i)
    

