#include "pch.h"

#include "../FingerPrinter/FingerPrinter.h"

using namespace FingerPrinter;

TEST(TestCaseName, TestName)
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

	std::string intended = "something";
	EXPECT_EQ(result, intended);
}
