@ECHO OFF
SETLOCAL

ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET Message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO error

IF "%~1"=="" SET "Message=Usage: %~nx0 ^<Input File^>"
IF "%~1"=="" GOTO :error

set FileName=%~n1

ffmpeg -i %1 %FileName%.wav

GOTO :finish

:error
ECHO %Message%
EXIT /b 1

:finish
ENDLOCAL
