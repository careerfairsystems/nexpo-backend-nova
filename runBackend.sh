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

while getopts ":h:q" opt; do
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
    esac
done

checkInstalled docker
checkInstalled dotnet

CONTAINER_NAME=nexpo_database



if docker ps -a --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}\$"; then
    echo "The container already exists"
else
    sudo docker run -d --name $CONTAINER_NAME -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
    echo "Created the container"
fi

if docker container inspect -f '{{.State.Running}}' "$CONTAINER_NAME" >/dev/null 2>&1; then
    echo "The container is already running"
else
    sudo docker start "$CONTAINER_NAME"
    echo "Started the container"
fi



dotnet run --project Nexpo
