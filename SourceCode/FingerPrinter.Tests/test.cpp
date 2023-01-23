#include "pch.h"

#include "../FingerPrinter/FingerPrinter.h"

using namespace FingerPrinter;

TEST(SanityCheck, Success)
{
	EXPECT_EQ(1, 1);
	EXPECT_TRUE(true);
}

TEST(TestFingerPrinter, Success)
{
	char* result = FingerPrint(nullptr);

	EXPECT_NE(result, nullptr);
}

TEST(TestFingerPrinter, Correct)
{
	char* result = FingerPrint(nullptr);

	const char* intended = "AQAAfFGiSAmTHVRyHg-FQ_yQH7WWHJ_m4NIx7nCSD2cR_Tp"
		"-8BC_F6-QJwSj8HDoFD2LHjk8KodWsiueHLNuaDoatUH08bj4BPQsKbiiY2d05IGOPD"
		"-8l3ieCueLBz7xC8mRG3ZyfFqD6xWO68iXB7qSNsgvNEcvZcdjzEye4NkX_HjzgJ-"
		"RT0G3PqCWHtexH-JNeHoVTAwrxMV3MMkT9ApyQj9y8mi6pUlQ57jw_PCTDHl-6MjyJ_"
		"CDP0W9gVEIh0T4hLgM_Ud-9LmCpsmDZ0e8Clo-hFfwnvivBHWOD2fQH_FRLg-"
		"YLMsFfcfRfEck73jy4_iNdhmadwzOhMeHM0cYOcmhHceZA0EQZdAJQZAAhCEgn"
		"MFEEEGEdAAhZRABhCCBGBvICIQAAIhRQKIBAAJJEDAGCCMFIcgAYxAxwCHDg"
		"EDCCIGEQAAQRRgAgBBMBGJCKCkEMUADQwAAgiCmlFAECVIEQsJAAgAQgiGHQRMA";

	EXPECT_EQ(result, intended);
}
