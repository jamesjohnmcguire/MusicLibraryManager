@ECHO off
SETLOCAL EnableDelayedExpansion

:: Check if file is provided
if "%~1"=="" SET Message=Usage: %~nx0 input.m4a
if "%~1"=="" GOTO error

REM Get codec info
ffprobe -v error -select_streams a:0 -show_entries stream=codec_name -of default=noprint_wrappers=1:nokey=1 "%~1" > codec.tmp
set /p CODEC=<codec.tmp
del codec.tmp

REM Check if it's ALAC
if /i "%CODEC%" == "alac" (
    echo Input file is already lossless (ALAC)
    
    REM Convert to FLAC
    echo Converting to FLAC...
    ffmpeg -i "%~1" -c:a flac -compression_level 8 "%~n1.flac"
    
    echo Done! Created: %~n1.flac
) else (
    echo Warning: Input file is not ALAC. Codec detected: %CODEC%
    echo Conversion not recommended as source may not be lossless
)

GOTO finish

;error
ECHO %Message%
EXIT /b 1

;finish
ENDLOCAL
