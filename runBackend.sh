# This file needs to be run with sudo

# Create an alias for this file. Is then run with "git run"
# git config --local alias.run '!sudo ./runBackend.sh'

checkInstalled() {
    if ! [ -x "$(command -v $@)" ]; then
        echo "Error: $@ is not installed." >&2
        exit 1
    else
        echo "$@ is installed"
    fi
}

show_help() {
    echo "Usage: ./runBackend.sh [OPTIONS]"
    echo "Likely needs to be run with sudo"
    echo "Script for running the backend"
    echo ""
    echo "Options:"
    echo "-h, --help     Display this help message and exit"
    echo "-s, --standalone   Make a standalone docker run. Does not use docker-compose."
    echo "-q, --quick    Create an alias for this file. Is then run with \"git run\""
    echo ""
}



while getopts ":h:q:s" opt; do
    case $opt in
    h|help)
        show_help
        exit 1
        ;;
    q|quick)
        # Create an alias for this file. Is then run with "git run"
        git config core.filemode false
        git config --local alias.run '!sh ./runBackend.sh'
        exit 1
        ;;
    s|standalone)
        # Make a standalone docker run. Does not use docker-compose.
        # Less relaiable, but an alternative
        sudo docker rm -f nexpo_database
        sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
        dotnet run --project Nexpo
        exit 1
        ;;
    esac
done

checkInstalled docker
checkInstalled dotnet

sudo docker-compose up -d

