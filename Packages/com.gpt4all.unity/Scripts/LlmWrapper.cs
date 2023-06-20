using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AOT;
using Gpt4All.Native;
using Gpt4All.Utils;
using Debug = UnityEngine.Debug;

namespace Gpt4All
{
    public delegate void LlmPromptDelegate(int tokenId);

    public delegate void LlmResponseDelegate(int tokenId, byte[] response);
    
    public delegate void LlmResponseUpdatedDelegate(string response);

    public delegate void LlmRecalculateDelegate(bool isRecalculating);

    /// <summary>
    /// Architecture type of the LLM.
    /// </summary>
    public enum LlmModelType
    {
        GPTJ,
        LLAMA,
        MPT
    }
    
    public class LlmWrapper
    {
        private static LlmWrapper _instance;
        private IntPtr _model;
        private readonly LlmModelType _type;
        private readonly List<byte> _buffer = new List<byte> ();
        private readonly object _lock = new object();

        public event LlmPromptDelegate OnPrompt;
        public event LlmResponseDelegate OnResponse;
        public event LlmResponseUpdatedDelegate OnResponseUpdated;
        public event LlmRecalculateDelegate OnRecalculate;

        private LlmWrapper(LlmModelType type, IntPtr model)
        {
            if (_instance != null)
            {
                throw new SystemException("Only one instance of Llm is supported!");
            }
            
            _type = type;
            _model = model;
            _instance = this;
        }

        ~LlmWrapper()
        {
            _instance = null;
            if (_model == IntPtr.Zero)
                return;

            switch (_type)
            {
                case LlmModelType.GPTJ:
                    LlmNative.llmodel_gptj_destroy(_model);
                    break;
                case LlmModelType.LLAMA:
                    LlmNative.llmodel_llama_destroy(_model);
                    break;
                case LlmModelType.MPT:
                    LlmNative.llmodel_mpt_destroy(_model);
                    break;
            }
            _model = IntPtr.Zero;
        }

        public string Prompt(string promptText, LlmPromptContext context)
        {
            lock (_lock)
            {
                _buffer.Clear();
                Debug.Log("Inference LLM on input data...");
                var sw = new Stopwatch();
                sw.Start();

                var ctx = context.Native;
                unsafe
                {
                    LlmNative.llmodel_prompt(_model, promptText,
                        PromptCallbackStatic, ResponseCallbackStatic, RecalculateCallbackStatic,
                        &ctx);
                }

                context.Native = ctx;
                Debug.Log($"Token size: {(int)ctx.tokens_size}");
                Debug.Log($"LLM inference finished, total time: {sw.ElapsedMilliseconds} ms.");
                
                var fullResponse = Encoding.UTF8.GetString(_buffer.ToArray());
                return fullResponse;
            }
        }

        public async Task<string> PromptAsync(string promptText, LlmPromptContext context)
        {
            var asyncTask = Task.Factory.StartNew(() => Prompt(promptText, context));
            var res = await asyncTask;
            return res;
        }

        [MonoPInvokeCallback(typeof(llmodel_prompt_callback))]
        private static bool PromptCallbackStatic(int tokenId)
        {
            return _instance.PromptCallback(tokenId);
        }

        private bool PromptCallback(int tokenId)
        {
            OnPrompt?.Invoke(tokenId);
            return true;
        }

        [MonoPInvokeCallback(typeof(llmodel_response_callback))]
        private static bool ResponseCallbackStatic(int tokenId, IntPtr response)
        {
            return _instance.ResponseCallback(tokenId, response);
        }

        private bool ResponseCallback(int tokenId, IntPtr response)
        {
            // invoke raw byte buffer
            var tokenBuffer = TextUtils.BytesFromNativeUtf8(response);
            OnResponse?.Invoke(tokenId, tokenBuffer);
            
            // append it to full response
            _buffer.AddRange(tokenBuffer);
            var fullResponse = Encoding.UTF8.GetString(_buffer.ToArray());
            OnResponseUpdated?.Invoke(fullResponse);
            
            return true;
        }

        [MonoPInvokeCallback(typeof(llmodel_recalculate_callback))]
        private static bool RecalculateCallbackStatic(bool isRecalculating)
        {
            return _instance.RecalculateCallback(isRecalculating);
        }

        private bool RecalculateCallback(bool isRecalculating)
        {
            OnRecalculate?.Invoke(isRecalculating);
            return true;
        }

        public static LlmWrapper InitFromPath(LlmModelType type, string modelPath)
        {
            // some sanity checks
            if (string.IsNullOrEmpty(modelPath))
            {
                Debug.LogError("LLM model path is null or empty!");
                return null;
            }

            if (!File.Exists(modelPath))
            {
                Debug.LogError($"LLM model path {modelPath} doesn't exist!");
                return null;
            }

            IntPtr model;
            switch (type)
            {
                case LlmModelType.GPTJ:
                    Debug.Log("Trying to init GPT-J model...");
                    model = LlmNative.llmodel_gptj_create();
                    Debug.Log("GPT-J model created!");
                    break;
                case LlmModelType.MPT:
                    Debug.Log("Trying to init MPT model...");
                    model = LlmNative.llmodel_mpt_create();
                    Debug.Log("MPT model created!");
                    break;
                case LlmModelType.LLAMA:
                    Debug.Log("Trying to init Llama model...");
                    model = LlmNative.llmodel_llama_create();
                    Debug.Log("Llama model created!");
                    break;
                default:
                    throw new Exception("Unknown model architecture!");
            }

            var wrapper = new LlmWrapper(type, model);

            Debug.Log($"Trying to load LLM model from {modelPath}...");

            // actually loading model
            var sw = new Stopwatch();
            sw.Start();

            if (!LlmNative.llmodel_loadModel(model, modelPath))
            {
                Debug.LogError("Failed to load LLM model!");
                return null;
            }

            Debug.Log($"LLM model is loaded, total time: {sw.ElapsedMilliseconds} ms.");
            return wrapper;
        }

        public static async Task<LlmWrapper> InitFromPathAsync(LlmModelType type, string modelPath)
        {
            var asyncTask = Task.Factory.StartNew(() => InitFromPath(type, modelPath));
            return await asyncTask;
        }
    }
}