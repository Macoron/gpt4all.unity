#!/bin/bash

gpt4all_path="$1"
targets=${2:-all}
unity_project="$PWD"
build_path="$gpt4all_path/gpt4all-backend/build"

clean_build(){
  rm -rf "$build_path"
  mkdir "$build_path"
  cd "$build_path"
}

build_mac() {
  clean_build
  echo "Starting building for Mac..."

  cmake -DBUILD_UNIVERSAL=ON -DCMAKE_BUILD_TYPE=Release ../
  make

  echo "Build for Mac complete!"

  cp "$build_path/libllmodel.dylib" "$unity_project/Packages/com.gpt4all.unity/Plugins/MacOS/llmodel.dylib"
  cp "$build_path/llama.cpp/libllama.dylib" "$unity_project/Packages/com.gpt4all.unity/Plugins/MacOS/libllama.dylib"

  echo "Build files copied to $target_path"
}

if [ "$targets" = "all" ]; then
  build_mac
elif [ "$targets" = "mac" ]; then
  build_mac
else
  echo "Unknown targets: $targets"
fi