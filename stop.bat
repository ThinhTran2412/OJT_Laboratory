@echo off
REM Script to stop Docker containers
setlocal enabledelayedexpansion

REM Change to script directory
cd /d "%~dp0"

powershell -Command "Write-Host '=== Stopping Docker Containers ===' -ForegroundColor Cyan"
echo.

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    powershell -Command "Write-Host 'WARNING: Docker is not running or not accessible.' -ForegroundColor Yellow"
    exit /b 0
)

REM Stop and remove containers
powershell -Command "Write-Host '=== Stopping and removing containers ===' -ForegroundColor DarkCyan"
docker-compose down
if errorlevel 1 (
    powershell -Command "Write-Host 'Warning: Some containers may not have been stopped' -ForegroundColor Yellow"
)

REM Remove orphaned containers
powershell -Command "Write-Host '=== Removing orphaned containers ===' -ForegroundColor DarkCyan"
docker-compose down --remove-orphans
if errorlevel 1 (
    powershell -Command "Write-Host 'Warning: Failed to remove orphaned containers' -ForegroundColor Yellow"
)

REM Show remaining containers (if any)
powershell -Command "Write-Host '=== Checking for remaining containers ===' -ForegroundColor DarkCyan"
docker-compose ps

echo.
powershell -Command "Write-Host '=== Docker Containers Stopped ===' -ForegroundColor Green"
echo.
powershell -Command "Write-Host 'Note: Volumes and images are preserved.' -ForegroundColor Gray"
powershell -Command "Write-Host 'To remove volumes: docker-compose down -v' -ForegroundColor Yellow"
powershell -Command "Write-Host 'To remove images: docker-compose down --rmi all' -ForegroundColor Yellow"
echo.

exit /b 0

