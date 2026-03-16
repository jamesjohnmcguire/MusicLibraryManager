@ECHO OFF
SETLOCAL

ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET Message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO error

IF "%~1"=="" SET "Message=Usage: %~nx0 ^<Input File^>.m4a"
IF "%~1"=="" GOTO :error

REM Get codec info
ffprobe -v error -select_streams a:0 -show_entries stream=codec_name -of default=noprint_wrappers=1:nokey=1 "%~1" > codec.tmp
SET /P CODEC=<codec.tmp
DEL codec.tmp

if NOT "%CODEC%" == "alac" ECHO Warning: Input file is not ALAC. Codec detected: %CODEC%

ECHO Converting to FLAC...
ffmpeg -i "%~1" -c:a flac -compression_level 8 "%~n1.flac"

ECHO Done! Created: %~n1.flac

GOTO finish

:error
ECHO %Message%
EXIT /B 1

:finish
ENDLOCAL
