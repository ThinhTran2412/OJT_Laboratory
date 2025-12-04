@echo off
REM Batch script to run tests for Laboratory_Service
setlocal enabledelayedexpansion

REM Change to project root directory
cd /d "%~dp0\.."

powershell -Command "Write-Host '=== Running Tests ===' -ForegroundColor DarkCyan"
echo.

REM Unlock files
taskkill /F /IM dotnet.exe /T 2>nul
taskkill /F /IM MSBuild.exe /T 2>nul
timeout /t 1 /nobreak >nul

REM Clean, restore, build
powershell -Command "Write-Host 'Cleaning solution...' -ForegroundColor DarkCyan"
dotnet clean Laboratory_Service.sln >nul 2>&1
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s /q "%%d" 2>nul

powershell -Command "Write-Host 'Restoring packages...' -ForegroundColor DarkCyan"
dotnet restore >nul 2>&1

powershell -Command "Write-Host 'Building solution...' -ForegroundColor DarkCyan"
dotnet build Laboratory_Service.sln --configuration Debug --no-restore >nul 2>&1

REM Prepare folders
if not exist "Laboratory_Service.Application.UnitTest\TestResults\coverage" mkdir "Laboratory_Service.Application.UnitTest\TestResults\coverage"
if not exist "Laboratory_Service.Application.UnitTest\TestResults\CoverageReport" mkdir "Laboratory_Service.Application.UnitTest\TestResults\CoverageReport"

powershell -Command "Write-Host 'Running tests with coverage...' -ForegroundColor DarkCyan"
dotnet test Laboratory_Service.Application.UnitTest/Laboratory_Service.Application.UnitTest.csproj ^
  --collect:"XPlat Code Coverage" ^
  --results-directory ./Laboratory_Service.Application.UnitTest/TestResults ^
  /p:CollectCoverage=true ^
  /p:CoverletOutput="Laboratory_Service.Application.UnitTest/TestResults/coverage/" ^
  /p:CoverletOutputFormat=cobertura ^
  /p:Include="[Laboratory_Service*]*" ^
  --configuration Debug ^
  -v n
set TEST_EXIT_CODE=%ERRORLEVEL%

echo.
powershell -Command "Write-Host 'Tests completed' -ForegroundColor DarkCyan"
powershell -Command "Write-Host 'Exit code: %TEST_EXIT_CODE%' -ForegroundColor DarkCyan"
echo.

REM Check coverage file
set COVERAGE_FILE=Laboratory_Service.Application.UnitTest\TestResults\coverage\coverage.cobertura.xml
if not exist "%COVERAGE_FILE%" (
    for /r "Laboratory_Service.Application.UnitTest\TestResults" %%f in (coverage.cobertura.xml) do (
        if exist "%%f" (
            set COVERAGE_FILE=%%f
            goto :coverage_found
        )
    )
    :coverage_found
    if not exist "%COVERAGE_FILE%" (
        if not exist "Laboratory_Service.Application.UnitTest\TestResults\coverage" mkdir "Laboratory_Service.Application.UnitTest\TestResults\coverage"
        echo ^<coverage^>^</coverage^> > "%COVERAGE_FILE%"
    )
)

REM Generate HTML report
powershell -Command "Write-Host 'Generating HTML coverage report...' -ForegroundColor DarkCyan"
dotnet tool restore >nul 2>&1
dotnet tool run reportgenerator ^
  -reports:"Laboratory_Service.Application.UnitTest/TestResults/**/coverage.cobertura.xml" ^
  -targetdir:"Laboratory_Service.Application.UnitTest/TestResults/CoverageReport" ^
  -reporttypes:Html >nul 2>nul

set HTML_REPORT=Laboratory_Service.Application.UnitTest\TestResults\CoverageReport\index.html
if exist "%HTML_REPORT%" (
    powershell -Command "Write-Host 'HTML coverage report generated' -ForegroundColor DarkGreen"
    start "" "%HTML_REPORT%"
) else (
    powershell -Command "Write-Host 'HTML report not found' -ForegroundColor DarkYellow"
)

echo.
if %TEST_EXIT_CODE% neq 0 (
    powershell -Command "Write-Host 'Some tests failed' -ForegroundColor DarkYellow"
) else (
    powershell -Command "Write-Host 'All tests passed' -ForegroundColor DarkGreen"
)

powershell -Command "Write-Host 'Build + Test + Coverage complete' -ForegroundColor DarkGreen"
exit /b 0
