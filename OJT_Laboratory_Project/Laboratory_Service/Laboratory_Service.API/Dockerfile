# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.config first (for local package source)
COPY ["Laboratory_Service/NuGet.config", "./"]

# Copy local nuget packages
COPY ["Laboratory_Service/nuget/", "./nuget/"]

# Copy csproj files first for better layer caching
COPY ["Laboratory_Service/Laboratory_Service.API/Laboratory_Service.API.csproj", "Laboratory_Service/Laboratory_Service.API/"]
COPY ["Laboratory_Service/Laboratory_Service.Application/Laboratory_Service.Application.csproj", "Laboratory_Service/Laboratory_Service.Application/"]
COPY ["Laboratory_Service/Laboratory_Service.Domain/Laboratory_Service.Domain.csproj", "Laboratory_Service/Laboratory_Service.Domain/"]
COPY ["Laboratory_Service/Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj", "Laboratory_Service/Laboratory_Service.Infrastructure/"]
COPY ["Laboratory_Service/ShareLibrarySolution/SharedLibrary.csproj", "Laboratory_Service/ShareLibrarySolution/"]

# Copy proto files from other services (required for gRPC clients)
# Create directory structure to match the reference paths in csproj files
# Path 1: ../../IAM_Service/IAM_Service.API/Protos/user_service.proto
RUN mkdir -p IAM_Service/IAM_Service.API/Protos
COPY ["IAM_Service/IAM_Service.API/Protos/user_service.proto", "IAM_Service/IAM_Service.API/Protos/"]

# Path 2: ../../Simulator_Service/Simulator.API/Protos/simulator_query.proto
RUN mkdir -p Simulator_Service/Simulator.API/Protos
COPY ["Simulator_Service/Simulator.API/Protos/simulator_query.proto", "Simulator_Service/Simulator.API/Protos/"]

# Path 3: ../../WareHouse_Service/WareHouse_Service.API/Protos/warehouse_service.proto
RUN mkdir -p WareHouse_Service/WareHouse_Service.API/Protos
COPY ["WareHouse_Service/WareHouse_Service.API/Protos/warehouse_service.proto", "WareHouse_Service/WareHouse_Service.API/Protos/"]

# Restore dependencies (NuGet.config will use local nuget source)
RUN dotnet restore "./Laboratory_Service/Laboratory_Service.API/Laboratory_Service.API.csproj"

# Copy all source code
COPY ["Laboratory_Service/", "Laboratory_Service/"]

# Build the project
WORKDIR "/src/Laboratory_Service/Laboratory_Service.API"
RUN dotnet build "./Laboratory_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Laboratory_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "Laboratory_Service.API.dll"]

