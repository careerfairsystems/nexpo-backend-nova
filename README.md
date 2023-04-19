# nexpo-backend-nova
A rebuild of the nexpo backend with C#, currently in use for the ARKAD career fair as of 2023.

This is the backend that supports the [Nexpo app](https://github.com/careerfairsystems/nexpo-app) and in the future hopefully the new nexpo website.

# Introduction
## Project Navigation: 

[Google Drive](https://drive.google.com/drive/u/4/folders/1jfxYhCdO21Vysh60JyGSvvSZEn65fIn5): Stores meeting protocols and image source files of the figures used in the wiki. 
(Requires a tlth google account to access) 

[Trello Board](https://trello.com/b/Mo2cpo31/backend): Provides an overview of all issues, their progress and who's responsible. 
(Requires access)


## Getting started
1. Download the required development tools 
    1. Download Docker
    2. Download the .NET SDK
2. Start the backend, using the method that fits you best
* see [Setup development environment](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment) for options and tutorials
3. Start developing! See [System Overview](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/2.-System-Overview) for additional help



## Table of Contents
- [Introduction](#introduction)
  - [Project Navigation:](#project-navigation)
  - [Getting started](#getting-started)
  - [Table of Contents](#table-of-contents)
  
---


- [Setup development environment](#setup-development-environment)
  - [**runBackend.sh**](#runbackendsh)
    - [Common causes of errors with ./runBackend.sh](#common-causes-of-errors-with-runbackendsh)
  - [Stand Alone](#stand-alone)
  - [Docker \& Visual Studio](#docker--visual-studio)
  
---

- [System Overview](#system-overview)
  - [Overall Architecture](#overall-architecture)
  - [Relations between Model tables](#relations-between-model-tables)
    - [ER-Diagram](#er-diagram)
    - [Relations](#relations)
  
---

- [Testing and developing tools](#testing-and-developing-tools)
  - [Setup Test Environment](#setup-test-environment)
  
---

- [Setting up production server](#setting-up-production-server)
  - [View endpoints in swagger](#view-endpoints-in-swagger)
  - [Install docker on amazon linux 2 (a lot of issues)](#install-docker-on-amazon-linux-2-a-lot-of-issues)
  
---

- [Update Database models](#update-database-models)
  - [Changing or adding a Model](#changing-or-adding-a-model)
  - [Create backup of the db](#create-backup-of-the-db)
  - [Restore db from backup](#restore-db-from-backup)
  - [Generate migration-script](#generate-migration-script)
  - [Apply migration to db](#apply-migration-to-db)
  
---


**Small footnote to whoever is updating this table of contents:
-  get "All in one markdown -> ctrl + shift + p -> update table of contents**


# Setup development environment

The main configuration methods for setting up the development environment   
 * Choose a method that suits you best:


1. [runBackend.sh](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#runbackendsh)
   * (Recommended for Unix systems - also works for WSL)
2. [Docker & Visual Studio](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#docker--visual-studio)  
   * (Recommended for Unix systems)
3. [Stand Alone](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#stand-alone)
    * (Recommended for Unix systems, if the the bash script does not work - for example due to issues with WSL)
    * (no Docker, and you can choose any IDE you like, for example Visual Studio Code) 

## **runBackend.sh**
* Recommended for Unix systems
1. **Install Docker**  

2. **Install .NET SDK**  
   1. It is available [here](https://dotnet.microsoft.com/download)
3. **Give the script execution permissions**  
   1. Run the following command to give the script execution permissions:
 ```
 chmod +x runBackend.sh
 ```
1. **Run the script:**
```
sudo ./runBackend.sh
```

### Common causes of errors with ./runBackend.sh
(Note, much of these solutions are also relevant to the standalone solution
* You are not using `sudo` in front of `sudo ./runBackend.sh`
    * Alternativly you are not using `sudo` in front of docker commands
* Dotnet or Docker is not installed (correctly)
    * To test this, try if the following commands give a version
```
docker -v
dotnet --version
```

* The configured dotnet version of the backend is not the same as your downloaded dotnet version
    * A quick way to temporarily fix this is to simply change what version the backend uses:
    1. Run the following command and note your dotnet version:
    ```
    dotnet --version
    ``` 

    2. Change the TargetFramework in Nexpo/Nexpo.csproj to match your dotnet version
        * For example if you have dotnet version 7.0.105, change to dotnet 7 in Nexpo/Nexpo.csproj, meaning replace with the following line:

    ```
    <TargetFramework>net7.0</TargetFramework>
    ```
    3. WSL1 causes some complications with docker. Use the following command to test your WSL version, and update if you still can not get Docker to work
    ```
    wsl -v
    ```
* If none if the above fixes the problem try making an standalone launch using the following command:
    ```
    sudo ./runBackend -s
    ```
    <summary>Help regarding runBackend.sh</summary>
    <div class="markdown">
The command ./runBackend -h is useful for receiving help regarding the bash script.
    </div>
</details>

## Stand Alone

1. **Install Docker Desktop**  
   1. You can get it for Windows and Mac [here](https://www.docker.com/products/docker-desktop/)
2. **Containerize the database, by running:**
```
sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
```

<details>
    <summary>Click to Expand for additional information</summary>
    <div class="markdown">
       1. Note: On windows, simply skip using sudo <br> 
       2. The default development database connection tries to access the database nexpo on localhost:5432 using the credentials nexpo:nexpo. The nexpo user of course needs the correct permissions on the database and if you change anything in the setup make sure to update the connection string as well. <br> 
       3. It will pull down the correct PostgreSQL server and set it up as we want it. Keep in mind though that no persistent volume is added to the container so don't do this in production.
    </div>
</details>

3. **Install .NET SDK**  
   1. It is available [here](https://dotnet.microsoft.com/download)
4. **Run the backend**  
    1. Run the following command to run:
 ```
 dotnet run --project Nexpo
 ```
<details>
    <summary>Click to Expand for additional information</summary>
    <div class="markdown">
       To avoid having to specify the project every time, you can also: <br>
         1. Change directories to the `Nexpo` project folder. <br>
         2. run `dotnet run` (without the `--project Nexpo` flag)
    </div>
</details>

<details>
    <summary>How to visualize DBeaver</summary>
    <div class="markdown">
       1. Run previous command for running database in docker without Docker Desktop.<br>
       2. Download DBeaver https://dbeaver.io/download/ <br>
       3. Connect to open postgres connection with settings Postgresql and default 5432 default port. db_name, user, password as nexpo <br>
    </div>
</details>

## Docker & Visual Studio 
* Recommended for Windows

1. **Install Docker Desktop**  
   1. You can get it for Windows and Mac [here](https://www.docker.com/products/docker-desktop/)
2. **Install Visual Studio**   
   1. Use the Community Edition if you don't have a license. During installation, select the "ASP.NET and web development" toolbox when given the choice to enable good support for Docker and the application.
3. **Open the solution (the code) in Visual Studio**
    1. Open Visual Studio and open the solution file `Nexpo.sln`.
   
4. **Make sure the `docker-compose` "project" is selected as the startup project**
    1. If not already selected as the startup project, right click `docker-compose` in the Solution Explorer and select "Set as Startup Project".
5. **Run the backend**
    1. A database with automatically be created





   
----

# System Overview
## Overall Architecture

The overall architecture can currently be split into 6 components with different responsibilities. They are as follows: 

**1. Controller:** Receives and responds to http-requests by calling on appropriate methods in the other components to generate the desired outcome. To control the format of the input and output, may requests and db responses be converted to DTO:s before being forwarded to repositories or sent as a response.   

**2. Repository:** Responsible for translating requests into queries against the model and converting query results to relevant data objects before returning them. 

**3. Model:** C# representation of the the database tables. 

**4. DTO:** Data Transfer Object that converts data to an object consisting of only relevant data. Can be used to prevent data leakage in http-responses or as an simplified method of moving data between different components.  

**5. Services:** Responsible for functionality outside the manipulation and gathering of data in the database. This entails token & file management, password validation and email services.

**6. Helpers:** Consists of helper functions for the controller. Currently only converts claims to intelligible data. 

![Overall_Architecture drawio (4)](https://user-images.githubusercontent.com/47223000/191040681-7c0c9409-48ca-4187-8ee0-023d1d1fc913.png)

**More in-depth resources:**

[Architecting-Modern-Web-Applications-with-ASP.NET-Core-and-Azure.pdf](https://github.com/careerfairsystems/nexpo-backend-nova/files/9549955/Architecting-Modern-Web-Applications-with-ASP.NET-Core-and-Azure.pdf)

[NET-Microservices-Architecture-for-Containerized-NET-Applications.pdf](https://github.com/careerfairsystems/nexpo-backend-nova/files/9549957/NET-Microservices-Architecture-for-Containerized-NET-Applications.pdf)

## Relations between Model tables

### ER-Diagram
![ER-backend drawio (2)](https://user-images.githubusercontent.com/47223000/208705255-17f30d38-a4d5-4702-bd65-6c4a2a48e657.png)

### Relations
Current relations between the tables in the model: 

> User(**Id**, Email, PasswordHash, Role, FirstName, LastName, PhoneNr, FoodPreferences, hasProfilePicture, hasCV, ProfilePictureUrl, _CompanyId_)

> Student(**Id**, Programme, ResumeEnUrl, ResumeSvUrl, LinkedIn, MasterTitle, Year, _UserId_)

> Company(**Id**, Name, Description, DidYouKnow, LogoUrl, Website, HostName, HostEmail, HostPhone, DesiredDegrees, DesiredProgramme, Positions, Industries)

> StudentSessionTimeslot(**Id**, Start, End, Location, _StudentId_, _CompanyId_)

> StudentSessionApplication(**Id**, Motivation, Status, Booked, _StudentId_, _CompanyId_)

> Event(**Id**, Name, Description, Date, Start, End, Location, Host, Language, Capacity, TicketCount)

> Ticket(**Id**, Code, PhotoOk, isConsumed, _EventId_, _UserId_)



---

# Testing and developing tools

## Setup Test Environment

No matter the chosen setup method, it´s required to start an external database server before running the tests for them to pass. This is due to some tests utilizing black-box testing through testing against the controllers. It may take a while for the container to populate the tables with the example data, so if most controller-tests fail during the first run try to run them again.

**NOTE:** Running docker-compose in Visual Studio and then the tests do for some reason not work.
To run the tests:  

  1.  
```
sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
```
  2.  
```
sudo dotnet test Nexpo.Tests/
```

# Setting up production server

## View endpoints in swagger
Swagger allows you to see the specifications of an API, including the endpoints, request parameters, response formats, and authentication methods. It provides an user interface for interacting with the API and testing its functionality. With Swagger, you can visualize and document your API in a standardized way, making it easier for developers to understand and use your API.

To open the application in swagger:
1. Start the backend
2. Go to [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

## Install docker on amazon linux 2 (a lot of issues)
![fc51af68f30228cf2bc666bbe4496087](https://user-images.githubusercontent.com/66833685/195836015-1fa2bb35-aa31-4240-8869-6dd39415b43f.png)

port 80 being used...
service nginx stop





# Update Database models

The classes stored in the `Models` directory are the skeleton for the database. We use something called Code First to define our database structure in code relationships and then generate the database modifications automatically.

## Changing or adding a Model

If you need to change a Model (adding a new field for example) or create a new one it is as simple as creating a new class in the Models directory. If you are adding a new Model you need to add the supporting classes (the repositories) as well and add the correct bindings that make them a new table. Take a look at how the others are connected, it should only be about two lines of code in ApplicationDbContext to add it as a table and then in Startup to add the repository to the dependency injector.

After the code changes you have to generate the database update code, the so called **Migration**. A migration is a small script that transforms the database from the old state to the new to match what you added or removed in the code. For example, if you added a field to a Model, the transformation will generate the necessary SQL to alter the table to include the new column. Migrations can also be reverted since they contain the information to undo the changes. To create a new migration for your changes, run the following command:

```shell
dotnet ef migrations add <a descriptive name for your changes>
```

After the migration is created you can take a look at the generated code to see if it is correct or if you need to make some changes (to the Model, don't edit the generated migration). The migration is applied on development server startup but if you want to apply it manually you can run:

```shell
dotnet ef database update
```

## Create backup of the db
To create a backup dump of the Postgres database, run the following shell command: 
```shell
docker exec -t name_of_db_container pg_dumpall -c -U nexpo > name_of_dump.sql
```

**NOTE:** Store the dump file on a secure and private instance as it contains sensitive data about the content of the db. 

## Restore db from backup
To restore the db from a backup dump, run the following shell command: 
```shell
cat name_of_dump.sql | docker exec -i name_of_db_container psql -U nexpo
```

## Generate migration-script
To generate a script that applies generated migrations to the db, run the following shell command: 
```shell
dotnet ef migrations script --idempotent > name_of_migration.sql
```

**NOTE**: Remove the two first lines in the generated script as they are simply just output from the build-process. 

## Apply migration to db
To apply the migration to the production db, use the generated script and run the following shell command: 
```shell
cat name_of_migration.sql | docker exec -i name_of_db_container psql -U nexpo
```



