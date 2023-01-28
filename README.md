# Infrastructure

## Setting up docker network

```
docker network create --attachable -d bridge cqrs-network
```

## Installing Apache Kafka
In this part we will install Kafka using <code>docker compose</code> that contains 2 services: <code>zookeeper</code> and <code>kafka</code>.

Kafka broker is stateless. Apache Zookerper is responsible for managing Kafka cluster and also electing the lead broker. In this example we will only a single broker.

```
docker-compose up -d -f kafka-zookeeper.yml
```

Check the kafka and zookeeper that should be running in docker.

```
docker ps
```

## Installing MongoDB

```
docker container run -it -d --name mongodb -p 27017:27017 --network cqrs-network --restart always -v $HOME/docker/volume/mongo_data:/data/db mongo:latest
```

It can also be included in the docker compose.

## Installing SQL Server 
```
docker run -d --name sql-container \
--network cqrs-network \
--restart always \
-e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=$tr0ngS@P@ssw0rd02' -e 'MSSQL_PID=Express' \
-p 1433:1433 mcr.microsoft.com/mssql/server:2017-latest-ubuntu 
```

# Project Setup

```
dotnet new sln --name Social.CQRS
```

```
dotnet new classlib -o CQRS.Core
```

```
dotnet new classlib -o Social.Command.Domain
dotnet new classlib -o Social.Command.Infra
dotnet new webapi -o Social.Command.Api
```

```
dotnet new classlib -o Social.Query.Domain
dotnet new classlib -o Social.Query.Infra
dotnet new webapi -o Social.Query.Api
```

```
dotnet sln Social.sln add CQRS.Core/CQRS.Core.csproj

dotnet sln Social.sln add Social.Command/Social.Command.Api/Social.Command.Api.csproj
dotnet sln Social.sln add Social.Command/Social.Command.Domain/Social.Command.Domain.csproj
dotnet sln Social.sln add Social.Command/Social.Command.Infra/Social.Command.Infra.csproj

dotnet sln Social.sln add Social.Query/Social.Query.Api/Social.Command.Api.csproj
dotnet sln Social.sln add Social.Query/Social.Query.Domain/Social.Command.Domain.csproj
dotnet sln Social.sln add Social.Query/Social.Query.Infra/Social.Command.Infra.csproj
```

```
dotnet add Social.Command/Social.Command.Api/Social.Command.Api.csproj reference CQRS.Core/CQRS.Core.csproj
```
# Reference
1. https://hub.docker.com/_/microsoft-mssql-server