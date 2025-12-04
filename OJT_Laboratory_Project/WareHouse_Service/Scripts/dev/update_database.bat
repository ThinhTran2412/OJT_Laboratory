@echo off
REM ====================================================================
REM Script to UPDATE database migrations for WareHouse_Service
REM Using DEVELOPMENT configuration
REM ====================================================================
REM Usage: update_database.bat
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to WareHouse_Service directory (go up two levels from dev folder)
cd /d "%~dp0\..\.."

if not exist "WareHouse_Service.sln" (
    powershell -Command "Write-Host 'Error: WareHouse_Service.sln not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run this script from WareHouse_Service\Scripts\dev folder.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Updating WareHouse_Service Database (DEVELOPMENT)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Local Database (localhost)' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host ''"

REM Check if dotnet-ef tool is installed
powershell -Command "Write-Host 'Checking EF Core Tools...' -ForegroundColor DarkCyan"
dotnet tool list -g | findstr "dotnet-ef" >nul
if %ERRORLEVEL% neq 0 (
    powershell -Command "Write-Host 'EF Core Tools not found. Installing...' -ForegroundColor DarkYellow"
    dotnet tool install --global dotnet-ef >nul 2>&1
    powershell -Command "Write-Host 'EF Core Tools installed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host 'EF Core Tools found' -ForegroundColor DarkGreen"
)

echo.
call dotnet restore >nul 2>&1
call dotnet build WareHouse_Service.sln --configuration Development --no-restore >nul 2>&1

echo Applying migrations to DEVELOPMENT database...
dotnet ef database update ^
  --project WareHouse_Service.Infrastructure/WareHouse_Service.Infrastructure.csproj ^
  --startup-project WareHouse_Service.API/WareHouse_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✓ WareHouse_Service database updated successfully!' -ForegroundColor DarkGreen"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✗ WareHouse_Service database update failed!' -ForegroundColor DarkRed"
    exit /b 1
)

