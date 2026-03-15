@ECHO OFF
:: Converts a FLAC file to ALAC using FFmpeg
:: Usage: convert_flac_to_alac.bat input.flac output.m4a

SETLOCAL

IF "%~1"=="" SET error="Please provide the input FLAC file."
IF "%~1"=="" GOTO error

set "source=%~1"

IF "%~2"=="" (
	set "outputFile=%~dp1%~n1.flac.m4a"
) ELSE (
	set outputFile=%~2
)

:: ffmpeg          - The Program
:: -i "%source%"        - Specifies the Input file
:: -c:a alac       - Convert to ALAC Format
:: -c:v copy       - Copy Video and Images Streams, as well
:: -map_metadata 0 - Copy Metadata and Tags, as well
:: -map
::"%outputFile%"
ffmpeg -i "%source%" -c:a alac -c:v copy -map_metadata 0 -map 0 "%outputFile%"

IF %ERRORLEVEL% EQU 0 (
	ECHO Conversion successful: %outputFile%
) ELSE (
	ECHO Conversion failed.
)

GOTO finish

:error
ECHO %error%
ECHO Usage: %0 input.flac [output.m4a]
EXIT /B 1

:finish
ENDLOCAL
