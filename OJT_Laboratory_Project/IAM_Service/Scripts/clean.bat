@echo off
REM Batch script to clean bin, obj, and Visual Studio files for IAM_Service
setlocal enabledelayedexpansion

REM Change to project root directory
cd /d "%~dp0\.."

powershell -Command "Write-Host '=== Cleaning IAM_Service ===' -ForegroundColor DarkCyan"
echo.

REM Stop dotnet and Visual Studio processes to unlock files
powershell -Command "Write-Host '=== Unlocking files ===' -ForegroundColor DarkCyan"
taskkill /F /IM dotnet.exe /T 2>nul
taskkill /F /IM MSBuild.exe /T 2>nul
taskkill /F /IM devenv.exe /T 2>nul
taskkill /F /IM VBCSCompiler.exe /T 2>nul
REM Kill browser processes that might have HTML report open
taskkill /F /IM chrome.exe /T 2>nul
taskkill /F /IM msedge.exe /T 2>nul
taskkill /F /IM firefox.exe /T 2>nul
taskkill /F /IM iexplore.exe /T 2>nul
timeout /t 2 /nobreak >nul

REM Clean bin, obj, and TestResults folders
powershell -Command "Write-Host '=== Removing bin, obj, and TestResults folders ===' -ForegroundColor DarkCyan"
echo.

REM Remove bin folders
powershell -Command "Write-Host '=== Removing bin folders ===' -ForegroundColor DarkCyan"
set COUNT=0
for /d /r . %%d in (bin) do (
    if exist "%%d" (
        set /a COUNT+=1
        rd /s /q "%%d" 2>nul
    )
)
if !COUNT! gtr 0 (
    echo.
    powershell -Command "Write-Host 'Found !COUNT! bin folder(s)' -ForegroundColor DarkCyan"
    powershell -Command "Write-Host 'Removed bin folders' -ForegroundColor DarkGreen"
) else (
    echo No bin folders found
)

REM Remove obj folders with retry
powershell -Command "Write-Host '=== Removing obj folders ===' -ForegroundColor DarkCyan"
set OBJ_MAX=3
set OBJ_COUNT=0
:remove_obj_loop
set OBJ_FOUND=0
for /d /r . %%d in (obj) do (
    if exist "%%d" (
        set /a OBJ_FOUND+=1
        rd /s /q "%%d" 2>nul
    )
)
if !OBJ_FOUND! gtr 0 (
    echo.
    echo Found !OBJ_FOUND! obj folder(s)
    timeout /t 1 /nobreak >nul
    set /a OBJ_COUNT+=1
    if !OBJ_COUNT! lss !OBJ_MAX! (
        goto :remove_obj_loop
    )
    powershell -Command "Write-Host 'Removed obj folders' -ForegroundColor DarkGreen"
) else (
    if !OBJ_COUNT! equ 0 (
        echo No obj folders found
    ) else (
        echo.
        powershell -Command "Write-Host 'All obj folders removed' -ForegroundColor DarkGreen"
    )
)

REM Remove TestResults folders
powershell -Command "Write-Host '=== Removing TestResults folders ===' -ForegroundColor DarkCyan"
set COUNT=0
for /d /r . %%d in (TestResults) do (
    if exist "%%d" (
        set /a COUNT+=1
        echo Found TestResults folder: %%d
        rd /s /q "%%d" 2>nul
        if exist "%%d" (
            powershell -Command "Write-Host 'Failed to remove: %%d' -ForegroundColor DarkRed"
        ) else (
            powershell -Command "Write-Host 'Successfully removed: %%d' -ForegroundColor DarkGreen"
        )
    )
)
if !COUNT! gtr 0 (
    echo.
    powershell -Command "Write-Host 'Found !COUNT! TestResults folder(s)' -ForegroundColor DarkCyan"
    REM Check if any remain
    timeout /t 1 /nobreak >nul
    set REMAINING=0
    for /d /r . %%d in (TestResults) do (
        if exist "%%d" (
            set /a REMAINING+=1
            powershell -Command "Write-Host '  Folder remains: %%d' -ForegroundColor DarkYellow"
        )
    )
    if !REMAINING! gtr 0 (
        echo.
        powershell -Command "Write-Host '!REMAINING! TestResults folder(s)' -ForegroundColor DarkYellow"
        powershell -Command "Write-Host '  Files may be locked by another process' -ForegroundColor DarkYellow"
    ) else (
        echo.
        powershell -Command "Write-Host 'Removed TestResults folders' -ForegroundColor DarkGreen"
    )
) else (
    echo No TestResults folders found
)

REM Clean Visual Studio files
echo.
powershell -Command "Write-Host '=== Removing Visual Studio files ===' -ForegroundColor DarkCyan"

REM Remove .vs folder
if exist ".vs" (
    rd /s /q ".vs" 2>nul
    echo Removed .vs folder
) else (
    echo No .vs folder found
)

REM Remove *.user files
set COUNT=0
for /r . %%f in (*.user) do (
    if exist "%%f" (
        set /a COUNT+=1
        del /f /q "%%f" 2>nul
    )
)
if !COUNT! gtr 0 (
    echo Found !COUNT! .user file(s)
    echo Removed .user files
) else (
    echo No .user files found
)

REM Remove *.suo files
set COUNT=0
for /r . %%f in (*.suo) do (
    if exist "%%f" (
        set /a COUNT+=1
        del /f /q "%%f" 2>nul
    )
)
if !COUNT! gtr 0 (
    echo Found !COUNT! .suo file(s)
    echo Removed .suo files
) else (
    echo No .suo files found
)

REM Run dotnet clean
echo.
powershell -Command "Write-Host '=== Running dotnet clean ===' -ForegroundColor DarkCyan"
dotnet clean IAM_Service.sln --verbosity quiet
powershell -Command "Write-Host 'dotnet clean completed' -ForegroundColor DarkGreen"

REM Final cleanup - remove any obj and TestResults folders created by dotnet clean
echo.
powershell -Command "Write-Host '=== Final cleanup of obj and TestResults folders ===' -ForegroundColor DarkCyan"
timeout /t 2 /nobreak >nul

REM Clean obj folders
set COUNT=0
for /d /r . %%d in (obj) do (
    if exist "%%d" (
        set /a COUNT+=1
        rd /s /q "%%d" 2>nul
    )
)
if !COUNT! gtr 0 (
    echo.
    powershell -Command "Write-Host 'Found !COUNT! obj folder(s)' -ForegroundColor DarkCyan"
    powershell -Command "Write-Host 'Removed remaining obj folders' -ForegroundColor DarkGreen"
) else (
    echo No remaining obj folders
)

REM Clean TestResults folders
powershell -Command "Write-Host '=== Final cleanup of TestResults folders ===' -ForegroundColor DarkCyan"
set COUNT=0
for /d /r . %%d in (TestResults) do (
    if exist "%%d" (
        set /a COUNT+=1
        echo Found remaining TestResults folder: %%d
        rd /s /q "%%d" 2>nul
        if exist "%%d" (
            powershell -Command "Write-Host 'Failed to remove: %%d' -ForegroundColor DarkRed"
        ) else (
            powershell -Command "Write-Host 'Successfully removed: %%d' -ForegroundColor DarkGreen"
        )
    )
)
if !COUNT! gtr 0 (
    echo.
    powershell -Command "Write-Host 'Found !COUNT! TestResults folder(s)' -ForegroundColor DarkCyan"
    REM Check if any remain
    timeout /t 1 /nobreak >nul
    set REMAINING=0
    for /d /r . %%d in (TestResults) do (
        if exist "%%d" (
            set /a REMAINING+=1
            powershell -Command "Write-Host '  Folder remains: %%d' -ForegroundColor DarkYellow"
        )
    )
    if !REMAINING! gtr 0 (
        echo.
        powershell -Command "Write-Host '!REMAINING! TestResults folder(s)' -ForegroundColor DarkYellow"
        powershell -Command "Write-Host '  Files may be locked by another process' -ForegroundColor DarkYellow"
    ) else (
        echo.
        powershell -Command "Write-Host 'Removed remaining TestResults folders' -ForegroundColor DarkGreen"
    )
) else (
    echo No remaining TestResults folders
)

echo.
powershell -Command "Write-Host 'Clean completed successfully!' -ForegroundColor DarkGreen"
exit /b 0

