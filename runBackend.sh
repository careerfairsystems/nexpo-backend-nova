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
    echo "-d, --redocker   Delete the docker container and recreate it"
    echo "-q, --quick    Create an alias for this file. Is then run with \"git run\""
    echo ""
}

checkInstalled docker
checkInstalled dotnet

while getopts ":h:d:q" opt; do
    case $opt in
    h|help)
        show_help
        exit 1
        ;;
    d|redocker)
        # Forcably deleted (and later reinstalls) the docker container
        sudo docker rm -f nexpo_database
        ;;
    q|quick)
        # Create an alias for this file. Is then run with "git run"
        git config core.filemode false
        git config --local alias.run '!sudo ./runBackend.sh'
        exit 1
        ;;
    esac
done

if(!(docker ps -a | grep nexpo_database)); then
    sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
fi

dotnet run --project Nexpo
