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
                name = ""
            
            try:
                we_Offer = item['profile']['weOffer']
            except KeyError:
                we_Offer = ""
            
            try:
                desiredDegree = item['profile']['desiredDegree']
            except KeyError:
                desiredDegree = ""
            
            try:
                industry = item['profile']['industry']
            except KeyError:
                industry = ""
            
            try:
                desiredProgramme = item['profile']['desiredProgramme']
            except KeyError:
                desiredProgramme = ""
            
            try:
                didYouKnow = item['profile']['didYouKnow']
            except KeyError:
                didYouKnow = ""
            
            try:
                aboutUs = item['profile']['aboutUs']
            except KeyError:
                aboutUs = ""
            
            try:
                urlWebsite = item['profile']['urlWebsite']
            except KeyError:
                urlWebsite = ""
            
            try:
                logotype = item['exhibition']['logotype']
            except KeyError:
                logotype = ""
            
            try:
                companyHosts = item['profile']['companyHosts']
            except KeyError:
                companyHosts = ""
            
            output_data.append(
                {
                    "profile": {
                        "name": name,
                        "weOffer": [we_Offer],
                        "desiredDegree": desiredDegree,
                        "industry": industry,
                        "desiredProgramme": desiredProgramme,
                        "didYouKnow": didYouKnow,
                        "aboutUs": aboutUs,
                        "urlWebsite": urlWebsite,
                        "logotype": logotype
                    },
                    "companyHosts": [
                        {
                            "email": companyHosts
                        }
                    ]
                }
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
