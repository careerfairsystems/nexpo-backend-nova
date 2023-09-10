# Script for Uploading to Database

## Background
### Explaination
One option for populating the database is to use python scripts to make http requests to the the available endpoints. This folder contains such scripts. 


## Running
### Preparing the scripts
1. Enter an admin account to the login.py file, if it is not already entered
    * **DO NOT ACCIDENTLY UPLOAD THIS TO GITHUB**
    * This account should be available in the Bitwarden, owned by the Head of IT of ARKAD


2. If needed, create a JSON script that will be used as input to the script. For creating companies, export the information from Jexpo to JSON, and use as input. Enter the path to this file to the script. 

3. If needed, enter the S3 bucket URL. For example: https://nexpo-bucket.s3.eu-north-1.amazonaws.com/


### Running the scripts
1. Download python
2. Run the script from your terminal

Note that, in the current state, some preprations need to be made before runnings the scripts. See **Preparing the scripts**


## Localhost VS Production Database
Note that these scripts works for populating both the actual database, but can also be tested in localhost. The only diffrence is the api used, for example: 
```
http://{localhost}/api/timeslots/add
```

vs

```
https://www.nexpo.arkadtlth.se/api/timeslots/add
```
### Running the scripts on Localhost
1. In the login.py script, change the log in to an admin account in the seeded database
    - See ApplicationDbContext for such accounts

2. Change the adress in login.py
    - Given you are using port 5000 the adress will be `http://localhost:5000/api/session/signin`
3. Change the adress in the script you are using (given that the script calls an endpoint)
    - http://localhost:5000/api/your/adress/here
4. Start the backend
5. Start the frontend
6. Run the script
- 
