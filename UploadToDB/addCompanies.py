import json
from re import I
from typing import Dict
import pandas as pd


with open('ArkadIndent2.json', encoding="utf-8") as d:
    dictData = json.load(d)




df = pd.DataFrame(dictData)


for row in range(len(df)):
    
   
    pr = df.iloc[row]
    prof = pr['profile']
 
    
    if type(prof) == dict:
    
        if 'name' in prof:
            name = prof['name']
        else: name = ""

        if 'weOffer' in prof:
            weOffer = prof['weOffer']
        else:
            weOffer = ""

        if 'desiredDegree' in prof:
            desiredDegree = prof['desiredDegree']
        else:
            desiredDegree = ""

        if 'industry' in prof:
            industry = prof['industry']
        else:
            industry = ""

        if 'desiredProgramme' in prof:
            desiredProgramme = prof['desiredProgramme']
        else:
            desiredProgramme  = ""

        if 'didYouKnow' in prof:
            didYouKnow = prof['didYouKnow']
        else:
            didYouKnow = ""

        if 'aboutUs' in prof:
            aboutUs = prof['aboutUs']
        else:
            aboutUs = ""

        if 'urlWebsite' in prof:
            urlWebsite = prof['urlWebsite']
        else:
            urlWebsite = ""

        if 'logotype' in prof and prof['logotype'] != None:

            pictureFileName = prof['logotype']['name']
            pictureFileName = pictureFileName.replace('eps','jpg')

        else:
            pictureFileName = ""

        print(weOffer,"\n",  name,"\n",  desiredDegree,"\n",  industry,"\n",  desiredProgramme,"\n",  didYouKnow,"\n",  aboutUs,"\n",  urlWebsite, "\n", pictureFileName)
  
        


