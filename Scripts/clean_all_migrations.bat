@echo off
REM ====================================================================
REM Script to CLEAR all migrations and snapshots for ALL services
REM in OJT_Laboratory_Project
REM ====================================================================
REM WARNING: This script will DELETE all migration files and snapshots!
REM Use this when you want to start fresh with database migrations.
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to OJT_Laboratory_Project directory
set PROJECT_ROOT=%~dp0..\OJT_Laboratory_Project
cd /d "%PROJECT_ROOT%"

if not exist "IAM_Service" (
    powershell -Command "Write-Host 'Error: OJT_Laboratory_Project folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run setup_project.bat first.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host '  WARNING: This will DELETE all migrations!' -ForegroundColor Red"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host ''"

REM Counter for tracking
set CLEARED_COUNT=0

REM ====================================================================
REM 1. Clear IAM_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[1/5] Clearing IAM_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%PROJECT_ROOT%\IAM_Service\IAM_Service.Infrastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared IAM_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear IAM_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in IAM_Service' -ForegroundColor DarkYellow"
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 2. Clear Laboratory_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[2/5] Clearing Laboratory_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%PROJECT_ROOT%\Laboratory_Service\Laboratory_Service.Infrastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared Laboratory_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear Laboratory_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in Laboratory_Service' -ForegroundColor DarkYellow"
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 3. Clear Monitoring_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[3/5] Clearing Monitoring_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%PROJECT_ROOT%\Monitoring_Service\Monitoring_Service.Infastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared Monitoring_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear Monitoring_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in Monitoring_Service' -ForegroundColor DarkYellow"
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 4. Clear Simulator_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[4/5] Clearing Simulator_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%PROJECT_ROOT%\Simulator_Service\Simulator.Infastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared Simulator_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear Simulator_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in Simulator_Service' -ForegroundColor DarkYellow"
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 5. Clear WareHouse_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[5/5] Clearing WareHouse_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%PROJECT_ROOT%\WareHouse_Service\WareHouse_Service.Infrastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared WareHouse_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear WareHouse_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in WareHouse_Service' -ForegroundColor DarkYellow"
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM Summary - Clear Phase
REM ====================================================================
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Migration Clear Summary' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Cleared: %CLEARED_COUNT% service(s)' -ForegroundColor DarkGreen"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
echo.

REM ====================================================================
REM Auto Recreate Migrations and Apply
REM ====================================================================
powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Auto Recreating Migrations' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Enter migration name (default: Init):' -ForegroundColor Yellow -NoNewline"
set /p MIGRATION_NAME=
if "!MIGRATION_NAME!"=="" (
    set MIGRATION_NAME=Init
)

REM Change to Deploy directory to run scripts
set DEPLOY_ROOT=%~dp0..
cd /d "%DEPLOY_ROOT%"
if not exist "%DEPLOY_ROOT%\Scripts_Database_Dev" (
    powershell -Command "Write-Host 'Error: Scripts_Database_Dev folder not found!' -ForegroundColor DarkRed"
    exit /b 1
)

REM Step 1: Create migrations
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Step 1: Creating new migrations...' -ForegroundColor DarkCyan"
call "%DEPLOY_ROOT%\Scripts_Database_Pro\create_migrations_prod.bat" "!MIGRATION_NAME!"

if %ERRORLEVEL% neq 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Failed to create migrations. Stopping...' -ForegroundColor DarkRed"
    exit /b 1
)

REM Step 2: Apply migrations
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Step 2: Applying migrations to database...' -ForegroundColor DarkCyan"
call "%DEPLOY_ROOT%\Scripts_Database_Pro\update_databases_prod.bat"

if %ERRORLEVEL% neq 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Failed to apply migrations. Please check errors above.' -ForegroundColor DarkRed"
    exit /b 1
)

REM Final Summary
powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Green"
powershell -Command "Write-Host '  ✓ All migrations cleared, recreated, and applied!' -ForegroundColor Green"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Green"
powershell -Command "Write-Host ''"
exit /b 0

