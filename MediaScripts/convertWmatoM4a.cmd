@ECHO off
SETLOCAL

:: Check if file is provided
IF "%~1"=="" SET Message=Usage: %~nx0 <<input file>>.wma
IF "%~1"=="" GOTO error

:: Extract the file name without extension
SET FileName=%~n1

:: Extract the source bitrate using ffprobe
for /f "tokens=*" %%i in ('ffprobe -v error -select_streams a:0 -show_entries stream=bit_rate -of default=nokey=1:noprint_wrappers=1 "%~1"') do set BitRate=%%i

:: If bitrate extraction failed, set a default bitrate
IF "%BitRate%"=="" ECHO Setting BitRate to Default of 192k
IF "%BitRate%"=="" PAUSE
IF "%BitRate%"=="" SET BitRate=192k

:: Convert the WMA file to M4A
::ffmpeg -i "%~1" -c:a aac -b:a %BitRate% -q:a 1 "%FileName%.m4a"
ffmpeg -i "%~1" -c:a aac -b:a %BitRate% "%FileName%.m4a"

ECHO Conversion complete: %FileName%.m4a
GOTO finish

;error
ECHO %Message%
EXIT /b 1

;finish
ENDLOCAL
