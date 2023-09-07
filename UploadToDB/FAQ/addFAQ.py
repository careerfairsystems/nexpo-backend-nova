import json
import pandas as pd
import requests

import sys
sys.path.append("..")
import login

"""
Add a one or several FAQ cards to the database, using JSON
"""

jsonfile = '../jsonTemplate/FAQ.json'
url = 'https://www.nexpo.arkadtlth.se/api/faq/add'


token = login.get_token()

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)

df = pd.DataFrame(dictData)

# ...

for row in range(len(df)):
    faq = df.iloc[row]
    id = faq['id']
    question = faq['question']
    answer = faq['answer']

    headers = {
        'accept': 'text/plain',
        'Content-Type': 'application/json',
        'Authorization': token,
    }

    data = {
        'id': id,
        'question': question,
        'answer': answer
    }

    data_str = json.dumps(data)
    r = requests.post(url, data=data_str.encode('utf-8'), headers=headers)
    print(r)
    print(r.content)
