import requests
import csv
import sys
sys.path.append("..")
import login 

# Validate and Get Arguments
if len(sys.argv) < 3:
    print("Usage: python3 takeAwayToCsv.py event_id <desired path>")
    sys.exit(1)

id = sys.argv[1]
output_path = sys.argv[2]

csv_file_path = f"{output_path}/takeAwayEvent{id}.csv"

token = login.get_token()

url = f"http://localhost:5000/api/events/{id}/tickets"
headers = {'Authorization': token}
response = requests.get(url, headers=headers)

if response.status_code == 200:
    data = response.json()
    if not data:
        print("No available tickets")
        sys.exit(0)  
    
    with open(csv_file_path, 'a', newline='') as csvfile:
        fieldnames = ['time', 'tickets']
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        
        if csvfile.tell() == 0:
            writer.writeheader()
        
        # Loop through all tickets and write all to the CSV regardless of their takeAway and isConsumed status
        for event_ticket in data: 
            if (not event_ticket['ticket']['isConsumed']) and (event_ticket['ticket']['takeAway']):
                ticket = event_ticket.get('ticket', {})
                writer.writerow({
                    'time': ticket.get('takeAwayTime', 'N/A'),  # Using 'N/A' as a default value if 'takeAwayTime' is not present
                    'tickets': len(data)
                })
else:
    print(f"Failed to retrieve data. Status code: {response.status_code}")
