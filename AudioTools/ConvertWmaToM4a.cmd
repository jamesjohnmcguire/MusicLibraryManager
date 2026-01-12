
	ffmpeg -i "input.wma" -c:a aac -q:a 2 -minrate 128k -maxrate 320k "output.m4a"
	ffmpeg -i "input.wma" -c:a aac -q:a 2 -map_metadata 0 -movflags +faststart "output.m4a"

	ffmpeg -i "Black Is Black.wma" -c:a aac -q:a 2 -map_metadata 0 -movflags +faststart "NewTest.m4a"

	lossy
	# Convert to AAC (M4A)
	ffmpeg -i "Black Is Black.wma" -c:a aac -b:a 192k "Black Is Black.m4a"
