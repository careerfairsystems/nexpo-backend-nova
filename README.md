# nexpo-backend-nova
A rebuild of the nexpo backend with C#, currently in use for the ARKAD career fair as of 2023.

This is the backend that supports the [Nexpo app](https://github.com/careerfairsystems/nexpo-app) and in the future hopefully the new nexpo website.

## Project Navigation: 

[Wiki](https://github.com/careerfairsystems/nexpo-backend-nova/wiki): Contains guides for the setup of the development environment, as well as, documentation of the overall system architecture and the database tables' relations. 
(Accessible through the 'Wiki' tab and is open to the public) 

[Google Drive](https://drive.google.com/drive/u/4/folders/1jfxYhCdO21Vysh60JyGSvvSZEn65fIn5): Stores meeting protocols and image source files of the figures used in the wiki. 
(Requires a tlth google account to access) 

[Trello Board](https://trello.com/b/Mo2cpo31/backend): Provides an overview of all issues, their progress and who's responsible. 
(Requires access)

Welcome to the nexpo-backend-nova wiki!

This wiki serves as a documentation portal, where the whole Nexpo system will be described.

# Getting started
1. Download the required development tools 
    1. Download Docker
    2. Download the .NET SDK
2. Start the backend, using the method that fits you best
* see [Setup development environment](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment) for options and tutorials
3. Start developing! See [System Overview](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/2.-System-Overview) for additional help

# Table of Contents
0. [**Introduction**](https://github.com/careerfairsystems/nexpo-backend-nova/wiki)  
    1. [Abstract](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/0.-Introduction#abstract)
    2. [Table of Contents](https://github.com/careerfairsystems/nexpo-backend-nova/wiki#table-of-contents)
1. [**Setup development environment**](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Setup-development-environment)
    1. [runBackend.sh](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#runbackendsh)
        1. [Common causes of errors with ./runBackend.sh](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#common-causes-of-errors-with-runbackendsh)
    2. [Stand Alone](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#stand-alone)
    3. [Docker & Visual Studio](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#docker--visual-studio)
    4. [Setup Test Environment](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/1.-Setup-development-environment#setup-test-environment)


2. [System Overview](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/2.-System-Overview) for additional 
    1. [Overall Architecture](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/System-Overview#overall-architecture)
    2. [Relations between Model tables](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/System-Overview#relations-between-model-tables)
        1. [ER-Diagram](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/System-Overview#er-diagram)
        2. [Relations](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/System-Overview#relations)

3. [**Update Database models**](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Update-Database-models)
    1. [Changing or adding a Model](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Update-Database-models#changing-or-adding-a-model)
    2. [Create backup of the db](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Update-Database-models#create-backup-of-the-db)
    3. [Restore db from backup](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Update-Database-models#restore-db-from-backup)
    4. [Generate migration-script](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Update-Database-models#generate-migration-script)
    5. [Apply migration to db](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Update-Database-models#apply-migration-to-db)

4. [**Setting up production server**](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/5.-Setting-up-production-server)
    1. [Install docker on amazon linux 2 (a lot of issues)](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/Setting-up-production-server#install-docker-on-amazon-linux-2-a-lot-of-issues)

5. [**SSO LU login feature**](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/SSO-LU-login-feature)
    1. [Background](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/6.-SSO-LU-login-feature#background)
    2. [Frontend](https://github.com/careerfairsystems/nexpo-backend-nova/wiki/5.-SSO-LU-login-feature#frontend)



