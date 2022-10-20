import json
import pandas as pd
import requests

jsonfile = 'Arkad.json'
url = 'http://localhost:5000/api/companies'
s3BucketUrl = 'https://nexpo-bucket.s3.eu-north-1.amazonaws.com/'
loginUrl = 'http://localhost:5000/api/session/signin'


loginHeaders = {
    'accept': 'text/plain',
    'Content-Type': 'application/json',
}
dataLogin = '{ "email": "admin@example.com", "password": "password" }'
login = requests.post(loginUrl, headers=loginHeaders, data=dataLogin)
token =login.text.replace('"token":', 'Bearer ').replace('{','').replace('}', ''). replace('"' , '')
print("lopgin response " , login)

with open(jsonfile, encoding="utf-8") as d:
    dictData = json.load(d)
df = pd.DataFrame(dictData)
for row in range(len(df)):
    pr = df.iloc[row]
    prof = pr['profile']
    
    if type(prof) == dict:
        if 'name' in prof:
            name: str = '"' + prof['name'] + '"'
        else: name = '"' +"" + '"' +'"' 

        if 'weOffer' in prof:
            weOffer: list = prof['weOffer'] 
            positions = set()
            for offer in range(len(weOffer)):
                if weOffer[offer] == 'Thesis' or weOffer[offer] == 'Exjobb':
                    positions.add(0)
                elif weOffer[offer] == 'TraineeEmployment' or weOffer[offer] == 'Heltidsjobb':
                    positions.add(1) 
                elif weOffer[offer] == 'Internship' or weOffer[offer] =='Traineeplatser' or weOffer[offer] == 'Praktikplatser':
                    positions.add(2) 
                elif weOffer[offer] == 'SummerJob' or  weOffer[offer] ==  'Sommarjobb':
                    positions.add(3) 
                elif weOffer[offer] == 'ForeignOppurtunity' or  weOffer[offer] ==  'Utlandsmöjligheter':
                    positions.add(4) 
                elif weOffer[offer] == 'PartTime' or weOffer[offer] == 'Extrajobb':
                    positions.add(5) 
        else:
            print("positions", name)
            positions = set()

        if 'desiredDegree' in prof:
            desiredDegree = prof['desiredDegree']
            for degree in range(len(desiredDegree)):
                if desiredDegree[degree] == 'Ph.D':
                    desiredDegree[degree] = 2
                elif desiredDegree[degree] == 'Master’s degree (300 ECTS)':
                    desiredDegree[degree] = 1
                elif desiredDegree[degree] == 'Bachelor’s degree (180 ECTS)':
                    desiredDegree[degree] = 0
        else:
            print("desiredDegree", name)
            desiredDegree = []

        #missing Industries: strategy, consumer goods
        if 'industry' in prof:
            industry: list= prof['industry'] 
            industryResult = set()
            for ind in range(len(industry)):
                if industry[ind] == 'ElectricityEnergyPower' or industry[ind] == 'Electricity' or industry[ind] == 'Energy & power' or industry[ind] == 'El, Energi och kraft':
                    industryResult.add(0)
                if industry[ind] == 'Environment' or industry[ind] == 'renewable energy' or industry[ind] == 'miljö' or industry[ind] == 'Life Science' or industry[ind] == 'water':
                    industryResult.add(1)
                if industry[ind] == 'Banking, Finance' or industry[ind] == 'Bank och finans':
                    industryResult.add(2)
                if industry[ind] == 'Union' or industry[ind] == 'Fackförbund':
                    industryResult.add(3)
                if industry[ind] == 'Investment' or industry[ind] == 'Investering':
                    industryResult.add(4)
                if industry[ind] == 'Insurance' or industry[ind] == 'Försäkring':
                    industryResult.add(5)
                if industry[ind] == 'Recruitment':
                    industryResult.add(6)
                if industry[ind] == 'Construction' or industry[ind] == 'Vägledning' or industry[ind] == 'Bygg' or  industry[ind] == 'Fastigheter & Infrastruktur' or  industry[ind] == 'Property & Infrastructure':
                    industryResult.add(7)
                if industry[ind] == 'Architecture' or industry[ind] == 'Arkitektur och Grafisk design':
                    industryResult.add(8)
                if industry[ind] == 'GraphicDesign' or industry[ind] == 'Arkitektur och Grafisk design':
                    industryResult.add(9)
                if industry[ind] == 'DataIT' or  industry[ind] == 'Data and IT' or industry[ind] == 'Data' or industry[ind] == 'information science' or industry[ind] == 'it':
                    industryResult.add(10)
                if industry[ind] == 'FinanceConsultancy':
                    industryResult.add(11)
                if industry[ind] == 'Telecommunication' or industry[ind] == 'Telekommunikation':
                    industryResult.add(12)
                if industry[ind] == 'Consulting' or industry[ind] == 'Bemanning & Arbetsförmedling' or industry[ind] == 'Ekonomi och konsultverksamhet' or industry[ind] == 'Konsultverksamhet':
                    industryResult.add(13)
                if industry[ind] == 'Management':
                    industryResult.add(14)
                if industry[ind] == 'Media':
                    industryResult.add(15)
                if industry[ind] == 'Industry' or industry[ind] == 'Industri':
                    industryResult.add(16)
                if industry[ind] == 'NuclearPower' or  industry[ind] =='Kärnkraft':
                    industryResult.add(17)
                if industry[ind] == 'LifeScience':
                    industryResult.add(18) 
                if industry[ind] == 'MedialTechniques' or  industry[ind] == 'Medicinteknik':
                    industryResult.add(19)
                if industry[ind] == 'PropertyInfrastructure':
                    industryResult.add(20)
                if industry[ind] == 'Research' or industry[ind] == 'Forskning':
                    industryResult.add(21)
                if industry[ind] == 'Coaching':
                    industryResult.add(22)
        else:
            print("industryResult", name)
            industryResult = set()

        #tagit höjd för att ändra "guilds" till "programme"
        if "desiredProgramme" in prof:
            desiredProgramme: list = prof['desiredProgramme']
            desiredProgrammeResult = set()
            for programme in range(len(desiredProgramme)):              
                if desiredProgramme[programme] == 'Brandingenjörsutbildning':
                    desiredProgrammeResult.add(8)
                elif desiredProgramme[programme] == 'Maskinteknik med teknisk design':
                    desiredProgrammeResult.add(7)
                elif desiredProgramme[programme] == 'Elektroteknik':
                    desiredProgrammeResult.add(2)
                elif desiredProgramme[programme] == 'Ekosystemteknik':
                    desiredProgrammeResult.add(9)
                elif desiredProgramme[programme] == 'Maskinteknik':
                    desiredProgrammeResult.add(7)
                elif desiredProgramme[programme] == 'TekniskNanovetenskap':
                    desiredProgrammeResult.add(3)
                elif desiredProgramme[programme] == 'Bioteknik':
                    desiredProgrammeResult.add(6)
                elif desiredProgramme[programme] == 'Industridesign':
                    desiredProgrammeResult.add(0)
                elif desiredProgramme[programme] == 'Arkitekt':
                    desiredProgrammeResult.add(0)
                elif desiredProgramme[programme] == 'Informations- och kommunikationsteknik':
                    desiredProgrammeResult.add(1)
                elif desiredProgramme[programme] == 'Kemiteknik':
                    desiredProgrammeResult.add(6)
                elif desiredProgramme[programme] == 'Byggteknik med järnvägsteknik':
                    desiredProgrammeResult.add(5)
                elif desiredProgramme[programme] == 'Väg- och vatttenbyggnad':
                    desiredProgrammeResult.add(8)
                elif desiredProgramme[programme] == 'Byggteknik med arkitektur':
                    desiredProgrammeResult.add(5)
                elif desiredProgramme[programme] == 'Industriell ekonomi':
                    desiredProgrammeResult.add(4)
                elif desiredProgramme[programme] == 'Teknisk Matematik':
                    desiredProgrammeResult.add(3)
                elif desiredProgramme[programme] == 'Medicinteknik':
                    desiredProgrammeResult.add(2)
                elif desiredProgramme[programme] == 'Lantmäteri':
                    desiredProgrammeResult.add(8)
                elif desiredProgramme[programme] == 'Datateknik':
                    desiredProgrammeResult.add(1)
                elif desiredProgramme[programme] == 'Teknisk Fysik':
                    desiredProgrammeResult.add(3)
                elif desiredProgramme[programme] == 'Byggteknik med väg- och trafikteknik':
                    desiredProgrammeResult.add(5)
        else:
            print("desired Programme", name)
            desiredProgrammeResult = set()
            desiredProgramme  = '"' +"" + '"'

        if 'didYouKnow' in prof:
            did: str = prof['didYouKnow'].replace('"','\"') .replace('\n', '').replace(chr(0x0B), '').replace(chr(0x09),'')
            didYouKnow: str = '"' + did + '"'
        else:
            didYouKnow = '"' +"" + '"' 

        if 'aboutUs' in prof:
            desc = prof['aboutUs'].replace('"','\"').replace('\n', '').replace(chr(0x0B), '').replace(chr(0x09),'')
            description: str = '"' + desc + '"'
        else:
            description = '"' +"" + '"' 

        if 'urlWebsite' in prof:
            website:str = '"' + prof['urlWebsite'] + '"'
        else:
            website = '"' +"" + '"' 

        if 'logotype' in prof and prof['logotype'] != None:

            logoUrl  = prof['logotype']['name']
            logoUrl: str  = '"' + s3BucketUrl + logoUrl.replace('eps','jpg') + '"'
        else:
            logoUrl  =  '"' +""+ '"' 

        headers = {
            'accept': 'text/plain',
            'Content-Type': 'application/json',
            'Authorization' : token,
        }
        data = '{ "name":' + name + ', "description":' + description +', "didYouKnow":' + didYouKnow + ', "website":' + website + ', "logoUrl":' + logoUrl + ',"desiredDegrees":' + json.dumps(desiredDegree) + ',"desiredGuilds":' + json.dumps(list(desiredProgrammeResult)) + ',"positions":' + json.dumps(list(positions)) + ',"industries":' + json.dumps(list(industryResult)) +  '}'
        r = requests.post(url, data=data.encode('utf-8'), headers=headers)
        print(r)
        #print(r.content)