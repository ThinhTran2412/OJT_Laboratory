@echo off
REM Batch script to update database migration for IAM_Service.API
setlocal enabledelayedexpansion

REM Change to project root directory
cd /d "%~dp0\.."

powershell -Command "Write-Host '=== Updating Database Migration ===' -ForegroundColor DarkCyan"
powershell -Command "Write-Host 'Project: IAM_Service.API' -ForegroundColor DarkCyan"
echo.

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
powershell -Command "Write-Host 'Restoring packages...' -ForegroundColor DarkCyan"
dotnet restore >nul 2>&1

echo.
powershell -Command "Write-Host 'Building solution...' -ForegroundColor DarkCyan"
dotnet build IAM_Service.sln --configuration Release --no-restore >nul 2>&1

echo.
powershell -Command "Write-Host 'Applying database migrations...' -ForegroundColor DarkCyan"
echo.

dotnet ef database update ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj

if %ERRORLEVEL% equ 0 (
    echo.
    powershell -Command "Write-Host 'Database migration completed successfully!' -ForegroundColor DarkGreen"
) else (
    echo.
    powershell -Command "Write-Host 'Database migration failed!' -ForegroundColor DarkRed"
    exit /b 1
)

exit /b 0

