#!/bin/bash

if [ "$1" = "-reset" ]; then
    # Reset the database before running the tests
    sudo docker rm -f nexpo_database
    sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
fi

sudo dotnet test Nexpo.Tests/
