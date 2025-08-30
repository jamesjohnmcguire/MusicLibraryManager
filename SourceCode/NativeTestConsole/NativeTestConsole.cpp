#define _CRT_SECURE_NO_WARNINGS

#include <filesystem>
#include <iostream>

#include "../AudioSignature/AudioSignature.h"
using namespace AudioSignature;

int main(int argc, char** argv)
{
	bool minimal = true;
	const char* dataPath = nullptr;
	std::string tempPath;

	if (minimal == false)
	{
		std::cout << "Testing\n";
	}

	if (argc > 1 && argv != nullptr)
	{
		dataPath = argv[1];
	}
	else
	{
		char* appdata = std::getenv("APPDATA");

		if (appdata)
		{
			std::filesystem::path path = appdata;
			path /= "DigitalZenWorks\\MusicManager\\sakura.mp4";

			tempPath = path.string();
			dataPath = tempPath.c_str();
		}
	}

	std::cout << dataPath << std::endl << std::endl;

	char* result = GetAudioSignature(dataPath);

	if (result != nullptr)
	{
		if (minimal == false)
		{
			std::cout << "AudioSignature: " << result <<
				std::endl << std::endl << std::endl;
		}
		else
		{
			std::cout << result << std::endl << std::endl << std::endl;
		}
	}
}
