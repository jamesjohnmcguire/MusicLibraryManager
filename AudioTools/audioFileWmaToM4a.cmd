@ECHO OFF
SETLOCAL

ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET Message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO :error

IF "%~1"=="" SET "Message=Usage: %~nx0 ^<Input File^>.wma"
IF "%~1"=="" GOTO :error

:: Extract the file name without extension
SET FileName=%~n1

:: Get bitrate
for /f "tokens=*" %%i in ('ffprobe -v error -select_streams a:0 -show_entries stream=bit_rate -of default=nokey=1:noprint_wrappers=1 "%~1"') do set BitRate=%%i

:: Get codec
for /f "tokens=*" %%i in ('ffprobe -v error -select_streams a:0 -show_entries stream=codec_name -of default=nokey=1:noprint_wrappers=1 "%~1"') do set CodecName=%%i

IF /I "%CodecName%"=="wmalossless" GOTO :use_lossless

GOTO :use_vbr

:use_cbr
:: If bitrate extraction failed, set a default bitrate
IF "%BitRate%"=="" ECHO Setting BitRate to Default of 192k
IF "%BitRate%"=="" PAUSE
IF "%BitRate%"=="" SET BitRate=192k

:: Convert the WMA file to M4A
::ffmpeg -i "%~1" -c:a aac -b:a %BitRate% "%FileName%.m4a"
::ffmpeg                The Program to Use
:: -i "%~1"             The Input File
:: -c:a aac             Use the Default AAC encoder
:: -b:a %BitRate%       Set the Bit-rate for the Output File
:: -ar 44100            Ensure the Output Sample Rate to Standard 44,100 Hz
:: -movflags +faststart Moves the MP4 index (moov atom) to the beginning of the file
:: -c:v copy            Copies the video/art stream as-is without re-encoding it
:: -map_metadata 0      Copies all metadata (tags) from the input file to the Output File
:: "%FileName%.m4a"  The Output File

ffmpeg -i "%~1" -c:a aac -b:a %BitRate% -ar 44100 -movflags +faststart -c:v copy -map_metadata 0 "%FileName%.m4a"

GOTO :finish

:use_vbr

:: Convert the WMA file to M4A
::ffmpeg -i "%~1" -c:a aac -b:a %BitRate% "%FileName%.m4a"
::ffmpeg                The Program to Use
:: -i "%~1"             The Input File
:: -c:a aac             Use the Default AAC encoder
:: -q:a 0.1             Set to the Highest Quality
:: -ar 44100            Ensure the Output Sample Rate to Standard 44,100 Hz
:: -movflags +faststart Moves the MP4 index (moov atom) to the beginning of the file
:: -c:v copy            Copies the video/art stream as-is without re-encoding it
:: -map_metadata 0      Copies all metadata (tags) from the input file to the Output File
:: "%FileName%.m4a"  The Output File

ffmpeg -i "%~1" -c:a aac -q:a 0.1 -ar 44100 -movflags +faststart -c:v copy -map_metadata 0 "%FileName%.m4a"

GOTO :finish

:use_libfdk

:: Convert the WMA file to M4A
::ffmpeg -i "%~1" -c:a aac -b:a %BitRate% "%FileName%.m4a"
::ffmpeg                The Program to Use
:: -i "%~1"             The Input File
:: -c:a libfdk_aac      Use the Fraunhofer FDK AAC encoder - Highest Quality
:: -vbr 5               Set to the Highest Quality
:: -ar 44100            Ensure the Output Sample Rate to Standard 44,100 Hz
:: -movflags +faststart Moves the MP4 index (moov atom) to the beginning of the file
:: -c:v copy            Copies the video/art stream as-is without re-encoding it
:: -map_metadata 0      Copies all metadata (tags) from the input file to the Output File
:: "%FileName%.m4a"  The Output File
ffmpeg -i "%~1" -c:a libfdk_aac -vbr 5 -ar 44100 -movflags +faststart -c:v copy -map_metadata 0 "%FileName%.m4a"

GOTO :finish

:use_lossless
ffmpeg -i "%~1" -c:a alac -q:a 0.1 -ar 44100 -movflags +faststart -c:v copy -map_metadata 0 "%FileName%.m4a"

GOTO :finish

ECHO Conversion complete: %FileName%.m4a
GOTO finish

:error
ECHO %Message%
EXIT /b 1

:finish
ENDLOCAL
