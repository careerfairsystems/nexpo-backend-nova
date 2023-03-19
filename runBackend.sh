checkInstalled() {
    if ! [ -x "$(command -v $@)" ]; then
        echo "Error: $@ is not installed." >&2
        exit 1
    else
        echo "$@ is installed"
    fi
}

checkInstalled docker
checkInstalled dotnet

if(docker ps -a | grep nexpo_database); then
    docker rm -f nexpo_database
fi
sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
dotnet run --project Nexpo
# Första gången: Kör denna fil
# Sedan: Kör endast dotnet run --project Nexpo
