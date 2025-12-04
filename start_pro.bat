@echo off
REM Script to start Docker containers in Production environment
setlocal enabledelayedexpansion

REM Change to script directory
cd /d "%~dp0"

powershell -Command "Write-Host '=== Starting Docker in Production Mode ===' -ForegroundColor Cyan"
echo.

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    powershell -Command "Write-Host 'ERROR: Docker is not running. Please start Docker Desktop.' -ForegroundColor Red"
    exit /b 1
)

REM Stop and remove existing containers
powershell -Command "Write-Host '=== Stopping existing containers ===' -ForegroundColor DarkCyan"
docker-compose -f docker-compose.yml down
if errorlevel 1 (
    powershell -Command "Write-Host 'Warning: docker-compose down failed, continuing...' -ForegroundColor Yellow"
)

REM Remove orphaned containers
powershell -Command "Write-Host '=== Removing orphaned containers ===' -ForegroundColor DarkCyan"
docker-compose -f docker-compose.yml down --remove-orphans
if errorlevel 1 (
    powershell -Command "Write-Host 'Warning: Failed to remove orphaned containers' -ForegroundColor Yellow"
)

REM Set environment to Production
set ASPNETCORE_ENVIRONMENT=Production

REM Start containers with Production environment (ignore override file)
powershell -Command "Write-Host '=== Starting containers in Production mode ===' -ForegroundColor DarkCyan"
powershell -Command "Write-Host 'Swagger is disabled in Production' -ForegroundColor Yellow"
echo.

REM Use only docker-compose.yml, ignore docker-compose.override.yml
docker-compose -f docker-compose.yml up -d --build
if errorlevel 1 (
    powershell -Command "Write-Host 'ERROR: Failed to start Docker containers' -ForegroundColor Red"
    exit /b 1
)

REM Wait a bit for services to start
powershell -Command "Write-Host '=== Waiting for services to start ===' -ForegroundColor DarkCyan"
timeout /t 10 /nobreak >nul

REM Show container status
powershell -Command "Write-Host '=== Container Status ===' -ForegroundColor DarkCyan"
docker-compose -f docker-compose.yml ps

echo.
powershell -Command "Write-Host '=== Production Environment Started ===' -ForegroundColor Green"
echo.
powershell -Command "Write-Host 'Service URLs (Production):' -ForegroundColor Cyan"
powershell -Command "Write-Host '  - Nginx (Frontend): http://localhost' -ForegroundColor White"
powershell -Command "Write-Host '  - IAM Service API: http://localhost/api/iam' -ForegroundColor White"
powershell -Command "Write-Host '  - Laboratory Service API: http://localhost/api/laboratory' -ForegroundColor White"
powershell -Command "Write-Host '  - Monitoring Service API: http://localhost/api/monitoring' -ForegroundColor White"
powershell -Command "Write-Host '  - Simulator Service API: http://localhost/api/simulator' -ForegroundColor White"
powershell -Command "Write-Host '  - WareHouse Service API: http://localhost/api/warehouse' -ForegroundColor White"
powershell -Command "Write-Host '  - RabbitMQ Management: http://localhost:15672' -ForegroundColor White"
powershell -Command "Write-Host '    (Username: guest, Password: guest)' -ForegroundColor Gray"
echo.
powershell -Command "Write-Host 'To view logs: docker-compose logs -f' -ForegroundColor Yellow"
powershell -Command "Write-Host 'To stop: docker-compose down' -ForegroundColor Yellow"
echo.

exit /b 0

