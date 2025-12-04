@echo off
setlocal enabledelayedexpansion

echo =================================================================================================================
echo                                       OJT Laboratory Project Setup Script
echo =================================================================================================================
echo.

REM --- Load Git Configuration ---
call git_config.bat

REM --- Step 1: Create Root Folder ---
echo [1/19] Create Main Folder...
mkdir OJT_Laboratory_Project
cd OJT_Laboratory_Project

REM --- Step 2: Create Child Folders ---
echo [2/19] Create Child Folders...
mkdir IAM_Service
mkdir Laboratory_Service
mkdir Front_End
mkdir Monitoring_Service
mkdir Simulator_Service
mkdir WareHouse_Service

REM --- Step 3: Configure IAM_Service ---
echo [3/19] Configure repository IAM_Service...
cd IAM_Service
git init
git remote add origin %IAM_SERVICE_REPO_URL%
git fetch origin
git checkout -b %IAM_SERVICE_BRANCH% origin/%IAM_SERVICE_BRANCH%
git pull origin %IAM_SERVICE_BRANCH%
cd ..

REM --- Step 4: Configure Laboratory_Service ---
echo [4/19] Configure repository Laboratory_Service...
cd Laboratory_Service
git init
git remote add origin %LABORATORY_SERVICE_REPO_URL%
git fetch origin
git checkout -b %LABORATORY_SERVICE_BRANCH% origin/%LABORATORY_SERVICE_BRANCH%
git pull origin %LABORATORY_SERVICE_BRANCH%
cd ..

REM --- Step 5: Configure Front_End ---
echo [5/19] Configure repository Front_End...
cd Front_End
git init
git remote add origin %FRONT_END_REPO_URL%
git fetch origin
git checkout -b %FRONT_END_BRANCH% origin/%FRONT_END_BRANCH%
git pull origin %FRONT_END_BRANCH%
cd ..

REM --- Step 6: Configure Monitoring_Service ---
echo [6/19] Configure repository Monitoring_Service...
cd Monitoring_Service
git init
git remote add origin %MONITORING_SERVICE_REPO_URL%
git fetch origin
git checkout -b %MONITORING_SERVICE_BRANCH% origin/%MONITORING_SERVICE_BRANCH%
git pull origin %MONITORING_SERVICE_BRANCH%
cd ..

REM --- Step 7: Configure Simulator_Service ---
echo [7/19] Configure repository Simulator_Service...
cd Simulator_Service
git init
git remote add origin %SIMULATOR_SERVICE_REPO_URL%
git fetch origin
git checkout -b %SIMULATOR_SERVICE_BRANCH% origin/%SIMULATOR_SERVICE_BRANCH%
git pull origin %SIMULATOR_SERVICE_BRANCH%
cd ..

REM --- Step 8: Configure WareHouse_Service ---
echo [8/19] Configure repository WareHouse_Service...
cd WareHouse_Service
git init
git remote add origin %WAREHOUSE_SERVICE_REPO_URL%
git fetch origin
git checkout -b %WAREHOUSE_SERVICE_BRANCH% origin/%WAREHOUSE_SERVICE_BRANCH%
git pull origin %WAREHOUSE_SERVICE_BRANCH%
cd ..

REM --- Step 9: Restore NuGet Packages for IAM_Service ---
echo [9/19] Restoring NuGet Packages for IAM_Service...
cd IAM_Service
dotnet restore
cd ..

REM --- Step 10: Restore NuGet Packages for Laboratory_Service ---
echo [10/19] Restoring NuGet Packages for Laboratory_Service...
cd Laboratory_Service
dotnet restore
cd ..

REM --- Step 11: Restore NuGet Packages for Monitoring_Service ---
echo [11/19] Restoring NuGet Packages for Monitoring_Service...
cd Monitoring_Service
dotnet restore
cd ..

REM --- Step 12: Restore NuGet Packages for Simulator_Service ---
echo [12/19] Restoring NuGet Packages for Simulator_Service...
cd Simulator_Service
dotnet restore
cd ..

REM --- Step 13: Restore NuGet Packages for WareHouse_Service ---
echo [13/19] Restoring NuGet Packages for WareHouse_Service...
cd WareHouse_Service
dotnet restore
cd ..

REM --- Step 14: Build IAM_Service ---
echo [14/19] Building IAM_Service with .NET...
cd IAM_Service
dotnet build 
cd ..

REM --- Step 15: Build Laboratory_Service ---
echo [15/19] Building Laboratory_Service with .NET...
cd Laboratory_Service
dotnet build 
cd ..

REM --- Step 16: Build Monitoring_Service ---
echo [16/19] Building Monitoring_Service with .NET...
cd Monitoring_Service
dotnet build 
cd ..

REM --- Step 17: Build Simulator_Service ---
echo [17/19] Building Simulator_Service with .NET...
cd Simulator_Service
dotnet build 
cd ..

REM --- Step 18: Build WareHouse_Service ---
echo [18/19] Building WareHouse_Service with .NET...
cd WareHouse_Service
dotnet build 
cd ..

REM --- Step 19: Call npm install script ---
echo [19/19] Calling npm install script...
call .\install_npm.bat

cd ..

REM --- Step 17: Database Scripts are already in place ---
echo [17/17] Database scripts are organized in Scripts_Database_Dev and Scripts_Database_Pro...
echo   - Scripts_Database_Dev\create_migrations_dev.bat
echo   - Scripts_Database_Dev\update_databases_dev.bat
echo   - Scripts_Database_Pro\create_migrations_prod.bat
echo   - Scripts_Database_Pro\update_databases_prod.bat
echo   - Scripts\clean_all_migrations.bat (for cleaning all migrations)
echo.
    echo   - Copied create_migrations_dev.bat (from Scripts_Database_Dev)
)
if exist "Scripts_Database_Dev\update_databases_dev.bat" (
    copy /Y "Scripts_Database_Dev\update_databases_dev.bat" "OJT_Laboratory_Project\" >nul 2>&1
    echo   - Copied update_databases_dev.bat (from Scripts_Database_Dev)
)
if exist "Scripts_Database_Pro\create_migrations_prod.bat" (
    copy /Y "Scripts_Database_Pro\create_migrations_prod.bat" "OJT_Laboratory_Project\" >nul 2>&1
    echo   - Copied create_migrations_prod.bat (from Scripts_Database_Pro)
)
if exist "Scripts_Database_Pro\update_databases_prod.bat" (
    copy /Y "Scripts_Database_Pro\update_databases_prod.bat" "OJT_Laboratory_Project\" >nul 2>&1
    echo   - Copied update_databases_prod.bat (from Scripts_Database_Pro)
)
echo   Database scripts copied successfully!
echo.

echo.
echo ================================================================================================================
echo                                         Success Setup Project!
echo ================================================================================================================
echo Folder: %cd%\OJT_Laboratory_Project
echo.

REM --- Step 18: Optional Database Migration Reset ---
echo.
echo ================================================================================================================
echo                         Optional: Reset Database Migrations
echo ================================================================================================================
echo.
echo This will:
echo   1. Clear all existing migrations and snapshots
echo   2. Create new initial migrations for all services
echo   3. Update the database with fresh migrations
echo.
echo WARNING: This will DELETE all existing migration files!
echo.
set /p RESET_DB="Do you want to reset database migrations? (y/n): "

if /i "%RESET_DB%"=="y" (
    echo.
    powershell -Command "Write-Host 'Starting database migration reset...' -ForegroundColor DarkCyan"
    echo.
    
    REM Clear all migrations (using script from Deploy folder)
    echo [Step 1/3] Clearing all existing migrations...
    call clear_all_migrations.bat
    if %ERRORLEVEL% neq 0 (
        powershell -Command "Write-Host 'Warning: Clear migrations had errors, continuing...' -ForegroundColor DarkYellow"
    )
    echo.
    
    REM Create new initial migrations (using script from Scripts_Database_Dev folder)
    echo [Step 2/3] Creating new initial migrations (Development)...
    call Scripts_Database_Dev\create_migrations_dev.bat "InitialCreate"
    if %ERRORLEVEL% neq 0 (
        powershell -Command "Write-Host 'Error: Failed to create migrations!' -ForegroundColor DarkRed"
        goto :skip_reset
    )
    echo.
    
    REM Update database (using script from Scripts_Database_Dev folder)
    echo [Step 3/3] Updating database (Development)...
    call Scripts_Database_Dev\update_databases_dev.bat
    if %ERRORLEVEL% neq 0 (
        powershell -Command "Write-Host 'Warning: Database update had errors. Please check manually.' -ForegroundColor DarkYellow"
    )
    echo.
    
    powershell -Command "Write-Host 'Database migration reset completed!' -ForegroundColor DarkGreen"
    echo.
    goto :continue_setup
)

:skip_reset
echo.
echo Database migration reset skipped.
echo You can run it manually later using:
echo   cd Deploy
echo   clear_all_migrations.bat
echo   create_all_migrations.bat "InitialCreate"
echo   update_all_databases.bat
echo.
echo Or from OJT_Laboratory_Project folder:
echo   cd OJT_Laboratory_Project
echo   clear_all_migrations.bat
echo   create_migrations_dev.bat "InitialCreate"  (copied from Scripts_Database_Dev)
echo   update_databases_dev.bat  (copied from Scripts_Database_Dev)
echo.
echo Or use scripts directly from Deploy folder:
echo   cd Deploy
echo   Scripts_Database_Dev\create_migrations_dev.bat "InitialCreate"
echo   Scripts_Database_Dev\update_databases_dev.bat
echo.
echo For Production:
echo   Scripts_Database_Pro\create_migrations_prod.bat "InitialCreate"
echo   Scripts_Database_Pro\update_databases_prod.bat
echo.

:continue_setup

REM --- Open VS Code ---
echo Opening VS Code...
cd OJT_Laboratory_Project
code .
cd ..

pause
