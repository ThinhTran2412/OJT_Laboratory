# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first for better layer caching
COPY ["Monitoring_Service.API/Monitoring_Service.API.csproj", "Monitoring_Service.API/"]
COPY ["Monitoring_Service.Application/Monitoring_Service.Application.csproj", "Monitoring_Service.Application/"]
COPY ["Monitoring_Service.Domain/Monitoring_Service.Domain.csproj", "Monitoring_Service.Domain/"]
COPY ["Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj", "Monitoring_Service.Infastructure/"]

# Restore dependencies
RUN dotnet restore "./Monitoring_Service.API/Monitoring_Service.API.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/Monitoring_Service.API"
RUN dotnet build "./Monitoring_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Monitoring_Service.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "Monitoring_Service.API.dll"]

