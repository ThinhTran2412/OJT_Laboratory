@echo off
REM ====================================================================
REM Script to CREATE new migrations for ALL services
REM Using DEVELOPMENT configuration
REM ====================================================================
REM Usage: create_migrations_dev.bat "MigrationName"
REM Example: create_migrations_dev.bat "AddNewTable"
REM ====================================================================

setlocal enabledelayedexpansion

REM Get migration name from argument or prompt user to input
if "%~1"=="" (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
    powershell -Command "Write-Host '  Create Migrations for ALL Services (DEVELOPMENT)' -ForegroundColor Cyan"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Enter migration name:' -ForegroundColor Yellow -NoNewline"
    set /p MIGRATION_NAME=
    if "!MIGRATION_NAME!"=="" (
        powershell -Command "Write-Host ''"
        powershell -Command "Write-Host 'Error: Migration name cannot be empty!' -ForegroundColor DarkRed"
        powershell -Command "Write-Host 'Usage: create_migrations_dev.bat \"MigrationName\"' -ForegroundColor Yellow"
        powershell -Command "Write-Host 'Example: create_migrations_dev.bat \"AddNewTable\"' -ForegroundColor Yellow"
        powershell -Command "Write-Host ''"
        powershell -Command "Write-Host 'If you want to create initial migrations, use:' -ForegroundColor Yellow"
        powershell -Command "Write-Host '  create_migrations_dev.bat \"InitialCreate\"' -ForegroundColor Yellow"
        exit /b 1
    )
) else (
    set MIGRATION_NAME=%~1
)

REM Change to OJT_Laboratory_Project directory (go up one level from Scripts_Database_Dev)
set PROJECT_ROOT=%~dp0..\OJT_Laboratory_Project
cd /d "%PROJECT_ROOT%"

if not exist "IAM_Service" (
    powershell -Command "Write-Host 'Error: OJT_Laboratory_Project folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run setup_project.bat first.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Creating Migrations (DEVELOPMENT)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Migration Name: %MIGRATION_NAME%' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Local Database (localhost:5432)' -ForegroundColor Cyan"
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
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host ''"

REM Counter for tracking success/failure
set SUCCESS_COUNT=0
set FAIL_COUNT=0

REM ====================================================================
REM 1. IAM_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[1/5] Creating IAM_Service Migration (DEV): %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\IAM_Service"
call dotnet restore >nul 2>&1
call dotnet build IAM_Service.sln --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ IAM_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ IAM_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 2. Laboratory_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[2/5] Creating Laboratory_Service Migration (DEV): %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\Laboratory_Service"
call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Laboratory_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Laboratory_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 3. Monitoring_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[3/5] Creating Monitoring_Service Migration (DEV): %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\Monitoring_Service"
call dotnet restore Monitoring_Service.API/Monitoring_Service.API.csproj >nul 2>&1
call dotnet build Monitoring_Service.API/Monitoring_Service.API.csproj --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj ^
  --startup-project Monitoring_Service.API/Monitoring_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Monitoring_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Monitoring_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 4. Simulator_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[4/5] Creating Simulator_Service Migration (DEV): %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\Simulator_Service"
call dotnet restore >nul 2>&1
call dotnet build Simulator_Service.sln --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Simulator.Infastructure/Simulator.Infastructure.csproj ^
  --startup-project Simulator.API/Simulator.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Simulator_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Simulator_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 5. WareHouse_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[5/5] Creating WareHouse_Service Migration (DEV): %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\WareHouse_Service"
call dotnet restore >nul 2>&1
call dotnet build WareHouse_Service.sln --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project WareHouse_Service.Infrastructure/WareHouse_Service.Infrastructure.csproj ^
  --startup-project WareHouse_Service.API/WareHouse_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ WareHouse_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ WareHouse_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM Summary
REM ====================================================================
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Migration Creation Summary (DEVELOPMENT)' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Successful: %SUCCESS_COUNT% service(s)' -ForegroundColor DarkGreen"
powershell -Command "Write-Host '  Failed:     %FAIL_COUNT% service(s)' -ForegroundColor DarkRed"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
echo.

if %FAIL_COUNT% equ 0 (
    powershell -Command "Write-Host 'All migrations created successfully! ✓' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host 'Run update_databases_dev.bat to apply migrations.' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host 'Some migrations creation failed. Please check the errors above. ✗' -ForegroundColor DarkRed"
    exit /b 1
)

