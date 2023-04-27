if [ "$1" = "-start" ]; then
    # Reset the database before running the tests
    sudo docker rm -f nexpo_database
    sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
elif [ "$1" = "-run" ]; then
    # Check if the nexpo_database container is running
    if [ "$(sudo docker ps -q -f name=nexpo_database)" ]; then
        # Run the tests
        sudo dotnet test Nexpo.Tests/
    else
        # Ask user if they want to continue without starting the database
        read -p "Warning: nexpo_database container is not running. It is reccomended to do run ./runTests -start first. Do you want to continue without starting it? (y/n): " choice
        case "$choice" in
            y|Y ) sudo dotnet test Nexpo.Tests/;;
            n|N ) echo "Aborted.";;
            * ) echo "Invalid choice. Aborted.";;
        esac
    fi
elif [ "$1" = "-help" ]; then
    # Print help message
    echo "Usage: ./test.sh [OPTIONS]"
    echo "Note: Mandatory to use options"
    echo "Options:"
    echo "  -start    Reset the database before running tests"
    echo "            This needs to be done when the database seeding has changed"
    echo "            The first time this is done, the tests will fail"
    echo "  -run      Run the tests"
    echo "  -help     Show this help message"
else
    # Print error message
    echo "Error: no option provided."
    echo "Usage: ./test.sh [OPTIONS]"
    echo "Options:"
    echo "  -start    Reset the database before running tests"
    echo "            This needs to be done when the database seeding has changed"
    echo "            The first time this is done, the tests will fail"
    echo "  -run      Run the tests"
    echo "  -help     Get help"
    exit 1
fi