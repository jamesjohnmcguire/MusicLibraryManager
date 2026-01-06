@echo off
setlocal EnableDelayedExpansion

REM ... [previous code] ...

echo Extracting detailed metadata...
echo Full metadata for %~1: > "%temp_dir%\%file1_base%_full_metadata.txt"
ffmpeg -i "%~1" 2>> "%temp_dir%\%file1_base%_full_metadata.txt"
echo Additional technical details: >> "%temp_dir%\%file1_base%_full_metadata.txt"
ffprobe -show_format -show_streams "%~1" 2>> "%temp_dir%\%file1_base%_full_metadata.txt"

echo Full metadata for %~2: > "%temp_dir%\%file2_base%_full_metadata.txt"
ffmpeg -i "%~2" 2>> "%temp_dir%\%file2_base%_full_metadata.txt"
echo Additional technical details: >> "%temp_dir%\%file2_base%_full_metadata.txt"
ffprobe -show_format -show_streams "%~2" 2>> "%temp_dir%\%file2_base%_full_metadata.txt"

REM ... [rest of code] ...