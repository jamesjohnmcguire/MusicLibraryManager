@ECHO off
SETLOCAL EnableDelayedExpansion


if "%~2"=="" SET Message=Usage: <<media file1>>  <<media file2>>
if "%~2"=="" GOTO error

:: Store the file names without extension for working files
SET "File1Base=%~n1"
SET "File2Base=%~n2"
SET File1="%TEMP%\%File1Base%.Metadata.txt"
SET File2="%TEMP%\%File2Base%.Metadata.txt"

REM ... [previous code] ...

ECHO Extracting detailed metadata...
ECHO Full metadata for %~1: > "%File1%"
ffmpeg -i "%~1" 2>> "%File1%"
ECHO Additional technical details: >> "%File1%"
ffprobe -show_format -show_streams "%~1" 2>> "%File1%"

ECHO Full metadata for %~2: > "%File2%"
ffmpeg -i "%~2" 2>> "%File2%"
ECHO Additional technical details: >> "%File2%"
ffprobe -show_format -show_streams "%~2" 2>> "%File2%"

REM ... [rest of code] ...

;error
ECHO %Message%
EXIT /b 1

ENDLOCAL
