#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nexpo/Nexpo.csproj", "Nexpo/"]
# Below is needed due to build issues that arrise when restoring on some types of hardware on .NET 7.0.
ENV DOTNET_EnableWriteXorExecute=0
RUN dotnet restore "Nexpo/Nexpo.csproj"
COPY . .
WORKDIR "/src/Nexpo"
RUN dotnet build "Nexpo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexpo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexpo.dll"]
