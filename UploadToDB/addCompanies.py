import json
from turtle import pos
import pandas as pd
import requests

jsonfile = 'ArkadIndent2.json'
url = 'http://localhost/api/companies/add'
s3BucketUrl = 's3bucketURL'
loginUrl = 'http://localhost/api/session/signin'


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

industrySet = set()

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
                if weOffer[offer] == 'Thesis' or 'Exjobb':
                    positions.add(1)
                if weOffer[offer] == 'TraineeEmployment' or 'Heltidsjobb':
                    positions.add(2) 
                if weOffer[offer] == 'Internship' or 'Traineeplatser' or 'Praktikplatser':
                    positions.add(3) 
                if weOffer[offer] == 'SummerJob' or 'Sommarjobb':
                    positions.add(4) 
                if weOffer[offer] == 'ForeignOppurtunity' or 'Utlandsmöjligheter':
                    positions.add(5) 
                if weOffer[offer] == 'PartTime' or 'Extrajobb':
                    positions.add(6) 
            
        else:
            weOffer = '"' + "" + '"' 

        if 'desiredDegree' in prof:
            
            desiredDegree = prof['desiredDegree']
            
            for degree in range(len(desiredDegree)):
                if desiredDegree[degree] == 'Ph.D':
                    desiredDegree[degree] = 3
                elif desiredDegree[degree] == 'Master’s degree (300 ECTS)':
                    desiredDegree[degree] = 2
                elif desiredDegree[degree] == 'Bachelor’s degree (180 ECTS)':
                    desiredDegree[degree] = 1
            
        else:
            desiredDegree = []

        #missing Industries: strategy, consumer goods
        if 'industry' in prof:
            industry: list= prof['industry'] 
            industryResult = set()
            
            for ind in range(len(industry)):
                industrySet.add(industry[ind])
                if industry[ind] == 'ElectricityEnergyPower' or 'Electricity' or 'Energy & power' or 'El, Energi och kraft':
                    industryResult.add(1)
                if industry[ind] == 'Environment' or 'renewable energy' or 'miljö' or 'Life Science' or 'water':
                    industryResult.add(2)
                if industry[ind] == 'Banking, Finance' or 'Investering' or 'Bank och finans':
                    industryResult.add(3)
                if industry[ind] == 'Union' or 'Fackförbund':
                    industryResult.add(4)
                if industry[ind] == 'Investment':
                    industryResult.add(5)
                if industry[ind] == 'Insurance' or 'Försäkring':
                    industryResult.add(6)
                if industry[ind] == 'Recruitment':
                    industryResult.add(7)
                if industry[ind] == 'Construction' or 'Vägledning' or 'Bygg' or 'Fastigheter & Infrastruktur' or 'Property & Infrastructure':
                    industryResult.add(8)
                if industry[ind] == 'Architecture' or 'Arkitektur och Grafisk design':
                    industryResult.add(9)
                if industry[ind] == 'GraphicDesign' or 'Arkitektur och Grafisk design':
                    industryResult.add(10)
                if industry[ind] == 'DataIT' or 'Data and IT' or 'Data' or 'information science' or 'it':
                    industryResult.add(11)
                if industry[ind] == 'FinanceConsultancy':
                    industryResult.add(12)
                if industry[ind] == 'Telecommunication' or 'Telekommunikation':
                    industryResult.add(13)
                if industry[ind] == 'Consulting' or 'Bemanning & Arbetsförmedling' or 'Ekonomi och konsultverksamhet' or 'Konsultverksamhet':
                    industryResult.add(14)
                if industry[ind] == 'Management':
                    industryResult.add(15)
                if industry[ind] == 'Media':
                    industryResult.add(16)
                if industry[ind] == 'Industry' or 'Industri':
                    industryResult.add(17)
                if industry[ind] == 'NuclearPower' or 'Kärnkraft':
                    industryResult.add(18)
                if industry[ind] == 'LifeScience':
                    industryResult.add(19)
                if industry[ind] == 'MedialTechniques' or 'Medicinteknik':
                    industryResult.add(20)
                if industry[ind] == 'PropertyInfrastructure':
                    industryResult.add(21)
                if industry[ind] == 'Research' or 'Forskning':
                    industryResult.add(22)
                if industry[ind] == 'Coaching':
                    industryResult.add(23)

        else:
            industry = ""

        if 'desiredProgramme' in prof:
            desiredProgramme: list = prof['desiredProgramme']
            desiredProgrammeResult = set()
            for programme in range(len(desiredProgramme)):              
                if desiredProgramme[programme] == 'Brandingenjörsutbildning':
                    desiredProgrammeResult.add(8)
                elif desiredProgramme[programme] == 'Maskinteknik med teknisk design':
                    desiredProgrammeResult.add(7)
                elif desiredProgramme[programme] == 'Elektroteknik':
                    desiredProgrammeResult.add(3)
                elif desiredProgramme[programme] == 'Ekosystemteknik':
                    desiredProgrammeResult.add(9)
                elif desiredProgramme[programme] == 'Maskinteknik':
                    desiredProgrammeResult.add(7)
                elif desiredProgramme[programme] == 'TekniskNanovetenskap':
                    desiredProgrammeResult.add(4)
                elif desiredProgramme[programme] == 'Bioteknik':
                    desiredProgrammeResult.add(6)
                elif desiredProgramme[programme] == 'Industridesign':
                    desiredProgrammeResult.add(1)
                elif desiredProgramme[programme] == 'Arkitekt':
                    desiredProgrammeResult.add(1)
                elif desiredProgramme[programme] == 'Informations- och kommunikationsteknik':
                    desiredProgrammeResult.add(2)
                elif desiredProgramme[programme] == 'Kemiteknik':
                    desiredProgrammeResult.add(6)
                elif desiredProgramme[programme] == 'Byggteknik med järnvägsteknik':
                    desiredProgrammeResult.add(5)
                elif desiredProgramme[programme] == 'Väg- och vatttenbyggnad':
                    desiredProgrammeResult.add(8)
                elif desiredProgramme[programme] == 'Byggteknik med arkitektur':
                    desiredProgrammeResult.add(5)
                elif desiredProgramme[programme] == 'Industriell ekonomi':
                    desiredProgrammeResult.add(5)
                elif desiredProgramme[programme] == 'Teknisk Matematik':
                    desiredProgrammeResult.add(4)
                elif desiredProgramme[programme] == 'Medicinteknik':
                    desiredProgrammeResult.add(3)
                elif desiredProgramme[programme] == 'Lantmäteri':
                    desiredProgrammeResult.add(8)
                elif desiredProgramme[programme] == 'Datateknik':
                    desiredProgrammeResult.add(2)
                elif desiredProgramme[programme] == 'Teknisk Fysik':
                    desiredProgrammeResult.add(4)
                elif desiredProgramme[programme] == 'Byggteknik med väg- och trafikteknik':
                    desiredProgrammeResult.add(5)

        else:
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

        r = requests.put(url, data=data.encode('utf-8'), headers=headers)
        print(r)
        #print(r.content)
        
