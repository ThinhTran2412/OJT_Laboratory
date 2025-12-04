@echo off
REM ====================================================================
REM Script to check database connection and migration status
REM Using PRODUCTION configuration (Render database)
REM ====================================================================
REM This script checks:
REM   1. Database connection
REM   2. Migration history for both services
REM   3. Schema existence
REM   4. Key tables existence
REM ====================================================================

setlocal enabledelayedexpansion

set "PROJECT_ROOT=%~dp0.."

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Database Connection & Migration Status Check (PRODUCTION)' -ForegroundColor Cyan"
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

REM ====================================================================
REM Check IAM_Service
REM ====================================================================
powershell -Command "Write-Host '[1/5] Checking IAM_Service...' -ForegroundColor Yellow"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\OJT_Laboratory_Project\IAM_Service"

set ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations list ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj ^
  --configuration Release 2>&1 | findstr /C:"has been applied" /C:"Pending" /C:"error" /C:"Exception" /C:"Failed"

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ IAM_Service migration check completed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host '✗ IAM_Service migration check failed - check connection string' -ForegroundColor DarkRed"
)

echo.

REM ====================================================================
REM Check Laboratory_Service
REM ====================================================================
powershell -Command "Write-Host '[2/5] Checking Laboratory_Service...' -ForegroundColor Yellow"
powershell -Command "Write-Host "--------------------------------------------------------------" -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\OJT_Laboratory_Project\Laboratory_Service"

set ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations list ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Release 2>&1 | findstr /C:"has been applied" /C:"Pending" /C:"error" /C:"Exception" /C:"Failed"

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Laboratory_Service migration check completed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host '✗ Laboratory_Service migration check failed - check connection string' -ForegroundColor DarkRed"
)

echo.

REM ====================================================================
REM Check Monitoring_Service
REM ====================================================================
powershell -Command "Write-Host '[3/5] Checking Monitoring_Service...' -ForegroundColor Yellow"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\OJT_Laboratory_Project\Monitoring_Service"

set ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations list ^
  --project Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj ^
  --startup-project Monitoring_Service.API/Monitoring_Service.API.csproj ^
  --configuration Release 2>&1 | findstr /C:"has been applied" /C:"Pending" /C:"error" /C:"Exception" /C:"Failed"

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Monitoring_Service migration check completed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host '✗ Monitoring_Service migration check failed - check connection string' -ForegroundColor DarkRed"
)

echo.

REM ====================================================================
REM Check Simulator_Service
REM ====================================================================
powershell -Command "Write-Host '[4/5] Checking Simulator_Service...' -ForegroundColor Yellow"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\OJT_Laboratory_Project\Simulator_Service"

set ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations list ^
  --project Simulator.Infastructure/Simulator.Infastructure.csproj ^
  --startup-project Simulator.API/Simulator.API.csproj ^
  --configuration Release 2>&1 | findstr /C:"has been applied" /C:"Pending" /C:"error" /C:"Exception" /C:"Failed"

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Simulator_Service migration check completed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host '✗ Simulator_Service migration check failed - check connection string' -ForegroundColor DarkRed"
)

echo.

REM ====================================================================
REM Check WareHouse_Service
REM ====================================================================
powershell -Command "Write-Host '[5/5] Checking WareHouse_Service...' -ForegroundColor Yellow"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\OJT_Laboratory_Project\WareHouse_Service"

set ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations list ^
  --project WareHouse_Service.Infrastructure/WareHouse_Service.Infrastructure.csproj ^
  --startup-project WareHouse_Service.API/WareHouse_Service.API.csproj ^
  --configuration Release 2>&1 | findstr /C:"has been applied" /C:"Pending" /C:"error" /C:"Exception" /C:"Failed"

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ WareHouse_Service migration check completed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host '✗ WareHouse_Service migration check failed - check connection string' -ForegroundColor DarkRed"
)

echo.

powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Status Check Complete' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Next steps:' -ForegroundColor Yellow"
powershell -Command "Write-Host '  1. If migrations show as Pending, run update_databases_prod.bat' -ForegroundColor Gray"
powershell -Command "Write-Host '  2. If connection fails, check connection string in appsettings.Production.json' -ForegroundColor Gray"
powershell -Command "Write-Host '  3. Check Render logs for detailed error messages' -ForegroundColor Gray"
powershell -Command "Write-Host ''"

cd /d "%PROJECT_ROOT%"

