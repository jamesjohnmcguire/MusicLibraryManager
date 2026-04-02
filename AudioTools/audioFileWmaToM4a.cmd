@ECHO OFF
SETLOCAL

ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO :error

IF "%~1"=="" SET "message=Usage: %~nx0 ^<Input File^>.wma"
IF "%~1"=="" GOTO :error

IF NOT EXIST "%~1" SET "message=File not found: %~1"
IF NOT EXIST "%~1" GOTO :error

IF /I NOT "%~x1"==".wma" SET "message=Error: Input file must have .wma extension"
IF /I NOT "%~x1"==".wma"  GOTO :error

:: Extract the file name without extension
SET fileName=%~n1

SET defaultOptions=-movflags +faststart -c:v copy -map 0:a -map_metadata 0 "%FileName%.m4a"

:: Get bitrate
FOR /F "tokens=*" %%i IN ('ffprobe -v error -select_streams a:0 -show_entries stream=bit_rate -of default=nokey=1:noprint_wrappers=1 "%~1"') DO SET bitRate=%%i
PAUSE

:: Get codec
FOR /F "tokens=*" %%i IN ('ffprobe -v error -select_streams a:0 -show_entries stream=codec_name -of default=nokey=1:noprint_wrappers=1 "%~1"') DO SET codecName=%%i
PAUSE

IF /I "%codec%"=="wmav2" GOTO :ok
IF /I "%codec%"=="wmav1" GOTO :ok
IF /I "%codec%"=="wmapro" GOTO :ok
IF /I "%codec%"=="wmalossless" GOTO :ok

SET "message=Error: File is not a valid WMA audio stream (detected: %codec%)"
GOTO :error

:ok

ffmpeg -encoders 2>nul | findstr /I "libfdk_aac" >nul

:: Use the Fraunhofer FDK AAC encoder - Highest Quality
IF %ERRORLEVEL% EQU 0 SET encoder=libfdk_aac
IF %ERRORLEVEL% EQU 0 SET bitRateOptions=-vbr 5 -profile:a aac_low
IF %ERRORLEVEL% EQU 0 GOTO :run
:: Else
IF %ERRORLEVEL% NEQ 0 SET encoder=aac
PAUSE

IF /I "%codecName%"=="wmalossless" SET encoder=alac
IF /I "%codecName%"=="wmalossless" SET bitRateOptions=-q:a 0.1
PAUSE

GOTO :use_vbr

:use_cbr
:: If bitrate extraction failed, set a default bitrate
IF "%bitRate%"=="" ECHO BitRate extraction failed - falling back to VBR
IF "%bitRate%"=="" PAUSE
IF "%bitRate%"=="" GOTO :use_vbr

SET bitRateOptions=-b:a %bitRate%
GOTO :run

:use_vbr

SET bitRateOptions=-q:a 0.1 -profile:a aac_low
GOTO :run

:run

:: Convert the WMA file to M4A
::ffmpeg -i "%~1" -c:a aac -b:a %BitRate% "%FileName%.m4a"
::ffmpeg                The Program to Use
:: -i "%~1"             The Input File
:: -c:a %encoder%       The Encoder to Use
:: -vbr 5               Set to the Highest Quality
:: -ar 44100            Ensure the Output Sample Rate to Standard 44,100 Hz
:: -movflags +faststart Moves the MP4 index (moov atom) to the beginning of the file
:: -c:v copy            Copies the video/art stream as-is without re-encoding it
:: -map_metadata 0      Copies all metadata (tags) from the input file to the Output File
:: "%FileName%.m4a"  The Output File
ECHO Using Encoder: %encoder%
ffmpeg -i "%~1" -c:a %encoder% %bitRateOptions% %defaultOptions%

ECHO Conversion complete: %FileName%.m4a
GOTO :finish

:error
ECHO %Message%
EXIT /b 1

:finish
ENDLOCAL
