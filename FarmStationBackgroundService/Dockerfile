#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY FarmStationBackgroundService/FarmStationBackgroundService.csproj /src/FarmStationBackgroundService/
RUN dotnet restore FarmStationBackgroundService/FarmStationBackgroundService.csproj
COPY . .
WORKDIR /src
RUN dotnet build "FarmStationBackgroundService/FarmStationBackgroundService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FarmStationBackgroundService/FarmStationBackgroundService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FarmStationBackgroundService.dll"]

#entrypoint ["/bin/sh", "-c", "'while :; do sleep 2h & wait ${!}; done;'"] 
#'while :; do sleep 2min; dotnet FarmStationMessenger.dll; & wait $${!}; done;'"] 