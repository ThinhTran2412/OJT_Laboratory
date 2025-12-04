# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.config first (for local package source)
COPY ["IAM_Service/NuGet.config", "./"]

# Copy local nuget packages
COPY ["IAM_Service/nuget/", "./nuget/"]

# Copy csproj files for better layer caching
COPY ["IAM_Service/IAM_Service.API/IAM_Service.API.csproj", "IAM_Service/IAM_Service.API/"]
COPY ["IAM_Service/IAM_Service.Application/IAM_Service.Application.csproj", "IAM_Service/IAM_Service.Application/"]
COPY ["IAM_Service/IAM_Service.Domain/IAM_Service.Domain.csproj", "IAM_Service/IAM_Service.Domain/"]
COPY ["IAM_Service/IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj", "IAM_Service/IAM_Service.Infrastructure/"]
COPY ["IAM_Service/ShareLibrarySolution/SharedLibrary/SharedLibrary.csproj", "IAM_Service/ShareLibrarySolution/SharedLibrary/"]

# Copy proto files from Laboratory_Service (required for gRPC client)
# Create directory structure to match the reference path in csproj (../../Laboratory_Service/Laboratory_Service.API/Protos)
RUN mkdir -p Laboratory_Service/Laboratory_Service.API/Protos
COPY ["Laboratory_Service/Laboratory_Service.API/Protos/patient.proto", "Laboratory_Service/Laboratory_Service.API/Protos/"]

# Restore dependencies (NuGet.config will use local nuget source)
RUN dotnet restore "./IAM_Service/IAM_Service.API/IAM_Service.API.csproj"
COPY ["IAM_Service/", "IAM_Service/"]
WORKDIR "/src/IAM_Service/IAM_Service.API"
RUN dotnet build "./IAM_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IAM_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
# Install curl for health checks (switch to root temporarily)
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER $APP_UID
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IAM_Service.API.dll"]