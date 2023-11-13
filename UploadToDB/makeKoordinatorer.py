import pandas as pd
import json

# Load the Excel file
df = pd.read_excel('/Users/victormikkelsen/ARKAD/2023/IT/nexpo-backend-nova/UploadToDB/värdar.xlsx', sheet_name='Koordinatorer Biljetter', header=None)

# Define the column indices for F to I (5 to 8 if using zero-based indexing)
time_columns_indices = list(range(5, 9))  # F=5, G=6, H=7, I=8

ticket_list = []

for index, row in df.iterrows():
    if 5 <= index < 59:  # Adjust the row numbers for the emails and tickets as needed
        email = row[3]  # Assuming the email is in the 4th column (D)
        # Since total_tickets is not used in this script, it is removed to avoid an undefined variable error

        time_slots_with_tickets = []
        for col_index in time_columns_indices:
            time_slot = df.iloc[3, col_index]  # Assuming the times are in row 4
            tickets = row[col_index]  # Tickets for corresponding time slot

            # Check if the time slot is a string and the tickets are not NaN
            if isinstance(time_slot, str) and not pd.isna(tickets):
                # Assuming tickets should be an integer value
                time_slots_with_tickets.append(f"{time_slot}")

        # Format the times string
        times_string = ', '.join(time_slots_with_tickets)

        # Construct the ticket JSON object
        ticket_json = {
            "tickets": {
                "mail": email,
                "eventid": 59,  # Assuming a new event ID for the updated script
                "numberOfTickets": 1,  # Assuming each email gets one ticket
                "appearAt": f"Use the attached QR-code(s) to receive your lunch. You are welcome to attend for lunch at the following location and time:<br><br><b>Location</b>: Kårhuset, Cornelis<br><br><b>Time</b>: {times_string}<br><br>If you have any questions about the lunch, contact service.arkad@box.tlth.se.<br><br>Enjoy your lunch!"
            }
        }

        # Add the ticket JSON object to the list
        ticket_list.append(ticket_json)

# Convert the list of ticket JSON objects to a JSON string
json_output = json.dumps(ticket_list, indent=4)

# Output to a file
output_file_path = '/Users/victormikkelsen/ARKAD/2023/IT/nexpo-backend-nova/UploadToDB/koordinator_tickets.json'
with open(output_file_path, 'w') as file:
    file.write(json_output)

print(f"JSON data has been written to {output_file_path}")
