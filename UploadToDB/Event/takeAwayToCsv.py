import requests
import csv
import sys
sys.path.append("..")
import login 

# Check if a path argument is provided
if len(sys.argv) < 3:
    print("Usage: python3 takeAwayToCsv.py event_id <desired path>")
    sys.exit(1)

id = sys.argv[1]
output_path = sys.argv[2]

output_name = f"takeaway-event-{str(id)}"
csv_file_path = f"{output_path}/{output_name}.csv" 


token = login.get_token()




url = f"https://www.nexpo.arkadtlth.se/api/events/{id}/tickets"

'''
-5
-1
-2
-3
-4
-8
-6
-7
'''


headers = {
    'Authorization': token,
}

response = requests.get(url, headers=headers)

if response.status_code == 200:
    data = response.json()
    print(data)

    if len(data) == 0: 
        print("no available apis ")

    
else:
    print(f"Failed to retrieve data. Status code: {response.status_code}")

'''
    breakpoint()
        
    for event_ticket in data:
        if event_ticket['takeAway'] and (not event_ticket['ticket']['isConsumed']):
            # Format the filename according to the guidelines
            event_name = event_ticket['ticket']['event']['name'].lower().replace(' ', '-')
            csv_file_path = f"{output_path}/{event_name}.csv"
            
            with open(csv_file_path, 'w', newline='') as csvfile:
                fieldnames = ['name', 'time', 'tickets']
                writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
                writer.writeheader()
                
                writer.writerow({
                    'name': event_ticket['ticket']['event']['name'], 
                    'time': event_ticket['takeAwayTime'],
                    'tickets': event_ticket['ticket']['event']['ticketCount']
                })

'''

