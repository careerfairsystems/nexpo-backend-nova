if(docker ps -a | grep nexpo_database); then
    docker rm -f nexpo_database
fi
sudo docker run -d --name nexpo_database -p 5432:5432 -e POSTGRES_USER=nexpo -e POSTGRES_PASSWORD=nexpo postgres:14
dotnet run --project Nexpo
