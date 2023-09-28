import json

def parse_json_file(input_file, output_file):
    try:
        # Open the original JSON file for reading
        with open(input_file, 'r') as json_file:
            # Read the JSON data from the original file
            data = json.load(json_file)
        
        output_data = []

        for item in data:
            try:
                lunch_tickets_day1 = item['tickets']['lunch_tickets_day1']
            except KeyError:
                lunch_tickets_day1 = "lunch_tickets_day1"
            
            try:
                lunch_tickets_day2 = item['tickets']['lunch_tickets_day2']
            except KeyError:
                lunch_tickets_day2= "lunch_tickets_day2"
            
            try:
                banquet_tickets = item['tickets']['banquet_tickets']
            except KeyError:
                banquet_tickets = "banquet_tickets"

            try:
                email = item['prereg']['contact']['email']
            except KeyError:
                email = "email"
                
            output_data.append(
                [
                    {
                        "tickets": {
                            "lunch_tickets_day1": lunch_tickets_day1,
                            "lunch_tickets_day2": lunch_tickets_day2,
                            "banquet_tickets": banquet_tickets,
                            "email": email
                        }
                    }
                ]
                        )
        
        # Open the new JSON file for writing
        with open(output_file, 'w') as new_json_file:
            # Write the JSON data to the new file
            json.dump(output_data, new_json_file, indent=4)
        
        print(f'Successfully duplicated {input_file} to {output_file}')
    except FileNotFoundError:
        print(f"Error: {input_file} not found.")
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    # Input and output file names
    input_file = '../jsonTemplate/real.json' # Replace with the name of your original JSON file
    output_file = "parsedCompany.json"  # Replace with the name of the new JSON file

    # Parse the JSON file
    parse_json_file(input_file, output_file)
