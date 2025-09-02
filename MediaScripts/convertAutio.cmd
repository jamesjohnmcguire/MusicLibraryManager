@ECHO off
SETLOCAL enabledelayedexpansion

REM Check if Python is installed
python --version >nul 2>&1
if errorlevel 1 (
    echo Python is not installed. Please install Python 3.x
    exit /b 1
)

REM Check if FFmpeg is installed
ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET Message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO error

set "INPUT_DIR=%~1"
if "%INPUT_DIR%"=="" (
    set "INPUT_DIR=%CD%"
)

REM Set default format
set "FORMAT=m4a"
if not "%~2"=="" (
    set "FORMAT=%~2"
)

REM Run the Python script
python convert_audio.py "%INPUT_DIR%" --format %FORMAT% --quality high

GOTO finish

;error
ECHO %Message%
EXIT /b 1

;finish
ENDLOCAL
