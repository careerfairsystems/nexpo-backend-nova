import json

def parse_json_file(input_file, output_file, output_data):
    try:
        # Open the new JSON file for writing
        with open(output_file, 'w') as new_json_file:
            # Write the JSON data to the new file
            json.dump(output_data, new_json_file, indent=4)
        
        print(f'Successfully parsed {input_file} to {output_file}')
    except FileNotFoundError:
        print(f"Error: {input_file} not found.")
    except Exception as e:
        print(f"An error occurred: {str(e)}")



def parseEvent(input_file):
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
            description = item['order']['rows'][2]['description']
        except KeyError:
            description = "description"
        
        try:
            date = item['order']['rows'][2]['date']
        except KeyError:
            date = "date"
        
        try:
            start = item['order']['rows'][2]['start']
        except KeyError:
            start = "start"
        
        try:
            end = item['order']['rows'][2]['end']
        except KeyError:
            end = "end"
        
        try:
            location = item['order']['location']
        except KeyError:
            location = "location"
        
        try:
            host = item['order']['rows'][2]['host']
        except KeyError:
            host = "host"
        
        try:
            language = item['prereg']['language']
        except KeyError:
            language = "language"
        
        try:
            capacity = item['order']['rows'][2]['capacity']
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
                    "location": location,
                    "host": host,
                    "language": language,
                    "capacity": capacity
                }
                        ]
                    )
        
        output_file = "parsedEvent.json"  # Replace with the name of the new JSON file
        parse_json_file(input_file, output_file, output_data)

def parseCompany(input_file):
    output_data = []
    # Open the original JSON file for reading
    with open(input_file, 'r') as json_file:
        # Read the JSON data from the original file
        data = json.load(json_file)

    for item in data:
        try:
            name = item['prereg']['name']
        except KeyError:
            name = "name"
        
        try:
            we_Offer = item['profile']['weOffer']
        except KeyError:
            we_Offer = "we_Offer"
        
        try:
            desiredDegree = item['profile']['desiredDegree']
        except KeyError:
            desiredDegree = "desiredDegree"
        
        try:
            industry = item['profile']['industry']
        except KeyError:
            industry = "industry"
        
        try:
            desiredProgramme = item['profile']['desiredProgramme']
        except KeyError:
            desiredProgramme = "desiredProgramme"
        
        try:
            didYouKnow = item['profile']['didYouKnow']
        except KeyError:
            didYouKnow = "didYouKnow"
        
        try:
            aboutUs = item['profile']['aboutUs']
        except KeyError:
            aboutUs = "aboutUs"
        
        try:
            urlWebsite = item['profile']['urlWebsite']
        except KeyError:
            urlWebsite = "urlWebsite"
        
        try:
            logotype = item['exhibition']['logotype']['name']
        except KeyError:
            logotype = "logotype"
        
        try:
            companyHosts = item['profile']['companyHosts']
        except KeyError:
            companyHosts = "companyHosts"
        
        output_data.append(
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
                    "logotype": logotype
                },
                "companyHosts": [
                    {
                        "email": companyHosts
                    }
                ]
            }
        )
        
        output_file = "parsedCompany.json"  # Replace with the name of the new JSON file
        parse_json_file(input_file, output_file, output_data)
    
def parseTicket(input_file):
    output_data = []
    # Open the original JSON file for reading
    with open(input_file, 'r') as json_file:
        # Read the JSON data from the original file
        data = json.load(json_file)
        
    for item in data:
        try:
            lunch_tickets_day1 = item['tickets']['lunch_tickets_day1']
        except (KeyError, TypeError):
            lunch_tickets_day1 = "lunch_tickets_day1"
        
        try:
            lunch_tickets_day2 = item['tickets']['lunch_tickets_day2']
        except (KeyError, TypeError):
            lunch_tickets_day2= "lunch_tickets_day2"
        
        try:
            banquet_tickets = item['tickets']['banquet_tickets']
        except (KeyError, TypeError):
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
        
        output_file = "parsedTicket.json"  # Replace with the name of the new JSON file
        parse_json_file(input_file, output_file, output_data)

def parseSS(input_file):
    output_data = []
    # Open the original JSON file for reading
    with open(input_file, 'r') as json_file:
        # Read the JSON data from the original file
        data = json.load(json_file)

    for item in data:
        try:
            name = item['studentsession']['date']
        except KeyError:
            date = "date"
        
        try:
            location = item['studentsession']['location']
        except KeyError:
            location = "location"
        
        try:
            start = item['studentsession']['companyId']
        except KeyError:
            companyId = "companyId"
        
            
        output_data.append(
            [
                {
                    "date": date,
                    "location": location,
                    "companyId": companyId
                }
            ]
        )
    
    output_file = "parsedSS.json"  # Replace with the name of the new JSON file
    parse_json_file(input_file, output_file, output_data)

if __name__ == "__main__":
    # Input and output file names
    input_file = '/Users/victormikkelsen/ARKAD/2023/IT/nexpo-backend-nova/UploadToDB/allCompanies.json' # Replace with the name of your original JSON file
    parseEvent(input_file)
    parseCompany(input_file)
    parseTicket(input_file)
    parseSS(input_file)


    
