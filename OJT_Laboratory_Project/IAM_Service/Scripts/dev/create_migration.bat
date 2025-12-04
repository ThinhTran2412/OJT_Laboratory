@echo off
REM ====================================================================
REM Script to CREATE new migration for IAM_Service
REM Using DEVELOPMENT configuration
REM ====================================================================
REM Usage: create_migration.bat "MigrationName"
REM Example: create_migration.bat "AddNewTable"
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

REM Change to IAM_Service directory (go up two levels from dev folder)
cd /d "%~dp0\..\.."

if not exist "IAM_Service.sln" (
    powershell -Command "Write-Host 'Error: IAM_Service.sln not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run this script from IAM_Service\Scripts\dev folder.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Creating IAM_Service Migration (DEVELOPMENT)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Migration Name: %MIGRATION_NAME%' -ForegroundColor Cyan"
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
call dotnet build IAM_Service.sln --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✓ IAM_Service migration created successfully!' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host 'Run update_database.bat to apply migration.' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✗ IAM_Service migration creation failed!' -ForegroundColor DarkRed"
    exit /b 1
)

