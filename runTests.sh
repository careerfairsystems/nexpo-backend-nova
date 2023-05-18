#!/bin/bash

if [ "$1" = "-start" ]; then
    # Reset the database before running the tests
    sudo docker rm -f nexpo_database
    sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
elif [ "$1" = "-run" ]; then
    if [ "$2" ]; then
        # Check if the nexpo_database container is running
        if [ "$(sudo docker ps -q -f name=nexpo_database)" ]; then
            # Run the specific test class
            # Notera att man måste ta classen - inte fillnamnet. Improve
            sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$2"
        else
            # Ask user if they want to continue without starting the database
            read -p "Warning: nexpo_database container is not running. It is recommended to run ./test.sh -start first. Do you want to continue without starting it? (y/n): " choice
            case "$choice" in
                y|Y ) sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$2";;
                n|N ) echo "Aborted.";;
                * ) echo "Invalid choice. Aborted.";;
            esac
        fi
    else
        echo "Error: No test class provided. Please provide the test class name."
        echo "Usage: ./test.sh -run <test_class_name>"
        exit 1
    fi
elif [ "$1" = "-help" ]; then
    # Print help message
    echo "Usage: ./test.sh [OPTIONS]"
    echo "Note: Mandatory to use options"
    echo "Options:"
    echo "  -start            Reset the database before running tests"
    echo "                    This needs to be done when the database seeding has changed"
    echo "                    The first time this is done, the tests will fail"
    echo "  -run <class_name> Run the specified test class"
    echo "  -help             Show this help message"
else
    # Print error message
    echo "Error: no option provided."
    echo "Usage: ./test.sh [OPTIONS]"
    echo "Options:"
    echo "  -start            Reset the database before running tests"
    echo "                    This needs to be done when the database seeding has changed"
    echo "                    The first time this is done, the tests will fail"
    echo "  -run <class_name> Run the specified test class"
    echo "  -help             Get help"
    exit 1
fi
