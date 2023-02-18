#include <cstdio>
#include <cstdlib>
//#include <cstring>
#include <sstream>
//#include <chrono>

#include "spdlog/spdlog.h"
#include "spdlog/sinks/basic_file_sink.h"
#include "spdlog/sinks/stdout_sinks.h"
#include "spdlog/sinks/stdout_color_sinks.h"

#define USE_SWRESAMPLE
#include "..\ChromaPrint\src\chromaprint.h"
#include "..\ChromaPrint\src/audio/ffmpeg_audio_reader.h"

using namespace chromaprint;

#include <iostream>

#include "FingerPrinter.h"

namespace FingerPrinter
{
	spdlog::logger GetLogger();
	bool IsStreamDone(size_t streamLimit, size_t streamSize, size_t frameSize);

	char* GetFingerPrint(
		ChromaprintContext* context,
		FFmpegAudioReader& reader,
		bool first,
		double timestamp,
		double duration)
	{
		int size;

		if (!chromaprint_get_raw_fingerprint_size(context, &size))
		{
			fprintf(stderr, "ERROR: Could not get the fingerprinting size\n");
			exit(2);
		}

		if (size <= 0)
		{
			if (first)
			{
				fprintf(stderr, "ERROR: Empty fingerprint\n");
				exit(2);
			}
		}

		char* tmp_fp2;
		if (!chromaprint_get_fingerprint(context, &tmp_fp2))
		{
			fprintf(stderr, "ERROR: Could not get the fingerprinting\n");
			exit(2);
		}

		return tmp_fp2;
	}

	char* FingerPrint(const char* filePath)
	{
		char* result = nullptr;

		spdlog::logger logger = GetLogger();

		FFmpegAudioReader reader;

		ChromaprintContext* context =
			chromaprint_new(CHROMAPRINT_ALGORITHM_DEFAULT);

		double ts = 0.0;
		double maxDuration = 120;
		double maxChunkDuration = 0;
		bool overlap = false;

		int channels = chromaprint_get_num_channels(context);
		int sampleRate = chromaprint_get_sample_rate(context);
		reader.SetOutputChannels(channels);
		reader.SetOutputSampleRate(sampleRate);

		if (!reader.Open(filePath))
		{
			std::string error = "ERROR: " + reader.GetError();
			logger.error(error);
		}
		else
		{
			channels = reader.GetChannels();
			sampleRate = reader.GetSampleRate();
			int checkResult = chromaprint_start(context, sampleRate, channels);

			if (checkResult == 0)
			{
				std::string error =
					"ERROR: Could not initialize the fingerprinting process";
				logger.error(error);
			}
			else
			{
				size_t chunk_size = 0;
				size_t stream_size = 0;

				const size_t stream_limit = maxDuration * sampleRate;
				const size_t chunk_limit = maxChunkDuration * sampleRate;
				size_t extra_chunk_limit = 0;
				double overlapAmount = 0.0;

				if (chunk_limit > 0 && overlap)
				{
					extra_chunk_limit = chromaprint_get_delay(context);
					overlapAmount = chromaprint_get_delay_ms(context) / 1000.0;
				}

				bool first_chunk = true;
				bool read_failed = false;
				bool got_results = false;

				while (!reader.IsFinished())
				{
					const int16_t* frame_data = nullptr;
					size_t frame_size = 0;
					bool check = reader.Read(&frame_data, &frame_size);

					if (check == false)
					{
						std::string error = "ERROR: " + reader.GetError();
						logger.error(error);
						read_failed = true;
						break;
					}

					bool streamDone =
						IsStreamDone(stream_limit, stream_size, frame_size);

					if (stream_limit > 0)
					{
						const auto remaining = stream_limit - stream_size;
						if (frame_size > remaining)
						{
							frame_size = remaining;
							streamDone = true;
						}
					}
					stream_size += frame_size;

					if (frame_size == 0)
					{
						if (streamDone)
						{
							break;
						}
						else
						{
							continue;
						}
					}

					bool chunk_done = false;
					size_t first_part_size = frame_size;
					if (chunk_limit > 0)
					{
						const auto remaining =
							chunk_limit + extra_chunk_limit - chunk_size;

						if (first_part_size > remaining)
						{
							first_part_size = remaining;
							chunk_done = true;
						}
					}

					auto sizing = first_part_size * channels;
					auto result = chromaprint_feed(context, frame_data, sizing);

					if (result == 0)
					{
						fprintf(stderr, "ERROR: Could not process audio data\n");
						exit(2);
					}

					chunk_size += first_part_size;

					if (chunk_done)
					{
						if (!chromaprint_finish(context))
						{
							fprintf(stderr, "ERROR: Could not finish the fingerprinting process\n");
							exit(2);
						}

						const auto chunk_duration = (chunk_size - extra_chunk_limit) * 1.0 / reader.GetSampleRate() + overlap;
						char* finger = GetFingerPrint(context, reader, first_chunk, ts, chunk_duration);

						got_results = true;

						ts += chunk_duration;

						if (!chromaprint_start(context, reader.GetSampleRate(), reader.GetChannels())) {
							fprintf(stderr, "ERROR: Could not initialize the fingerprinting process\n");
							exit(2);
						}

						if (first_chunk) {
							extra_chunk_limit = 0;
							first_chunk = false;
						}

						chunk_size = 0;
					}

					frame_data += first_part_size * reader.GetChannels();
					frame_size -= first_part_size;

					if (frame_size > 0)
					{
						if (!chromaprint_feed(context, frame_data, frame_size * reader.GetChannels())) {
							fprintf(stderr, "ERROR: Could not process audio data\n");
							exit(2);
						}
					}

					chunk_size += frame_size;

					if (streamDone)
					{
						break;
					}
				}

				if (!chromaprint_finish(context)) {
					fprintf(stderr, "ERROR: Could not finish the fingerprinting process\n");
					exit(2);
				}

				if (chunk_size > 0) {
					const auto chunk_duration = (chunk_size - extra_chunk_limit) * 1.0 / reader.GetSampleRate() + overlap;
					result = GetFingerPrint(
						context, reader, first_chunk, ts, chunk_duration);
					got_results = true;
				}
				else if (first_chunk) {
					fprintf(stderr, "ERROR: Not enough audio data\n");
					exit(2);
				}
			}
		}

		reader.Close();
		chromaprint_free(context);
		return result;
	}

	spdlog::logger GetLogger()
	{
		std::vector<spdlog::sink_ptr> sinks;

		std::shared_ptr<spdlog::sinks::stdout_sink_st> console_log =
			std::make_shared<spdlog::sinks::stdout_sink_st>();
		sinks.push_back(console_log);

		std::shared_ptr<spdlog::sinks::basic_file_sink_st> file_log =
			std::make_shared<spdlog::sinks::basic_file_sink_st>("logfile");
		sinks.push_back(file_log);

		spdlog::logger logger =
			spdlog::logger("log", begin(sinks), end(sinks));

		return logger;
	}

	bool IsStreamDone(size_t streamLimit, size_t streamSize, size_t frameSize)
	{
		bool streamDone = false;

		if (streamLimit > 0)
		{
			const size_t remaining = streamLimit - streamSize;

			if (frameSize > remaining)
			{
				streamDone = true;
			}
		}

		return streamDone;
	}
}
