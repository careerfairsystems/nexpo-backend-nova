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
                name = item['prereg']['name']
            except KeyError:
                name = "name"
            
            try:
                description = item['order']['description']
            except KeyError:
                description = "description"
            
            try:
                start = item['order']['start']
            except KeyError:
                start = "start"
            
            try:
                end = item['order']['end']
            except KeyError:
                end = "end"
            
            try:
                location = item['order']['location']
            except KeyError:
                location = "location"
            
            try:
                host = item['order']['host']
            except KeyError:
                host = "host"
            
            try:
                language = item['prereg']['language']
            except KeyError:
                language = "language"
            
            try:
                capacity = item['order']['capacity']
            except KeyError:
                capacity = "capacity"
            
            output_data.append(
                [
                    {
                        "name": name,
                        "description": description,
                        "date": date,
                        "start": start,
                        "end": end,
                        "location": Kårhuset,
                        "host": Test,
                        "language": English,
                        "capacity": capacity
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
