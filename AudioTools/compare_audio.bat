@echo off
setlocal EnableDelayedExpansion

REM Check if two arguments were provided
if "%~2"=="" (
    echo Usage: %~nx0 file1.m4a file2.m4a
    echo Both files must be m4a format
    exit /b 1
)

REM Store the file names without extension for working files
set "file1_base=%~n1"
set "file2_base=%~n2"

echo Comparing files:
echo 1: %~1
echo 2: %~2
echo.

REM Create a temporary working directory
set "temp_dir=%TEMP%\audio_compare_%RANDOM%"
mkdir "%temp_dir%"

echo Extracting metadata...
ffmpeg -i "%~1" -f ffmetadata "%temp_dir%\%file1_base%_metadata.txt" 2>nul
ffmpeg -i "%~2" -f ffmetadata "%temp_dir%\%file2_base%_metadata.txt" 2>nul

echo Comparing metadata...
fc "%temp_dir%\%file1_base%_metadata.txt" "%temp_dir%\%file2_base%_metadata.txt" > "%temp_dir%\metadata_diff.txt"
if errorlevel 1 (
    echo Metadata differences found - see "%temp_dir%\metadata_diff.txt"
) else (
    echo No metadata differences found
)
echo.

echo Converting to raw PCM format...
ffmpeg -i "%~1" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 "%temp_dir%\%file1_base%.raw" 2>nul
ffmpeg -i "%~2" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 "%temp_dir%\%file2_base%.raw" 2>nul

echo Comparing raw audio data...
fc /b "%temp_dir%\%file1_base%.raw" "%temp_dir%\%file2_base%.raw" > "%temp_dir%\audio_diff.txt"
if errorlevel 1 (
    echo Audio content differences found - see "%temp_dir%\audio_diff.txt"
) else (
    echo No audio content differences found
)

echo.
echo Temporary files stored in: %temp_dir%
echo.
echo Files created:
echo - Metadata files: %file1_base%_metadata.txt, %file2_base%_metadata.txt
echo - Raw audio files: %file1_base%.raw, %file2_base%.raw
echo - Comparison results: metadata_diff.txt, audio_diff.txt

REM Note: Leaving temp files for inspection
REM To clean up, uncomment the following line:
REM rmdir /s /q "%temp_dir%"

endlocal