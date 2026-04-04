cmake_minimum_required(VERSION 3.20)
project(CS2HvHUtilitiesMM LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

add_library(CS2HvHUtilitiesMM SHARED
    src/plugin.cpp
    src/plugin.hpp
)

target_include_directories(CS2HvHUtilitiesMM PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    ${METAMOD_SOURCE_INCLUDE_DIR}
    ${SOURCE2SDK_INCLUDE_DIR}
)

target_compile_definitions(CS2HvHUtilitiesMM PRIVATE
    _GLIBCXX_USE_CXX11_ABI=1
)

# NOTE:
# Link variables are expected from your MetaMod/Source2 SDK toolchain preset.
# Typical values:
#   METAMOD_LIBRARIES, SOURCEHOOK_LIBRARIES, TIER1_LIBRARIES

target_link_libraries(CS2HvHUtilitiesMM PRIVATE
    ${METAMOD_LIBRARIES}
    ${SOURCEHOOK_LIBRARIES}
    ${TIER1_LIBRARIES}
)
