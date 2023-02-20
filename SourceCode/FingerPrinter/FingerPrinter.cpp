﻿#include <filesystem>
#include <iostream>

#pragma warning(push 0)
#include "spdlog/spdlog.h"
#include "spdlog/sinks/basic_file_sink.h"
#include "spdlog/sinks/stdout_sinks.h"
#include "spdlog/sinks/stdout_color_sinks.h"

#define USE_SWRESAMPLE
#include "../ChromaPrint/src/chromaprint.h"
#include "../ChromaPrint/src/audio/ffmpeg_audio_reader.h"
#pragma warning(pop)

#include "FingerPrinter.h"

using namespace chromaprint;

namespace FingerPrinter
{
	char* GetFingerPrint(
		ChromaprintContext* context,
		FFmpegAudioReader& reader,
		bool first,
		double timestamp,
		double duration,
		spdlog::logger);
	size_t GetFirstPartSize(
		size_t frameSize,
		size_t chunkLimit,
		size_t extraChunkLimit,
		size_t chunkSize);
	size_t GetFrameSize(
		size_t streamLimit, size_t streamSize, size_t frameSize);
	spdlog::logger GetLogger();
	bool IsChunkDone(
		size_t frameSize,
		size_t chunkLimit,
		size_t extraChunkLimit,
		size_t chunkSize);
	bool IsStreamDone(size_t streamLimit, size_t streamSize, size_t frameSize);

	void FreeFingerPrint(char* data)
	{
		if (data != nullptr)
		{
			free(data);
		}
	}

	char* FingerPrint(const char* filePath)
	{
		char* result = nullptr;

		spdlog::logger logger = GetLogger();

		if (filePath != nullptr && std::filesystem::exists(filePath))
		{
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
				int checkResult =
					chromaprint_start(context, sampleRate, channels);

				if (checkResult == 0)
				{
					std::string error =
						"Could not initialize the fingerprinting process";
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
						overlapAmount =
							chromaprint_get_delay_ms(context) / 1000.0;
					}

					bool first_chunk = true;
					bool read_failed = false;

					while (!reader.IsFinished())
					{
						const int16_t* frame_data = nullptr;
						size_t frame_size = 0;
						bool check = reader.Read(&frame_data, &frame_size);

						if (check == false)
						{
							std::string error = reader.GetError();
							logger.error(error);
							read_failed = true;
							break;
						}

						bool streamDone =
							IsStreamDone(stream_limit, stream_size, frame_size);
						frame_size =
							GetFrameSize(stream_limit, stream_size, frame_size);
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

						bool chunk_done = IsChunkDone(
							frame_size,
							chunk_limit,
							extra_chunk_limit,
							chunk_size);

						size_t first_part_size = GetFirstPartSize(
							frame_size,
							chunk_limit,
							extra_chunk_limit,
							chunk_size);

						auto sizing = first_part_size * channels;
						int result =
							chromaprint_feed(context, frame_data, sizing);

						if (result == 0)
						{
							logger.error("Could not process audio data");
							break;
						}

						chunk_size += first_part_size;

						if (chunk_done)
						{
							if (!chromaprint_finish(context))
							{
								std::string message =
									"Could not finish the fingerprinting process";
								logger.error(message);
								break;
							}

							const auto chunk_duration = (chunk_size - extra_chunk_limit) * 1.0 / reader.GetSampleRate() + overlap;
							char* finger = GetFingerPrint(
								context,
								reader,
								first_chunk,
								ts,
								chunk_duration,
								logger);

							ts += chunk_duration;

							result = chromaprint_start(
								context, sampleRate, channels);

							if (result == 0)
							{
								logger.error("Could not initialize the fingerprinting process");
								break;
							}

							if (first_chunk)
							{
								extra_chunk_limit = 0;
								first_chunk = false;
							}

							chunk_size = 0;
						}

						frame_data += first_part_size * reader.GetChannels();
						frame_size -= first_part_size;

						if (frame_size > 0)
						{
							if (!chromaprint_feed(context, frame_data, frame_size * reader.GetChannels()))
							{
								logger.error("Could not process audio data");
								break;
							}
						}

						chunk_size += frame_size;

						if (streamDone)
						{
							break;
						}
					}

					if (!chromaprint_finish(context))
					{
						logger.error("Could not finish the fingerprinting process");
					}
					else if (chunk_size > 0)
					{
						const auto chunk_duration =
							(chunk_size - extra_chunk_limit) * 1.0 /
							sampleRate + overlap;

						result = GetFingerPrint(
							context,
							reader,
							first_chunk,
							ts,
							chunk_duration,
							logger);
					}
					else if (first_chunk)
					{
						logger.error("Not enough audio data");
					}
				}
			}

			reader.Close();
			chromaprint_free(context);
		}
		else
		{
			std::string error = "File Doesn't Exist";
			logger.error(error);
		}

		return result;
	}

	char* GetFingerPrint(
		ChromaprintContext* context,
		FFmpegAudioReader& reader,
		bool first,
		double timestamp,
		double duration,
		spdlog::logger log)
	{
		char* fingerPrint = nullptr;
		int size;

		int result = chromaprint_get_raw_fingerprint_size(context, &size);

		if (result == 0)
		{
			log.error("Could not get the fingerprinting size");
		}
		else
		{
			if (size <= 0 && first == true)
			{
				log.error("Empty fingerprint");
			}
			else
			{
				result = chromaprint_get_fingerprint(context, &fingerPrint);

				if (result == 0)
				{
					log.error("Could not get the fingerprinting");
				}
			}
		}

		return fingerPrint;
	}

	size_t GetFirstPartSize(
		size_t frameSize,
		size_t chunkLimit,
		size_t extraChunkLimit,
		size_t chunkSize)
	{
		bool chunkDone = false;

		size_t firstPartSize = frameSize;

		if (chunkLimit > 0)
		{
			size_t remaining = chunkLimit + extraChunkLimit - chunkSize;

			if (frameSize > remaining)
			{
				firstPartSize = remaining;
			}
		}

		return firstPartSize;
	}

	size_t GetFrameSize(
		size_t streamLimit, size_t streamSize, size_t frameSize)
	{
		if (streamLimit > 0)
		{
			const size_t remaining = streamLimit - streamSize;

			if (frameSize > remaining)
			{
				frameSize = remaining;
			}
		}

		return frameSize;
	}

	spdlog::logger GetLogger()
	{
		std::vector<spdlog::sink_ptr> sinks;

		std::shared_ptr<spdlog::sinks::stdout_sink_st> consoleLog =
			std::make_shared<spdlog::sinks::stdout_sink_st>();
		sinks.push_back(consoleLog);

		std::shared_ptr<spdlog::sinks::basic_file_sink_st> fileLog =
			std::make_shared<spdlog::sinks::basic_file_sink_st>("MusicMan.log");
		sinks.push_back(fileLog);

		spdlog::logger logger =
			spdlog::logger("log", begin(sinks), end(sinks));

		return logger;
	}

	bool IsChunkDone(
		size_t frameSize,
		size_t chunkLimit,
		size_t extraChunkLimit,
		size_t chunkSize)
	{
		bool chunkDone = false;

		if (chunkLimit > 0)
		{
			size_t remaining = chunkLimit + extraChunkLimit - chunkSize;

			if (frameSize > remaining)
			{
				chunkDone = true;
			}
		}

		return chunkDone;
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
