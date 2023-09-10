import json

def parse_json_file(input_file, output_file):
    try:
        # Open the original JSON file for reading
        with open(input_file, 'r') as json_file:
            # Read the JSON data from the original file
            data = json.load(json_file)

            name = data[0]['prereg']['name']
            we_Offer = data[0]['profile']['weOffer']
            desiredDegree = data[0]['profile']['desiredDegree']
            industry = data[0]['profile']['industry']
            desiredProgramme = data[0]['profile']['desiredProgramme']
            try:
                didYouKnow = data[0]['profile']['didYouKnow']
            except KeyError:
                didYouKnow = ""
            aboutUs = data[0]['profile']['aboutUs']
            urlWebsite = data[0]['profile']['urlWebsite']
            logotype = data[0]['exhibition']['logotype']
            try:
                companyHosts = data[0]['profile']['companyHosts']
            except KeyError:
                companyHosts = ""

        output_data = [
    {
        "profile": {
            "name": name,
            "weOffer": [we_Offer],
            "desiredDegree": [desiredDegree],
            "industry": [industry],
            "desiredProgramme": [desiredProgramme],
            "didYouKnow": didYouKnow,
            "aboutUs": aboutUs,
            "urlWebsite": urlWebsite,
            "logotype":logotype
            
        },
        "companyHosts": [
            {
                "email": companyHosts
            }
        ]
    }
]

        # Open the new JSON file for writing
        with open(output_file, 'w') as new_json_file:
            # Write the JSON data to the new file
            json.dump(output_data, new_json_file, indent=4)
        print("Company Name:", data[0]['prereg']['name'])
        print(f'Successfully duplicated {input_file} to {output_file}')
    except FileNotFoundError:
        print(f"Error: {input_file} not found.")
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    # Input and output file names
    input_file = '../jsonTemplate/arkad_20231009_183832.json' # Replace with the name of your original JSON file
    output_file = "parsedCompany.json"  # Replace with the name of the new JSON file

    # Parse the JSON file
    parse_json_file(input_file, output_file)
