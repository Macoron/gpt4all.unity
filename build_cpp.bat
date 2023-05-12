set MY_PATH=%CD%

cd %1\llmodel
rmdir .\build /s /q
cmake -B ./build -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=Release .

cd ./build
cmake --build . --parallel
xcopy /y /q .\libllmodel.dll %MY_PATH%\Packages\com.gpt4all.unity\Plugins\Windows\libllmodel.dll*
xcopy /y /q .\bin\libllama.dll %MY_PATH%\Packages\com.gpt4all.unity\Plugins\Windows\libllama.dll*