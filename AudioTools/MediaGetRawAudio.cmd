MediaGetRawAudio.cmd

	# Convert with specific sample rate and bit depth
	ffmpeg -i input.m4a -acodec pcm_s16le -ar 44100 output.wav

	# Convert with higher quality settings
	ffmpeg -i input.m4a -acodec pcm_s24le -ar 96000 output.wav

	# Convert while preserving metadata
	ffmpeg -i input.m4a -acodec pcm_s16le -map_metadata 0 output.wav

	# To PCM Raw (headerless)
	ffmpeg -i input.m4a -f s16le -acodec pcm_s16le output.raw
