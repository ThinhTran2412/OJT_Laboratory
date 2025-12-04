@echo off
REM ====================================================================
REM Script to CREATE new migration for Monitoring_Service
REM Using PRODUCTION configuration (Render database)
REM ====================================================================
REM Usage: create_migration.bat "MigrationName"
REM Example: create_migration.bat "AddNewTable"
REM WARNING: This uses Production configuration (appsettings.Production.json)
REM ====================================================================

setlocal enabledelayedexpansion

REM Get migration name from argument or prompt user to input
if "%~1"=="" (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Enter migration name:' -ForegroundColor Cyan -NoNewline"
    set /p MIGRATION_NAME=
    if "!MIGRATION_NAME!"=="" (
        powershell -Command "Write-Host 'Error: Migration name cannot be empty!' -ForegroundColor DarkRed"
        exit /b 1
    )
) else (
    set MIGRATION_NAME=%~1
)

REM Change to Monitoring_Service directory (go up two levels from pro folder)
cd /d "%~dp0\..\.."

if not exist "Monitoring_Service.API\Monitoring_Service.API.csproj" (
    powershell -Command "Write-Host 'Error: Monitoring_Service.API.csproj not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run this script from Monitoring_Service\Scripts\pro folder.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Creating Monitoring_Service Migration (PRODUCTION)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Migration Name: %MIGRATION_NAME%' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Uses: appsettings.Production.json (Render DB)' -ForegroundColor Cyan"
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
call dotnet restore Monitoring_Service.API/Monitoring_Service.API.csproj >nul 2>&1
call dotnet build Monitoring_Service.API/Monitoring_Service.API.csproj --configuration Release --no-restore >nul 2>&1

echo Creating migration with PRODUCTION configuration (uses appsettings.Production.json)...
set ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj ^
  --startup-project Monitoring_Service.API/Monitoring_Service.API.csproj ^
  --configuration Release

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✓ Monitoring_Service migration created successfully!' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host 'Remember to commit and push this migration file to Git.' -ForegroundColor Yellow"
    powershell -Command "Write-Host 'Run update_database.bat to apply migration.' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✗ Monitoring_Service migration creation failed!' -ForegroundColor DarkRed"
    exit /b 1
)

