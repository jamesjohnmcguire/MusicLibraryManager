@ECHO OFF
SETLOCAL

:: Converts a FLAC file to ALAC using FFmpeg
:: Usage: %~nx0 input.flac output.m4a

ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET Message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO error

IF "%~1"=="" SET "Message=Usage: %~nx0 ^<Input File^>.flac"
IF "%~1"=="" GOTO :error

set "source=%~1"

IF "%~2"=="" SET "outputFile=%~dp1%~n1.flac.m4a"
IF NOT "%~2"=="" SET outputFile=%~2

ffmpeg -i "%source%" -c:a alac -c:v copy -map_metadata 0 -map 0 "%outputFile%"

IF %ERRORLEVEL% EQU 0 ECHO Conversion successful: %outputFile%
IF %ERRORLEVEL% NEQ 0 ECHO Conversion failed.

GOTO finish

:error
ECHO %Message%
ECHO Usage: %0 input.flac [output.m4a]
EXIT /B 1

:finish
ENDLOCAL
