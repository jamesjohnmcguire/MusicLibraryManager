@ECHO OFF
SETLOCAL EnableDelayedExpansion

:: The script uses 16-bit, 44100Hz, stereo as the PCM parameters

if "%~1"=="-help" GOTO :help
if "%~1"=="--help" GOTO :help

ffmpeg -version >nul 2>&1
IF ERRORLEVEL 1 SET Message=FFmpeg is not installed. Please install FFmpeg
IF ERRORLEVEL 1 GOTO :error

IF "%~1"=="" SET "Message=Usage: %~nx0 ^<command^> ^<Input File 1^> ^<Input File 2^>"
IF "%~1"=="" GOTO :error
IF "%~2"=="" SET "Message=Usage: %~nx0 ^<command^> ^<Input File 1^> ^<Input File 2^>"
IF "%~2"=="" GOTO :error
IF "%~3"=="" SET "Message=Usage: %~nx0 ^<command^> ^<Input File 1^> ^<Input File 2^>"
IF "%~3"=="" GOTO :error

SET "File1=%~nx2"
SET "File2=%~nx3"
SET "File1Base=%~n2"
SET "File2Base=%~n3"

SET Normalize=false
if "%~2"=="--normalize" SET Normalize=true
if "%~2"=="--normalize" GOTO :check3
GOTO continue

:check3
if "%~4"=="" SET Message=Usage: %~nx0 --normalize <<media file1>> <<media file2>>
if "%~4"=="" GOTO :error
SET "File1=%~nx3"
SET "File2=%~nx4"
SET "File1Base=%~n3"
SET "File2Base=%~n4"
PAUSE

:continue
SET "TemporaryDirectory=%TEMP%\AudioCompare%RANDOM%"
MD %TemporaryDirectory%

:: Store the file names without extension for working files
SET File1Meta="%TemporaryDirectory%\%File1Base%.Metadata.txt"
SET File2Meta="%TemporaryDirectory%\%File2Base%.Metadata.txt"
SET DiffFile="%TemporaryDirectory%\MetadataDiff.txt"

ECHO Comparing files:
ECHO 1: %File1%
ECHO 2: %File2%
ECHO.

SET CheckFile=%File1%
CALL :audio-info

SET CheckFile=%File2%
CALL :audio-info

ECHO Extracting metadata...
ECHO Full metadata for %File1%: > "%File1Meta%"
ECHO Full metadata for %File2%: > "%File2Meta%"
:: ffmpeg -f ffmetadata %File2Meta% -i "%File2%" 2>nul
:: ffmpeg -f ffmetadata %File1Meta% -i "%File1%" 2>nul

ECHO Additional technical details: >> "%File1Meta%"
ECHO Additional technical details: >> "%File2Meta%"

ffprobe -show_format -show_streams "%File1%" 2>> "%File1Meta%"
ffprobe -show_format -show_streams "%File2%" 2>> "%File2Meta%"

ECHO Comparing metadata...
fc %File1Meta% %File2Meta% > %TemporaryDirectory%\MetaDataDiff.txt
IF %ERRORLEVEL% NEQ 0 ECHO Metadata differences found - see "%TemporaryDirectory%\metadata_diff.txt"
IF %ERRORLEVEL% EQU 0 ECHO No metadata differences found

fc "%File1Meta%" "%File2Meta%" > "%DiffFile%"
if errorlevel 1 SET Message=Metadata differences found - see "%DiffFile%"
if errorlevel 0 SET Message=No metadata differences found

ECHO.
ECHO Converting to raw PCM format...
SET Normalize=false
IF "%Normalize%"=="true" ECHO Normalize=true
IF "%Normalize%"=="false" ECHO Normalize=false

IF "%Normalize%"=="false" CALL ffmpeg -i "%File1%" "%TemporaryDirectory%\%File1Base%.wav" 2>nul
IF "%Normalize%"=="false" CALL ffmpeg -i "%File2%" "%TemporaryDirectory%\%File2Base%.wav" 2>nul
IF "%Normalize%"=="true" ffmpeg -i "%File1%" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 "%TemporaryDirectory%\%File1Base%.wav" 2>nul
IF "%Normalize%"=="true" ffmpeg -i "%File2%" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 "%TemporaryDirectory%\%File2Base%.raw" 2>nul

ECHO Comparing raw audio data...
fc /b "%TemporaryDirectory%\%File1Base%.wav" "%TemporaryDirectory%\%File2Base%.wav" > "%TemporaryDirectory%\audioDiff.txt"

IF %ERRORLEVEL% NEQ 0 ECHO Audio content differences found - see "%TemporaryDirectory%\audioDiff.txt"
IF %ERRORLEVEL% EQU 0 ECHO No audio content differences found

ECHO.
ECHO Process Complete
GOTO :finish

:audio-info
	SET "TempFile=%temp%\AudioInfo%random%.txt"

	ffprobe -v quiet -select_streams a:0 -show_entries stream=sample_rate,channels,bits_per_sample,codec_name,bit_rate -of csv=p=0 "%CheckFile%" 2>nul > "%TempFile%"

	IF NOT EXIST "%TempFile%" (
		ECHO Error: Could not get audio information
		EXIT /B 1
	)

	FOR /F "USEBACKQ TOKENS=1-5 DELIMS=," %%a IN ("%TempFile%") DO (
		ECHO   Codec: %%d
		ECHO   Sample Rate: %%a Hz
		ECHO   Channels: %%b
		ECHO   Bits per Sample: %%c
		ECHO   Bit Rate: %%e bps
	)

	DEL "%TempFile%" 2>nul
    GOTO :EOF

:help
	ECHO Audio Comparison Script
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

	GOTO :finish

:error
	ECHO %Message%
	ECHO.
	ECHO Try: %~nx0 --help for more information.
	EXIT /B 1

:finish
RD /S /Q "%TemporaryDirectory%"
ENDLOCAL
