cmake_minimum_required (VERSION 3.12)
project(FingerPrinter LANGUAGES CXX)

add_compile_definitions(DLL_EXPORTS)

add_library (FingerPrinter SHARED "FingerPrinter.cpp" "FingerPrinter.h")

set_property(TARGET FingerPrinter PROPERTY CXX_STANDARD 20)
set_property(TARGET FingerPrinter PROPERTY CMAKE_CXX_STANDARD_REQUIRED ON)
set_property(TARGET FingerPrinter PROPERTY CMAKE_CXX_EXTENSIONS OFF)
