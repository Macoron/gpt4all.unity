using System;
using System.IO;
using System.Threading.Tasks;
using Gpt4All.Utils;
using UnityEngine;

namespace Gpt4All
{
    public class LlmManager : MonoBehaviour
    {
        private const string DefaultPrompt = "### Instruction:\n" +
                                             "The prompt below is a question to answer, a task to complete, " +
                                             "or a conversation to respond to; " +
                                             "decide which and write an appropriate response.\n" +
                                             "### Prompt:\n" +
                                             "{0}\n" +
                                             "### Response:\n";

        [SerializeField] 
        [Tooltip("Model architecture type")]
        private LlmModelType modelType = LlmModelType.GPTJ;

        [SerializeField] 
        [Tooltip("Path to model weights file relative to StreamingAssets")]
        private string modelPath = "Gpt4All/ggml-gpt4all-j-v1.3-groovy.bin";

        [SerializeField] 
        [Tooltip("Should model weights be loaded on awake?")]
        private bool initOnAwake = true;

        [Header("Prompt")] 
        [TextArea(8, int.MaxValue)] 
        [Tooltip("Generic template that will be used for prompt. {0} will be replaced with users prompt")]
        public string promptTemplate = DefaultPrompt;

        [Header("Settings")] 
        [Tooltip("Max number of tokens to predict for next prompt (n_predict)")]
        public int maxTokensPredict = 256;
        
        [Range(0, 1)] 
        [Tooltip("Temperature to adjust model's output distribution. Bigger will result more random result")]
        public float temperature = 0.27f;

        [Header("Advanced settings")] 
        [Tooltip("Number of tokens possible in context window (n_ctx)")]
        public int contextWindow = 4096;
        
        [Tooltip("Top k logits to sample from")]
        public int topK = 40;
        
        [Range(0, 1)] 
        [Tooltip("Nucleus sampling probability threshold")]
        public float topP = 0.95f;
        
        [Tooltip("Number of predictions to generate in parallel (n_batch)")]
        public int batch = 1;
        
        [Tooltip("Penalty factor for repeated tokens")]
        public float repeatPenalty = 1.1f;
        
        [Tooltip("Last n tokens to penalize")]
        public int repeatLastN = 64;

        [Range(0, 1)] 
        [Tooltip("Percent of context to erase if we exceed the context window")]
        public float contextErase = 0.5f;

        public event LlmPromptDelegate OnPrompt;
        public event LlmResponseDelegate OnResponse;
        public event LlmResponseUpdatedDelegate OnResponseUpdated;
        public event LlmRecalculateDelegate OnRecalculate;

        private LlmWrapper _llm;
        private LlmPromptContext _ctx;
        private readonly MainThreadDispatcher _dispatcher = new MainThreadDispatcher();
        
        public bool IsLoaded => _llm != null;
        public bool IsLoading { get; private set; }

        private async void Awake()
        {
            if (!initOnAwake)
                return;
            await InitModel();
        }

        private void Update()
        {
            _dispatcher.Update();
        }

        public async Task InitModel()
        {
            // check if model is already loaded or actively loading
            if (IsLoaded)
            {
                Debug.LogWarning("LLM model is already loaded and ready for use!");
                return;
            }

            if (IsLoading)
            {
                Debug.LogWarning("LLM model is already loading!");
                return;
            }

            // load model and default params
            IsLoading = true;
            try
            {
                var path = Path.Combine(Application.streamingAssetsPath, modelPath);
                _llm = await LlmWrapper.InitFromPathAsync(modelType, path);
                _ctx = LlmPromptContext.GetDefaultContext();
                _llm.OnPrompt += OnPromptHandler;
                _llm.OnResponse += OnResponseHandler;
                _llm.OnResponseUpdated += OnResponseUpdatedHandler;
                _llm.OnRecalculate += OnRecalculateHandler;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            IsLoading = false;
        }

        public async Task<string> Prompt(string rawPrompt)
        {
            var isLoaded = await CheckIfLoaded();
            if (!isLoaded)
                return null;

            var prompt = string.Format(promptTemplate, rawPrompt);
            UpdateCtx();
            return await _llm.PromptAsync(prompt, _ctx);
        }

        private async Task<bool> CheckIfLoaded()
        {
            if (!IsLoaded && !IsLoading)
            {
                Debug.LogError("LLM model isn't loaded! Init LLM model first!");
                return false;
            }

            // wait while model still loading
            while (IsLoading)
            {
                await Task.Yield();
            }

            return IsLoaded;
        }

        private void UpdateCtx()
        {
            _ctx.ContextWindow = contextWindow;
            _ctx.MaxTokensPredict = maxTokensPredict;
            _ctx.TopK = topK;
            _ctx.TopP = topP;
            _ctx.Temperature = temperature;
            _ctx.Batch = batch;
            _ctx.RepeatPenalty = repeatPenalty;
            _ctx.RepeatLastN = repeatLastN;
            _ctx.ContextErase = contextErase;
        }

        private void OnPromptHandler(int tokenId)
        {
            _dispatcher.Execute(() => OnPrompt?.Invoke(tokenId));
        }

        private void OnResponseHandler(int tokenId, byte[] response)
        {
            _dispatcher.Execute(() => OnResponse?.Invoke(tokenId, response));
        }
        
        private void OnResponseUpdatedHandler(string response)
        {
            _dispatcher.Execute(() => OnResponseUpdated?.Invoke(response));
        }

        private void OnRecalculateHandler(bool isRecalculating)
        {
            _dispatcher.Execute(() => OnRecalculate?.Invoke(isRecalculating));
        }
    }
}
