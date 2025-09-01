@ECHO off
SETLOCAL EnableDelayedExpansion

:: The script assumes ffmpeg is in your system PATH
:: It uses 16-bit, 44100Hz, stereo as the PCM parameters

if "%~2"=="" SET Message=Usage: <<media file1>>  <<media file2>>
if "%~2"=="" GOTO error

SET "TempDirectory=%TEMP%\AudioCompare%RANDOM%"
MKDIR "%TempDirectory%"

:: Store the file names without extension for working files
SET "File1Base=%~n1"
SET "File2Base=%~n2"
SET File1="%TempDirectory%\%File1Base%.Metadata.txt"
SET File2="%TempDirectory%\%File2Base%.Metadata.txt"
SET DiffFile="%TempDirectory%\metadata_diff.txt"

ECHO Comparing files:
ECHO 1: %~1
ECHO 2: %~2
ECHO.

ECHO Extracting detailed metadata...
ECHO Full metadata for %~1: > "%File1%"
:: ffmpeg -i "%~1" -f ffmetadata "%temp_dir%\%file1_base%_metadata.txt" 2>nul
ffmpeg -i "%~1" 2>> "%File1%"

ECHO Additional technical details: >> "%File1%"
ffprobe -show_format -show_streams "%~1" 2>> "%File1%"

ECHO Full metadata for %~2: > "%File2%"
ffmpeg -i "%~2" 2>> "%File2%"
ECHO Additional technical details: >> "%File2%"
ffprobe -show_format -show_streams "%~2" 2>> "%File2%"

:: Note: Leaving temp files for inspection
:: To clean up, uncomment the following line:
:: rmdir /s /q "%TempDirectory%"

ECHO Comparing metadata...
fc "%File1%" "%File2%" > "%DiffFile%"
if errorlevel 1 SET Message=Metadata differences found - see "%DiffFile%"
if errorlevel 0 SET Message=No metadata differences found

ECHO %Message%
ECHO.

;error
ECHO %Message%
EXIT /b 1

ENDLOCAL
