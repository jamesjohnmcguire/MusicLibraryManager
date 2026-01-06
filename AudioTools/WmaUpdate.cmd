@ECHO OFF
SETLOCAL

REM Check if the input file is provided
if "%~1"=="" (
	echo Usage: WmaUpdate input_file.wma
	exit /b 1
)

REM Extract the file name without extension
set FileName=%~n1

REM Extract the source bitrate using ffprobe
for /f "tokens=*" %%i in ('ffprobe -v error -select_streams a:0 -show_entries stream=bit_rate -of default=nokey=1:noprint_wrappers=1 "%~1"') do set BitRate=%%i

REM If bitrate extraction failed, set a default bitrate
if "%BitRate%"=="" ECHO Unable to obtain bitrate, setting to 192K
if "%BitRate%"=="" set BitRate=192k
PAUSE

REM Convert the WMA file to M4A using the extracted bitrate
ffmpeg -i "%~1" -c:a aac -b:a %BitRate% "%FileName%.m4a"

echo Conversion complete: %FileName%.m4a
ENDLOCAL
