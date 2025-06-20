using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    public class OllamaResponse
    {
        /// <summary>
        /// Required. The model name.
        /// </summary>
        [JsonProperty("model")] public string Model { get; set; }

        /// <summary>
        /// Optional. Time spent generating the response (in nanoseconds).
        /// </summary>
        [JsonProperty("total_duration")] public long? TotalDuration { get; set; }

        /// <summary>
        /// Optional. Time spent loading the model (in nanoseconds).
        /// </summary>
        [JsonProperty("load_duration")] public long? LoadDuration { get; set; }

        /// <summary>
        /// Optional. Number of tokens in the prompt.
        /// </summary>
        [JsonProperty("prompt_eval_count")] public int? InputTokenCount { get; set; }
    }

    public class OllamaTextResponse : OllamaResponse
    {
        /// <summary>
        /// Required. The timestamp when the response was created.
        /// </summary>
        [JsonProperty("created_at")] public ZuluTime CreatedAt { get; set; }

        /// <summary>
        /// Optional. The reason the response generation was stopped.
        /// </summary>
        [JsonProperty("done_reason")] public StopReason? FinishReason { get; set; }

        /// <summary>
        /// Optional. Encoding of the conversation used in this response.
        /// </summary>
        [JsonProperty("context")] public List<int> Context { get; set; }

        /// <summary>
        /// Optional. Time spent evaluating the prompt (in nanoseconds).
        /// </summary>
        [JsonProperty("prompt_eval_duration")] public long? PromptEvalDuration { get; set; }

        /// <summary>
        /// Optional. Number of tokens in the response.
        /// </summary>
        [JsonProperty("eval_count")] public int? OutputTokenCount { get; set; }

        /// <summary>
        /// Optional. Time in nanoseconds spent generating the response.
        /// </summary>
        [JsonProperty("eval_duration")] public long? EvalDuration { get; set; }

    }
}