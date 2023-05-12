set MY_PATH=%CD%

cd %1\llmodel
rmdir .\build /s /q
cmake -B ./build -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=Release .

cd ./build
cmake --build . --parallel
xcopy /y /q .\bin\Release\whisper.dll %MY_PATH%\Packages\com.whisper.unity\Plugins\Windows\libwhisper.dll*