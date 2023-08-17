# gpt4all.unity
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT) 

This is Unity3d bindings for the [gpt4all](https://github.com/nomic-ai/gpt4all). It provides high-performance inference of large language models (LLM) running on your local machine.

> This bindings use outdated version of gpt4all. They don't support latest models architectures and quantization. Here is a [list of models](#downloading-model-weights) that I have tested. For more information [check this issue](https://github.com/Macoron/gpt4all.unity/issues/11).

**Main features:**
- Chat-based LLM that can be used for NPCs and virtual assistants
- Models of different sizes for commercial and non-commercial use
- Fast CPU based inference
- Runs on local users device without Internet connection
- Free and open source

**Supported platforms:**
- [x] Windows (x86_64)
- [x] MacOS (Intel and ARM)
- [ ] Linux
- [ ] iOS
- [ ] Android
- [ ] WebGL

## Samples

https://github.com/Macoron/gpt4all.unity/assets/6161335/1c0540d0-a169-4d88-8661-35ec0681ff5b

*"mpt-7b-chat" model, roleplaying dwarf NPC on Macbook with M1 Pro*

## Getting started
Clone this repository and open it as regular Unity project. It should be working starting from Unity 2019.4 LTS.

Alternatively you can add this repository to your existing project as a **Unity Package**. Add it by this git URL to your Unity Package Manager:
```
https://github.com/Macoron/gpt4all.unity.git?path=/Packages/com.gpt4all.unity
```
### Downloading model weights

You would need to download model weights in order to use this library. You can find full list of officially supported gpt4all models and their download links [here](https://github.com/nomic-ai/gpt4all/tree/main/gpt4all-chat#manual-download-of-models).

> Some models can't be used for commercial projects or has other restrictions. Check their licenses before using them in your project.

After downloading model, place it `StreamingAssets/Gpt4All` folder and update path in `LlmManager` component.

Here is models that I've tested in Unity:
- [mpt-7b-chat](https://huggingface.co/macoron/ggml-mpt-7b-chat) [license: cc-by-nc-sa-4.0]
- [mpt-7b-instruct](https://huggingface.co/macoron/ggml-mpt-7b-instruct) [license: cc-by-sa-3.0]
- [mpt-7b-base](https://huggingface.co/macoron/ggml-mpt-7b-base) [license: apache-2.0]
- [gpt4all-j-v1.3-groovy](https://huggingface.co/macoron/ggml-gpt4all-j-v1.3-groovy) [license: apache-2.0]
- gpt4all-l13b-snoozy

## Compiling C++ libraries from source
TBD


## License
This project is licensed under the MIT License. 

It uses compiled libraries of [gpt4all](https://github.com/nomic-ai/gpt4all/tree/main) and [llama.cpp](https://github.com/ggerganov/llama.cpp) which are also under MIT license.

Models aren't include in this repository. Please contact original models creators to learn more about their licenses.
