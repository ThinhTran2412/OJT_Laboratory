@echo off
REM ====================================================================
REM Script to CLEAR all migrations for WareHouse_Service
REM ====================================================================
REM WARNING: This script will DELETE all migration files and snapshots!
REM Use this when you want to start fresh with database migrations.
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to WareHouse_Service directory
cd /d "%~dp0\.."

if not exist "WareHouse_Service.Infrastructure\Migrations" (
    powershell -Command "Write-Host 'Error: Migrations folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please check if WareHouse_Service.Infrastructure exists.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host '  WARNING: This will DELETE all WareHouse_Service migrations!' -ForegroundColor Red"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host ''"

cd /d "WareHouse_Service.Infrastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '✓ Cleared WareHouse_Service migrations' -ForegroundColor DarkGreen"
    ) else (
        powershell -Command "Write-Host '✗ Failed to clear WareHouse_Service migrations' -ForegroundColor DarkRed"
        exit /b 1
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in WareHouse_Service' -ForegroundColor DarkYellow"
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'WareHouse_Service migrations cleared successfully! ✓' -ForegroundColor DarkGreen"
powershell -Command "Write-Host 'You can now create new migrations with: Scripts\dev\create_migration.bat' -ForegroundColor Yellow"
exit /b 0

