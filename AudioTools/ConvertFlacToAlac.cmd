Convert Flac
	@echo off
	REM Converts a FLAC file to AAC using FFmpeg
	REM Usage: convert_flac_to_aac.bat input.flac output.aac

	IF "%~1"=="" (
		echo Please provide the input FLAC file.
		echo Usage: %0 input.flac [output.aac]
		exit /b 1
	)

	IF "%~2"=="" (
		set OUTPUT_FILE=%~n1.aac
	) ELSE (
		set OUTPUT_FILE=%~2
	)

	ffmpeg -i "%~1" -c:a aac -b:a 192k "%OUTPUT_FILE%"

	IF %ERRORLEVEL% EQU 0 (
		echo Conversion successful: %OUTPUT_FILE%
	) ELSE (
		echo Conversion failed.
	)
	pause

	convert_flac_to_alac.bat input.flac output.m4a
