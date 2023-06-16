set MY_PATH=%CD%

cd %1\gpt4all-backend
rmdir .\build /s /q
cmake -S . -B ./build -A x64 -DCMAKE_BUILD_TYPE=Release -DCMAKE_CXX_FLAGS="/std:c++20"

cd ./build
msbuild ALL_BUILD.vcxproj -t:build -p:configuration=Release -p:platform=x64
xcopy /y /q .\Release\llmodel.dll %MY_PATH%\Packages\com.gpt4all.unity\Plugins\Windows\llmodel.dll*
xcopy /y /q .\bin\Release\llama.dll %MY_PATH%\Packages\com.gpt4all.unity\Plugins\Windows\llama.dll*