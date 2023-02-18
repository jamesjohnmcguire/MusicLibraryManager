#define _CRT_SECURE_NO_WARNINGS

#include <filesystem>
#include <iostream>

#include "../FingerPrinter/FingerPrinter.h"
using namespace FingerPrinter;

int main(int argc, char** argv)
{
	const char* dataPath = nullptr;
	std::cout << "Testing\n";

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

			std::string tempPath = path.string();
			dataPath = tempPath.c_str();
		}
	}

	char* result = FingerPrint(dataPath);

	std::cout << "Fingerprint: " << result << "\n";
}
