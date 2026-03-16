@ECHO off
SETLOCAL

SET FileName=%~n1

FOR /f "tokens=*" %%i in ('ffprobe -v error -select_streams a:0 -show_entries stream=bit_rate -of default=nokey=1:noprint_wrappers=1 "%~1"') do set BitRate=%%i

REM If bitrate extraction failed, set a default bitrate
IF "%BitRate%"=="" SET BitRate=192k

ffmpeg -i "%~1" -c:a aac -b:a %BitRate% -q:a 1 "%FileName%.m4a"

REM Lossy options
REM With metadata preservation and additional options
REM ffmpeg -i "input.wma" -c:a aac -b:a 192k -map_metadata 0 -movflags +faststart "output.m4a"

REM # With specific AAC encoder (if available)
REM ffmpeg -i "input.wma" -c:a libfdk_aac -b:a 192k -map_metadata 0 -movflags +faststart "output.m4a"

REM # With quality-based VBR encoding
REM ffmpeg -i "input.wma" -c:a aac -q:a 2 -map_metadata 0 -movflags +faststart "output.m4a"
REM ffmpeg -i "input.wma" -c:a aac -q:a 2 -minrate 128k -maxrate 320k "output.m4a"

REM # For best quality/size balance:
REM ffmpeg -i "input.wma" -c:a aac -q:a 2 -map_metadata 0 -movflags +faststart "output.m4a"

ECHO Conversion complete: %FileName%.m4a
ENDLOCAL
