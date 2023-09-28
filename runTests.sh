#!/bin/bash

if [ "$1" = "-start" ]; then
    # Check if the nexpo_database container exists
    if sudo docker ps -a --format '{{.Names}}' | grep -q "^nexpo_database$"; then
        # Reset the database before running the tests
        sudo docker rm -f nexpo_database
    fi
    sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
elif [ "$1" = "-run" ]; then
    # Check if the nexpo_database container is running
    if [ "$(sudo docker ps -q -f name=nexpo_database)" ]; then
        if [ "$2" ]; then
            # Check if the test class ends with "ControllerTest"
            if [[ "$2" == *ControllerTest ]]; then
                # Run the specific test class
                sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$2"
            elif [[ "$2" == *Controller ]]; then
                # Append "Test" to the test class and run
                test_class="${2}Test"
                sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$test_class"
            else
                # Append "ControllerTest" to the test class and run
                test_class="$2ControllerTest"
                sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$test_class"
            fi
        else
            # Run all tests
            sudo dotnet test Nexpo.Tests/
        fi
    else
        # Prompt the user to start the database
        read -p "Warning: nexpo_database container is not running. Do you want to start it? (y/n): " choice
        case "$choice" in
            y|Y )
                # Start the database container
                sudo docker start nexpo_database

                if [ "$2" ]; then
                    # Check if the test class ends with "ControllerTest"
                    if [[ "$2" == *ControllerTest ]]; then
                        # Run the specific test class
                        sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$2"
                    elif [[ "$2" == *Controller ]]; then
                        # Append "Test" to the test class and run
                        test_class="${2}Test"
                        sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$test_class"
                    else
                        # Append "ControllerTest" to the test class and run
                        test_class="$2ControllerTest"
                        sudo dotnet test Nexpo.Tests/ --filter "FullyQualifiedName~Nexpo.Tests.Controllers.$test_class"
                    fi
                else
                    # Run all tests
                    sudo dotnet test Nexpo.Tests/
                fi
                ;;
            n|N ) echo "Aborted.";;
            * ) echo "Invalid choice. Aborted.";;
        esac
    fi
elif [ "$1" = "-help" ]; then
    # Print help message
    echo "Usage: ./test.sh [OPTIONS]"
    echo "Note: Mandatory to use options "
    echo ""
    echo "Options:"
    echo "  -start            Reset the database before running tests"
    echo "                    This needs to be done when the database seeding has changed"
    echo "                    The first time this is done, the tests will fail"
    echo "                    Ergo, run needs to be run twice"
    echo ""
    echo "  Run twice after -start:"
    echo "  -run Run all tests"
    echo "  -run <class_name> Run the specified test class"
    echo "  -run <controller_name> Run the test class for the specified controller"
    echo "  -run <name>       Run the test class for the specified controller"
    echo ""
    echo "  -help             Show this help message"
else
    # Print error message
    echo "Error: no valid option provided."
    echo "Note: Mandatory to use options"
    echo ""
    echo "Usage: ./test.sh [OPTIONS]"
    echo "Options:"
    echo "  -start            Reset the database before running tests"
    echo "                    This needs to be done when the database seeding has changed"
    echo "                    The first time this is done, the tests will fail"
    echo "  -run <class_name> Run the specified test class"
    echo "  -help             Get help"
    exit 1
fi