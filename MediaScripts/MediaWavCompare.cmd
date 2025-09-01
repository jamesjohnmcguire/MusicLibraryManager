@echo off
setlocal EnableDelayedExpansion

IF "%~2"=="" ECHO Usage: %~nx0 file1.m4a file2.m4a
IF "%~2"=="" ECHO Both files must be m4a format

if "%~2"=="" EXIT /b 1

SET "FileBase1=%~n1"
SET "FileBase2=%~n2"

ECHO Comparing files:
ECHO 1: %~1
ECHO 2: %~2
ECHO.

SET TemporaryDirectory=%TEMP%\AudioCompare%RANDOM%"
MD %TemporaryDirectory%

ECHO Extracting metadata...
ffmpeg -i "%~1" -f ffmetadata %TemporaryDirectory%\%FileBase1%MetaData.txt 2>nul
ffmpeg -i "%~2" -f ffmetadata %TemporaryDirectory%\%FileBase2%MetaData.txt 2>nul

ECHO Comparing metadata...
fc "%TemporaryDirectory%\%FileBase1%MetaData.txt" "%TemporaryDirectory%\%FileBase2%MetaData.txt" > %TemporaryDirectory%\MetaDataDiff.txt
IF %ERRORLEVEL% NEQ 0 ECHO Metadata differences found - see "%TemporaryDirectory%\metadata_diff.txt"
IF %ERRORLEVEL% EQU 0 ECHO No metadata differences found

ECHO.
ECHO Converting to raw PCM format...
ffmpeg -i "%~1" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 "%TemporaryDirectory%\%file1_base%.raw" 2>nul
ffmpeg -i "%~2" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 "%TemporaryDirectory%\%file2_base%.raw" 2>nul

ECHO Comparing raw audio data...
fc /b "%TemporaryDirectory%\%file1_base%.raw" "%TemporaryDirectory%\%file2_base%.raw" > "%TemporaryDirectory%\audio_diff.txt"

IF %ERRORLEVEL% NEQ 0 ECHO Audio content differences found - see "%TemporaryDirectory%\audio_diff.txt"
IF %ERRORLEVEL% EQU 0 ECHO No audio content differences found

RD /s /q "%TemporaryDirectory%"
endlocal
