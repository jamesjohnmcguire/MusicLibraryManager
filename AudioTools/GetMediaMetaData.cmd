	To see this metadata in detail, you can use various tools:

	bash
	# Using FFmpeg
	ffmpeg -i file.m4a -f ffmetadata metadata.txt

	# Using MediaInfo (more detailed)
	mediainfo file.m4a

	# Using AtomicParsley (specifically for MPEG-4 containers)
	AtomicParsley file.m4a -t

	# Using ExifTool (very comprehensive)
	exiftool file.m4a

