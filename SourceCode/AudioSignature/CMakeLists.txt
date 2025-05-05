cmake_minimum_required (VERSION 3.12)
project(AudioSignature LANGUAGES CXX)

add_compile_definitions(DLL_EXPORTS)

add_library (AudioSignature SHARED AudioSignature.cpp AudioSignature.h)

set_property(TARGET AudioSignature PROPERTY CXX_STANDARD 20)
set_property(TARGET AudioSignature PROPERTY CMAKE_CXX_STANDARD_REQUIRED ON)
set_property(TARGET AudioSignature PROPERTY CMAKE_CXX_EXTENSIONS OFF)
