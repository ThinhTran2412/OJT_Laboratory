# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first for better layer caching
COPY ["Simulator.API/Simulator.API.csproj", "Simulator.API/"]
COPY ["Simulator.Application/Simulator.Application.csproj", "Simulator.Application/"]
COPY ["Simulator.Domain/Simulator.Domain.csproj", "Simulator.Domain/"]
COPY ["Simulator.Infastructure/Simulator.Infastructure.csproj", "Simulator.Infastructure/"]

# Restore dependencies
RUN dotnet restore "./Simulator.API/Simulator.API.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/Simulator.API"
RUN dotnet build "./Simulator.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Simulator.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "Simulator.API.dll"]

