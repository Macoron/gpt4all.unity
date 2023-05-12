#!/bin/bash

gpt4all_path="$1"
targets=${2:-all}
unity_project="$PWD"
build_path="$gpt4all_path/llmodel/build"

clean_build(){
  rm -rf "$build_path"
  mkdir "$build_path"
  cd "$build_path"
}

build_mac() {
  clean_build
  echo "Starting building for Mac..."

  cmake -DBUILD_UNIVERSAL=ON ../
  make

  echo "Build for Mac complete!"

  artifact_path="$build_path/libllmodel.dylib"
  target_path="$unity_project/Packages/com.gpt4all.unity/Plugins/MacOS/libllmodel.dylib"
  cp "$artifact_path" "$target_path"

  echo "Build files copied to $target_path"
}

if [ "$targets" = "all" ]; then
  build_mac
elif [ "$targets" = "mac" ]; then
  build_mac
else
  echo "Unknown targets: $targets"
fi