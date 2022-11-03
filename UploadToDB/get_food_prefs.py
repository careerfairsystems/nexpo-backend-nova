import requests
import json
import pandas as pd
from collections import Counter

HEADER = {
    "accept": "application/json",
    "Content-Type": "application/json",
}

BASE_URL = "https://www.nexpo.arkadtlth.se/api"

ADMIN_USER = {
    "email": "admin@admin.com",
    "password": "admin"
}

def _get_token():
    url = f"{BASE_URL}/session/signin"
    response = requests.post(url, headers=HEADER, data=json.dumps(ADMIN_USER))
    return f"Bearer {response.json()['token']}"


def all_events():
    url = f"{BASE_URL}/events"
    events = requests.get(url, headers=HEADER).json()
    for event in events:
        print(event['id'], event['name'])
        get_food_pref(event['id'])


def get_food_pref(event_id: int):
    food_preferences = []
    url = f"{BASE_URL}/events/{event_id}/tickets"
    admin_header = {**HEADER, 'authorization': _get_token()}

    tickets = requests.get(url, headers=admin_header).json()
    for ticket in tickets:
        user_id = ticket['ticket']['userId']
        user_info = requests.get(f"{BASE_URL}/users/{user_id}", headers=admin_header)
        user_food_pref = user_info.json()['foodPreferences']

        if user_food_pref not in [None, 'null', '', 'None']:
            food_preferences.append(user_food_pref)
        else:
            food_preferences.append('Nothing')

    df = pd.DataFrame(Counter(food_preferences), index=["Pref", "Count"])
    df.to_csv(f'food_prefs_{event_id}.csv', sep=';')
    print(f"Wrote {len(food_preferences)} food preferences to file food_prefs_{event_id}.csv")

if __name__ == "__main__":
    # Get prefs by event_id: 
    get_food_pref(3)

    # Get all prefs on available events:
    #all_events()
