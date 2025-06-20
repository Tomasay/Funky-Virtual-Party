using System;
using Glitch9.Collections;
using Glitch9.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Usage statistics for the completion request. 
    /// </summary>
    [Serializable]
    public class Usage
    {
        [JsonProperty] public string modelId; // Model ID used for the request 
        [JsonProperty] public ReferencedDictionary<UsageType, int> usages = new();

        /// <summary>
        /// Number of tokens used in the prompt (input sent by the user).
        /// </summary>
        public int? InputTokens { get => GetUsage(UsageType.InputToken); set => SetUsage(UsageType.InputToken, value); }

        /// <summary>
        /// Number of tokens generated in the response (output by the model).
        /// </summary>
        public int? OutputTokens { get => GetUsage(UsageType.OutputToken); set => SetUsage(UsageType.OutputToken, value); }

        /// <summary>
        /// Number of tokens used for fine-tuning or training the model.
        /// </summary>
        public int? TrainingTokens { get => GetUsage(UsageType.TrainingToken); set => SetUsage(UsageType.TrainingToken, value); }

        /// <summary>
        /// Number of tokens reused from cache instead of re-sending in the input (for efficiency).
        /// </summary>
        public int? CachedInputTokens { get => GetUsage(UsageType.CachedInputToken); set => SetUsage(UsageType.CachedInputToken, value); }

        /// <summary>
        /// Number of tokens derived from audio input (e.g., in transcriptions).
        /// </summary>
        public int? AudioInputTokens { get => GetUsage(UsageType.AudioInputToken); set => SetUsage(UsageType.AudioInputToken, value); }

        /// <summary>
        /// Number of tokens used for audio output (e.g., in TTS responses).
        /// </summary>
        public int? AudioOutputTokens { get => GetUsage(UsageType.AudioOutputToken); set => SetUsage(UsageType.AudioOutputToken, value); }

        /// <summary>
        /// Number of tokens that matched model predictions and were accepted (prediction mode).
        /// </summary>
        public int? AcceptedPredictionTokens { get => GetUsage(UsageType.AcceptedPredictionToken); set => SetUsage(UsageType.AcceptedPredictionToken, value); }

        /// <summary>
        /// Number of tokens that were predicted but rejected or unused.
        /// </summary>
        public int? RejectedPredictionTokens { get => GetUsage(UsageType.RejectedPredictionToken); set => SetUsage(UsageType.RejectedPredictionToken, value); }

        /// <summary>
        /// Number of tokens spent on internal reasoning (e.g., CoT steps).
        /// </summary>
        public int? ReasoningTokens { get => GetUsage(UsageType.ReasoningToken); set => SetUsage(UsageType.ReasoningToken, value); }

        /// <summary>
        /// Returns true if no usage data is present.
        /// </summary>
        public bool IsEmpty => usages.IsNullOrEmpty();

        /// <summary>
        /// Returns true if the response was free of charge (marked with UsageType.Free).
        /// </summary>
        public bool IsFree => usages.ContainsKey(UsageType.Free);

        public Usage() { }

        private string _cachedInspectorText; // Cached inspector text for performance
        private string[] _cachedInspectorTextParts; // Cached parts for inspector text

        public override string ToString()
        {
            if (_cachedInspectorText != null) return _cachedInspectorText;
            _cachedInspectorText = string.Join(", ", ToInspectorTextParts());
            return _cachedInspectorText;
        }

        internal string[] ToInspectorTextParts() => _cachedInspectorTextParts
            ??= UsageUtil.CreateInspectorTextParts(usages, _cachedInspectorTextParts);

        internal void ResetInspectorTexts()
        {
            _cachedInspectorText = null; // Invalidate cached text
            _cachedInspectorTextParts = null; // Invalidate cached parts
        }

        private int? GetUsage(UsageType type)
            => usages.TryGetValue(type, out var value) ? value : null;

        private void SetUsage(UsageType type, int? value)
        {
            if (value == null) return;
            usages.AddOrUpdate(type, value.Value); //[type] = value.Value;
        }
    }

    internal class UsageConverter : JsonConverter<Usage>
    {
        private readonly Api _api;
        internal UsageConverter(Api api) => _api = api;

        public override Usage ReadJson(JsonReader reader, Type objectType, Usage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);

            if (_api == Api.Google)
            {
                return new Usage
                {
                    InputTokens = (int?)json["promptTokenCount"],
                    OutputTokens = (int?)json["candidatesTokenCount"],
                    //TotalTokens = (int?)json["totalTokenCount"]
                };
            }

            // all other APIs other than Google (e.g. OpenAI, Ollama, OpenRouter, etc.) 

            Usage result = new()
            {
                InputTokens = (int?)json["prompt_tokens"],
                OutputTokens = (int?)json["completion_tokens"],
                //TotalTokens = (int?)json["total_tokens"]
            };

            // check if the response has 'PromptTokensDetails'
            if (json.TryGetValue("prompt_tokens_details", StringComparison.OrdinalIgnoreCase, out JToken promptDetails))
            {
                try
                {
                    var details = promptDetails.ToObject<PromptTokensDetails>(serializer);

                    if (details != null)
                    {
                        result.AudioInputTokens = details.AudioTokens;
                        result.CachedInputTokens = details.CachedTokens;
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"Failed to deserialize prompt tokens details: {ex.Message}");
                }
            }

            // check if the response has 'CompletionTokensDetails'
            if (json.TryGetValue("completion_tokens_details", StringComparison.OrdinalIgnoreCase, out JToken completionDetails))
            {
                try
                {
                    var details = completionDetails.ToObject<CompletionTokensDetails>(serializer);

                    if (details != null)
                    {
                        result.AudioOutputTokens = details.AudioTokens;
                        result.ReasoningTokens = details.ReasoningTokens;
                        result.AcceptedPredictionTokens = details.AcceptedPredictionTokens;
                        result.RejectedPredictionTokens = details.RejectedPredictionTokens;
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"Failed to deserialize completion tokens details: {ex.Message}");
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, Usage value, JsonSerializer serializer)
        {
            throw new NotImplementedException("WriteJson is not implemented(no need) for UsageConverter.");
        }
    }

    public enum UsageType
    {
        [ExInspectorName("Free", "-")] Free,

        [ExInspectorName("Input Token", "In")] InputToken,
        [ExInspectorName("Output Token", "Out")] OutputToken,
        [ExInspectorName("Training Token", "Train")] TrainingToken,
        [ExInspectorName("Input Token (Cached)", "In(Cache)")] CachedInputToken,

        [ExInspectorName("Image (General)", "Img")] Image,
        [ExInspectorName("Image SD 256×256", "256SD")] ImageSD256,
        [ExInspectorName("Image SD 512×512", "512SD")] ImageSD512,
        [ExInspectorName("Image SD 1024×1024", "1024SD")] ImageSD1024,
        [ExInspectorName("Image SD 1024×1792, 1792x1024", "1792SD")] ImageSD1792,
        [ExInspectorName("Image HD 1024×1024", "1024HD")] ImageHD1024,
        [ExInspectorName("Image HD 1792×1024, 1792x1024", "1792HD")] ImageHD1792,
        [ExInspectorName("Image Low 1024×1024", "1024L")] ImageLow1024,
        [ExInspectorName("Image Low 1024×1536, 1536×1024", "1536L")] ImageLow1536,
        [ExInspectorName("Image Medium 1024×1024", "1024M")] ImageMedium1024,
        [ExInspectorName("Image Medium 1024×1536, 1536×1024", "1536M")] ImageMedium1536,
        [ExInspectorName("Image High 1024×1024", "1024H")] ImageHigh1024,
        [ExInspectorName("Image High 1024×1536, 1536×1024", "1536H")] ImageHigh1536,

        [ExInspectorName("Per Minute", "STT")] PerMinute,
        [ExInspectorName("Per Character", "TTS")] PerCharacter,
        [ExInspectorName("Per Request", "Req")] PerRequest,

        [ExInspectorName("Input Cache Read", "Read(Cache)")] InputCacheRead,
        [ExInspectorName("Input Cache Write", "Write(Cache)")] InputCacheWrite,

        [ExInspectorName("Per Web Search (Low)", "Web↓")] WebSearchLow,
        [ExInspectorName("Per Web Search (Medium)", "Web")] WebSearch,
        [ExInspectorName("Per Web Search (High)", "Web↑")] WebSearchHigh,
        [ExInspectorName("Internal Reasoning", "Logic")] InternalReasoning,

        [ExInspectorName("Reasoning Token (Thinking)", "Think")] ReasoningToken,

        [ExInspectorName("Input Token (Audio)", "In(A)")] AudioInputToken,
        [ExInspectorName("Output Token (Audio)", "Out(A)")] AudioOutputToken,
        [ExInspectorName("Prediction Token (Accepted)", "Pred✓")] AcceptedPredictionToken,
        [ExInspectorName("Prediction Token (Rejected)", "Pred✗")] RejectedPredictionToken,

        [Obsolete] OutputTokenThinking,
    }


    internal static class UsageTypeExtensions
    {
        internal static bool IsTokenType(this UsageType type)
        {
            return type == UsageType.InputToken
            || type == UsageType.OutputToken
            || type == UsageType.TrainingToken
            || type == UsageType.CachedInputToken;
        }
    }


    /// <summary>
    /// Input token details for the prompt.
    /// This includes audio input tokens and cached tokens.
    /// </summary>
    public class PromptTokensDetails
    {
        /// <summary>
        /// Audio input tokens present in the prompt.
        /// </summary>
        [JsonProperty("audio_tokens")] public int AudioTokens { get; set; }

        /// <summary>
        /// Cached tokens present in the prompt.
        /// </summary>
        [JsonProperty("cached_tokens")] public int CachedTokens { get; set; }
    }

    /// <summary>
    /// Output token details for the completion.
    /// This includes tokens generated by the model, reasoning tokens, and tokens from predicted outputs.
    /// </summary>
    public class CompletionTokensDetails
    {
        /// <summary>
        /// When using Predicted Outputs, the number of tokens in the prediction that appeared in the completion.
        /// </summary>
        [JsonProperty("accepted_prediction_tokens")] public int AcceptedPredictionTokens { get; set; }

        /// <summary>
        /// Audio input tokens generated by the model.
        /// </summary>
        [JsonProperty("audio_tokens")] public int AudioTokens { get; set; }

        /// <summary>
        /// Tokens generated by the model for reasoning.
        /// </summary>
        [JsonProperty("reasoning_tokens")] public int ReasoningTokens { get; set; }

        /// <summary>
        /// When using Predicted Outputs, the number of tokens in the prediction that did not appear in the completion.
        /// However, like reasoning tokens, these tokens are still counted in the total completion tokens for purposes
        /// of billing, output, and context window limits.
        /// </summary>
        [JsonProperty("rejected_prediction_tokens")] public int RejectedPredictionTokens { get; set; }
    }
}