using System;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global

using llmodel_model = System.IntPtr;

namespace Gpt4All.Native
{
    /// <summary>
    /// This is direct copy of C++ struct.
    /// Do not change or add any fields without changing it in C++.
    /// Check <see cref="LlmPromptContext"/> for more information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct llmodel_prompt_context
    {
        public float *logits;          // logits of current context
        public UIntPtr logits_size;     // the size of the raw logits vector
        public int *tokens;        // current tokens in the context window
        public UIntPtr tokens_size;     // the size of the raw tokens vector
        public int n_past;         // number of tokens in past conversation
        public int n_ctx;          // number of tokens possible in context window
        public int n_predict;      // number of tokens to predict
        public int top_k;          // top k logits to sample from
        public float top_p;            // nucleus sampling probability threshold
        public float temp;             // temperature to adjust model's output distribution
        public int n_batch;        // number of predictions to generate in parallel
        public float repeat_penalty;   // penalty factor for repeated tokens
        public int repeat_last_n;  // last n tokens to penalize
        public float context_erase;    // percent of context to erase if we exceed the context window
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool llmodel_prompt_callback(int token_id);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool llmodel_response_callback(int token_id, IntPtr response);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate bool llmodel_recalculate_callback(bool is_recalculating);
    
    /// <summary>
    /// Bindings to native gpt4all C API.
    /// </summary>
    public static class LlmNative
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        private const string LibraryName = "__Internal";
#else
        private const string LibraryName = "llmodel";
#endif
    
        [DllImport(LibraryName)]
        public static extern llmodel_model llmodel_gptj_create();

        [DllImport(LibraryName)]
        public static extern void llmodel_gptj_destroy(llmodel_model gptj);
        
        [DllImport(LibraryName)]
        public static extern llmodel_model llmodel_mpt_create();

        [DllImport(LibraryName)]
        public static extern void llmodel_mpt_destroy(llmodel_model mpt);
    
        [DllImport(LibraryName)]
        public static extern llmodel_model llmodel_llama_create();
    
        [DllImport(LibraryName)]
        public static extern void llmodel_llama_destroy(llmodel_model llama);
    
        [DllImport(LibraryName)]
        public static extern bool llmodel_loadModel(llmodel_model model, string model_path);
    
        [DllImport(LibraryName)]
        public static extern unsafe void llmodel_prompt(llmodel_model model, string prompt,
            llmodel_prompt_callback prompt_callback,
            llmodel_response_callback response_callback,
            llmodel_recalculate_callback recalculate_callback,
            llmodel_prompt_context *ctx);
    }
}



