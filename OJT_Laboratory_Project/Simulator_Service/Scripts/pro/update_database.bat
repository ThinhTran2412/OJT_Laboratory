@echo off
REM ====================================================================
REM Script to UPDATE database migrations for Simulator_Service
REM Using PRODUCTION configuration (Render database)
REM ====================================================================
REM Usage: update_database.bat
REM WARNING: This will update the PRODUCTION database on Render!
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to Simulator_Service directory (go up two levels from pro folder)
cd /d "%~dp0\..\.."

if not exist "Simulator_Service.sln" (
    powershell -Command "Write-Host 'Error: Simulator_Service.sln not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run this script from Simulator_Service\Scripts\pro folder.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Updating Simulator_Service Database (PRODUCTION)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Render Database' -ForegroundColor Cyan"
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
call dotnet build Simulator_Service.sln --configuration Release --no-restore >nul 2>&1

echo Applying migrations to PRODUCTION database (uses appsettings.Production.json)...
set ASPNETCORE_ENVIRONMENT=Production
dotnet ef database update ^
  --project Simulator.Infastructure/Simulator.Infastructure.csproj ^
  --startup-project Simulator.API/Simulator.API.csproj ^
  --configuration Release

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✓ Simulator_Service database updated successfully!' -ForegroundColor DarkGreen"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✗ Simulator_Service database update failed!' -ForegroundColor DarkRed"
    exit /b 1
)

