using System;
using Gpt4All.Native;

namespace Gpt4All
{
    /// <summary>
    /// Current context and inference settings of LLM.
    /// </summary>
    /// <remarks>
    /// Replacing this in running model doesn't change it's internal context, only settings.
    /// If you want full context reset - restart model.
    /// </remarks>
    public class LlmPromptContext
    {
        /// <summary>
        /// Native C++ structure representing parameters.
        /// Don't change it directly, use getters and setters.
        /// </summary>
        public llmodel_prompt_context Native;

        public LlmPromptContext(llmodel_prompt_context native)
        {
            Native = native;
        }

        /// <summary>
        /// Number of tokens possible in context window (n_ctx).
        /// </summary>
        public int ContextWindow
        {
            get => Native.n_ctx;
            set => Native.n_ctx = value;
        }
        
        /// <summary>
        /// Max number of tokens to predict for next prompt (n_predict).
        /// </summary>
        public int MaxTokensPredict
        {
            get => Native.n_predict;
            set => Native.n_predict = value;
        }

        /// <summary>
        /// Top k logits to sample from.
        /// </summary>
        public int TopK
        {
            get => Native.top_k;
            set => Native.top_k = value;
        }
        
        /// <summary>
        /// Nucleus sampling probability threshold.
        /// </summary>
        public float TopP
        {
            get => Native.top_p;
            set => Native.top_p = value;
        }

        /// <summary>
        /// Temperature to adjust model's output distribution.
        /// </summary>
        public float Temperature
        {
            get => Native.temp;
            set => Native.temp = value;
        }

        /// <summary>
        /// Number of predictions to generate in parallel (n_batch).
        /// </summary>
        public int Batch
        {
            get => Native.n_batch;
            set => Native.n_batch = value;
        }

        /// <summary>
        /// Penalty factor for repeated tokens.
        /// </summary>
        public float RepeatPenalty
        {
            get => Native.repeat_penalty;
            set => Native.repeat_penalty = value;
        }

        /// <summary>
        /// Last n tokens to penalize.
        /// </summary>
        public int RepeatLastN
        {
            get => Native.repeat_last_n;
            set => Native.repeat_last_n = value;
        }

        /// <summary>
        /// Percent of context to erase if we exceed the context window.
        /// </summary>
        public float ContextErase
        {
            get => Native.context_erase;
            set => Native.context_erase = value;
        }
        
        /// <summary>
        /// Generate new context with default parameters.
        /// </summary>
        public static LlmPromptContext GetDefaultContext()
        {
            var nativeCtx = new llmodel_prompt_context
            {
                logits = null,
                logits_size = (UIntPtr) 0,
                tokens = null,
                tokens_size = (UIntPtr) 0,
                n_past = 0,
                n_ctx = 4096,
                n_predict = 64,
                top_k = 40,
                top_p = 0.95f,
                temp = 0.28f,
                n_batch = 1,
                repeat_penalty = 1.1f,
                repeat_last_n = 10,
                context_erase = 0.5f
            };

            var ctx = new LlmPromptContext(nativeCtx);
            return ctx;
        }
    }
}
