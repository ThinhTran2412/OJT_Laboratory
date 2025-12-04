# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.config first (for local package source)
COPY ["WareHouse_Service/NuGet.config", "./"]

# Copy local nuget packages (create directory first if it doesn't exist)
RUN mkdir -p ./nuget
COPY ["WareHouse_Service/nuget/", "./nuget/"]

# Copy csproj files for better layer caching
COPY ["WareHouse_Service/WareHouse_Service.API/WareHouse_Service.API.csproj", "WareHouse_Service/WareHouse_Service.API/"]
COPY ["WareHouse_Service/WareHouse_Service.Application/WareHouse_Service.Application.csproj", "WareHouse_Service/WareHouse_Service.Application/"]
COPY ["WareHouse_Service/WareHouse_Service.Domain/WareHouse_Service.Domain.csproj", "WareHouse_Service/WareHouse_Service.Domain/"]
COPY ["WareHouse_Service/WareHouse_Service.Infrastructure/WareHouse_Service.Infrastructure.csproj", "WareHouse_Service/WareHouse_Service.Infrastructure/"]

# Copy proto files from Simulator_Service (required for gRPC client)
# Create directory structure to match the reference path in csproj (../../Simulator_Service/Simulator.API/Protos)
RUN mkdir -p Simulator_Service/Simulator.API/Protos
COPY ["Simulator_Service/Simulator.API/Protos/simulator_query.proto", "Simulator_Service/Simulator.API/Protos/"]

# Restore dependencies (NuGet.config will use local nuget source)
RUN dotnet restore "./WareHouse_Service/WareHouse_Service.API/WareHouse_Service.API.csproj"
COPY ["WareHouse_Service/", "WareHouse_Service/"]
WORKDIR "/src/WareHouse_Service/WareHouse_Service.API"
RUN dotnet build "./WareHouse_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WareHouse_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
# Install curl for health checks (switch to root temporarily)
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER $APP_UID
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WareHouse_Service.API.dll"]
