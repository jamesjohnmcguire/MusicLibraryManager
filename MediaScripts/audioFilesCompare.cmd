@ECHO off
SETLOCAL EnableDelayedExpansion

:: The script assumes ffmpeg is in your system PATH
:: It uses 16-bit, 44100Hz, stereo as the PCM parameters

if "%~1"=="" GOTO error
if "%~1"=="--help" GOTO error

SET Normalize=false
if "%~1"=="--normalize" SET Normalize=true

if "%~2"=="" SET Message=Usage: %~nx0 <<media file1>> <<media file2>>
if "%~2"=="" GOTO error

if "%~1"=="--normalize" GOTO check3
GOTO continue
:check3

if "%~3"=="" SET Message=Usage: %~nx0 --normalize <<media file1>> <<media file2>>
if "%~3"=="" GOTO error

:continue
SET "TempDirectory=%TEMP%\AudioCompare%RANDOM%"
MKDIR "%TempDirectory%"

:: Store the file names without extension for working files
SET "File1Base=%~n1"
SET "File2Base=%~n2"
SET File1="%TempDirectory%\%File1Base%.Metadata.txt"
SET File2="%TempDirectory%\%File2Base%.Metadata.txt"
SET DiffFile="%TempDirectory%\metadata_diff.txt"

ECHO Comparing files:
ECHO 1: %~1
ECHO 2: %~2
ECHO.

ECHO Extracting detailed metadata...
ECHO Full metadata for %~1: > "%File1%"
:: ffmpeg -i "%~1" -f ffmetadata "%temp_dir%\%file1_base%_metadata.txt" 2>nul
ffmpeg -i "%~1" 2>> "%File1%"

ECHO Additional technical details: >> "%File1%"
ffprobe -show_format -show_streams "%~1" 2>> "%File1%"

ECHO Full metadata for %~2: > "%File2%"
ffmpeg -i "%~2" 2>> "%File2%"
ECHO Additional technical details: >> "%File2%"
ffprobe -show_format -show_streams "%~2" 2>> "%File2%"

:: Note: Leaving temp files for inspection
:: To clean up, uncomment the following line:
:: rmdir /s /q "%TempDirectory%"

ECHO Comparing metadata...
fc "%File1%" "%File2%" > "%DiffFile%"
if errorlevel 1 SET Message=Metadata differences found - see "%DiffFile%"
if errorlevel 0 SET Message=No metadata differences found

ECHO %Message%
ECHO.
GOTO finish

:audio-info
	SET "TempFile=%temp%\audio_info_%random%.txt"

	ffprobe -v quiet -select_streams a:0 -show_entries stream=sample_rate,channels,bits_per_sample,codec_name,bit_rate -of csv=p=0 "%file%" 2>nul > "%TempFile%"

	if not exist "%TempFile%" (
		echo Error: Could not get audio information
		exit /b 1
	)

	for /f "usebackq tokens=1-5 delims=," %%a in ("%TempFile%") do (
		echo   Codec: %%d
		echo   Sample Rate: %%a Hz
		echo   Channels: %%b
		echo   Bits per Sample: %%c
		echo   Bit Rate: %%e bps
	)

	DEL "%TempFile%" 2>nul

:help
	ECHO Audio PCM Comparison Script
	ECHO.
	ECHO Usage: %~nx0 [command] [--normalize] file1 file2
	ECHO.
	ECHO Command:
	ECHO audio       Compares the raw audio (PCM) data.
	ECHO metadata    Compares the metadata within the file.
	ECHO.
	ECHO Options:
	ECHO --normalize Normalize both files to same format before comparison
	ECHO             (44.1kHz, 16-bit, stereo)
	ECHO --help      Show this help message
	ECHO.
	ECHO Without --normalize, compares files in their native formats
	ECHO and shows technical differences.

	GOTO finish

;error
	ECHO %Message%
	ECHO.
	ECHO Try: %~nx0 --help for more information.
	EXIT /b 1

;finish
ENDLOCAL
