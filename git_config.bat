@echo off
REM Git Configuration Loader
REM This script reads git_config.txt and loads the variables

if not exist "git_config.txt" (
    echo Error: git_config.txt not found!
    pause
    exit /b 1
)

REM Read each line from git_config.txt
REM Skip lines that start with # or are empty
for /f "usebackq eol=# tokens=1,* delims==" %%a in ("git_config.txt") do (
    if not "%%b"=="" (
        set "%%a=%%b"
    )
)

echo Git configuration loaded from git_config.txt
