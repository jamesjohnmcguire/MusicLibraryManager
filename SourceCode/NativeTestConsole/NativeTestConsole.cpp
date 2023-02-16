#define _CRT_SECURE_NO_WARNINGS

#include <filesystem>
#include <iostream>

#include "../FingerPrinter/FingerPrinter.h"
using namespace FingerPrinter;

int main()
{
	std::cout << "Hello World!\n";

	char* appdata = std::getenv("APPDATA");

	if (appdata)
	{
		std::filesystem::path path = appdata;
		path /= "DigitalZenWorks\\MusicManager\\sakura.mp4";

		std::string tempPath = path.string();
		const char* dataPath = tempPath.c_str();

		char* result = FingerPrint(dataPath);
	}
}
