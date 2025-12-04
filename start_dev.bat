@echo off
REM Script to start Docker containers in Development environment with Swagger enabled
setlocal enabledelayedexpansion

REM Change to script directory
cd /d "%~dp0"

powershell -Command "Write-Host '=== Starting Docker in Development Mode ===' -ForegroundColor Cyan"
echo.

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    powershell -Command "Write-Host 'ERROR: Docker is not running. Please start Docker Desktop.' -ForegroundColor Red"
    exit /b 1
)

REM Stop and remove existing containers
powershell -Command "Write-Host '=== Stopping existing containers ===' -ForegroundColor DarkCyan"
docker-compose down
if errorlevel 1 (
    powershell -Command "Write-Host 'Warning: docker-compose down failed, continuing...' -ForegroundColor Yellow"
)

REM Remove orphaned containers
powershell -Command "Write-Host '=== Removing orphaned containers ===' -ForegroundColor DarkCyan"
docker-compose down --remove-orphans
if errorlevel 1 (
    powershell -Command "Write-Host 'Warning: Failed to remove orphaned containers' -ForegroundColor Yellow"
)

REM Set environment to Development
set ASPNETCORE_ENVIRONMENT=Development

REM Start containers with Development environment (uses docker-compose.override.yml)
powershell -Command "Write-Host '=== Starting containers in Development mode ===' -ForegroundColor DarkCyan"
powershell -Command "Write-Host 'Swagger will be available for all services' -ForegroundColor Green"
echo.

REM docker-compose automatically uses docker-compose.override.yml if it exists
docker-compose up -d --build
if errorlevel 1 (
    powershell -Command "Write-Host 'ERROR: Failed to start Docker containers' -ForegroundColor Red"
    exit /b 1
)

REM Wait a bit for services to start
powershell -Command "Write-Host '=== Waiting for services to start ===' -ForegroundColor DarkCyan"
timeout /t 10 /nobreak >nul

REM Show container status
powershell -Command "Write-Host '=== Container Status ===' -ForegroundColor DarkCyan"
docker-compose ps

echo.
powershell -Command "Write-Host '=== Development Environment Started ===' -ForegroundColor Green"
echo.
powershell -Command "Write-Host 'Service URLs (Development):' -ForegroundColor Cyan"
powershell -Command "Write-Host '  - Nginx (Frontend): http://localhost' -ForegroundColor White"
powershell -Command "Write-Host '  - IAM Service Direct: http://localhost:5001/swagger' -ForegroundColor Gray"
powershell -Command "Write-Host '  - Laboratory Service Direct: http://localhost:5002/swagger' -ForegroundColor Gray"
powershell -Command "Write-Host '  - Monitoring Service Direct: http://localhost:5003/swagger' -ForegroundColor Gray"
powershell -Command "Write-Host '  - Simulator Service Direct: http://localhost:5004/swagger' -ForegroundColor Gray"
powershell -Command "Write-Host '  - WareHouse Service Direct: http://localhost:5005/swagger' -ForegroundColor Gray"
powershell -Command "Write-Host '  - RabbitMQ Management: http://localhost:15672' -ForegroundColor White"
powershell -Command "Write-Host '    (Username: guest, Password: guest)' -ForegroundColor Gray"
echo.
powershell -Command "Write-Host 'To view logs: docker-compose logs -f' -ForegroundColor Yellow"
powershell -Command "Write-Host 'To stop: docker-compose down' -ForegroundColor Yellow"
echo.

exit /b 0

