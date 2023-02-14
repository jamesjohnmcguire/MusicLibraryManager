#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <sstream>
#include <chrono>

#define USE_SWRESAMPLE
#include "..\ChromaPrint\src\chromaprint.h"
#include "..\ChromaPrint\src/audio/ffmpeg_audio_reader.h"

using namespace chromaprint;

#include <iostream>

#include "FingerPrinter.h"

namespace FingerPrinter
{
	char* FingerPrint(const wchar_t* filePath)
	{
		char* result = nullptr;

		FFmpegAudioReader reader;

		ChromaprintContext* chromaprint_ctx =
			chromaprint_new(CHROMAPRINT_ALGORITHM_DEFAULT);

		auto channels = chromaprint_get_num_channels(chromaprint_ctx);
		auto sampleRate = chromaprint_get_sample_rate(chromaprint_ctx);
		reader.SetOutputChannels(channels);
		reader.SetOutputSampleRate(sampleRate);

		//ProcessFile(chromaprint_ctx, reader, argv[i]);

		chromaprint_free(chromaprint_ctx);
		return result;
	}
}
