import requests
import csv
import sys
import login 

# Check if a path argument is provided
if len(sys.argv) < 2:
    print("Usage: python3 takeAwayToCsv.py <desired path>")
    sys.exit(1)

output_path = sys.argv[1]

token = login.get_token()

url = "https://www.nexpo.arkadtlth.se/api/tickets"

headers = {
    'Authorization': token,
}

response = requests.get(url, headers=headers)

if response.status_code == 200:
    data = response.json()

    if len(data) == 0: 
        print("no available apis ")
        
    
    for ticket in data:
        if (ticket['takeAway']) and (not ticket['isConsumed']):
            # Format the filename according to the guidelines
            event_name = ticket['event']['name'].lower().replace(' ', '-')
            csv_file_path = f"{output_path}/{event_name}.csv"
            
            with open(csv_file_path, 'w', newline='') as csvfile:
                fieldnames = ['name', 'time', 'tickets']
                writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
                writer.writeheader()
                
                writer.writerow({
                    'name': ticket['event']['name'], 
                    'time': ticket['takeAwayTime'],
                    'tickets': ticket['event']['ticketCount']
                })
else:
    print(f"Failed to retrieve data. Status code: {response.status_code}")
