import pandas as pd
import json

# Load the Excel file
df = pd.read_excel('/Users/victormikkelsen/ARKAD/2023/IT/nexpo-backend-nova/UploadToDB/lunch.xlsx', sheet_name='Schema', header=None)

# Define the column indices for P to X (15 to 23 if using zero-based indexing)
time_columns_indices = list(range(15, 24))  # P=15, Q=16, ..., X=23

ticket_list = []

for index, row in df.iterrows():
    if 5 <= index < 195:  # Adjust the row numbers for the emails and tickets as needed
        email = row[3]  # Assuming the email is in the 4th column (D)
        total_tickets = row[5]  # The total tickets are now in the 6th column (F)

        # Handle NaN values by skipping or setting a default value for total_tickets
        if pd.isna(total_tickets):
            continue  # Skip this row, or you could set a default value like total_tickets = 0

        time_slots_with_tickets = []
        for col_index in time_columns_indices:
            time_slot = df.iloc[3, col_index]  # Assuming the times are in row 4
            tickets = row[col_index]  # Tickets for corresponding time slot

            # Check if the time slot is a string and the tickets are not NaN
            if isinstance(time_slot, str) and not pd.isna(tickets):
                # Assuming tickets should be an integer value
                time_slots_with_tickets.append(f"{time_slot} ({int(tickets)} indv.)")

        # Format the times string
        times_string = ', '.join(time_slots_with_tickets)

        # Construct the ticket JSON object
        ticket_json = {
            "tickets": {
                "mail": email,
                "eventid": 60,
                "numberOfTickets": int(total_tickets) if not pd.isna(total_tickets) else 0,
                "appearAt": f"Use the attached QR-code(s) to receive your lunch. You are welcome to attend for lunch at the following location and time:<br><br><b>Location</b>: Kårhuset, Moroten och Piskan<br><br><b>Time</b>: {times_string}<br><br>If you have any questions about the lunch, contact service.arkad@box.tlth.se.<br><br>Enjoy your lunch!"
            }
        }

        # Add the ticket JSON object to the list
        ticket_list.append(ticket_json)

# Convert the list of ticket JSON objects to a JSON string
json_output = json.dumps(ticket_list, indent=4)

# Output to a file
output_file_path = '/Users/victormikkelsen/ARKAD/2023/IT/nexpo-backend-nova/UploadToDB/lunch_tickets2.json'
with open(output_file_path, 'w') as file:
    file.write(json_output)

print(f"JSON data has been written to {output_file_path}")
