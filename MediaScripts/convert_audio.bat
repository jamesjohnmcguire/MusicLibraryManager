@echo off
REM Batch script to run the Python converter
REM Created: 2025-02-06
REM Author: James John McGuire

setlocal enabledelayedexpansion

REM Check if Python is installed
python --version >nul 2>&1
if errorlevel 1 (
    echo Python is not installed. Please install Python 3.x
    exit /b 1
)

REM Check if FFmpeg is installed
ffmpeg -version >nul 2>&1
if errorlevel 1 (
    echo FFmpeg is not installed. Please install FFmpeg
    exit /b 1
)

REM Get input directory
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

pause