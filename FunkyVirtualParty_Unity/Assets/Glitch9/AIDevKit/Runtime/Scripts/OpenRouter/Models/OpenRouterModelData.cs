using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenRouter
{
    public class OpenRouterModelData : IModelData
    {
        /// <summary>
        /// Required. The ID of the model.
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// Required. The name of the model.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Required. The creation time of the model.
        /// </summary>
        [JsonProperty("created")] public UnixTime? CreatedAt { get; set; }

        /// <summary>
        /// Required. The description of the model.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// Required. The architecture details of the model.
        /// </summary>
        [JsonProperty("architecture")] public ModelArchitecture Architecture { get; set; }

        /// <summary>
        /// Required. The top provider details.
        /// </summary>
        [JsonProperty("top_provider")] public TopProvider TopProvider { get; set; }

        /// <summary>
        /// Required. The pricing details of the model.
        /// </summary>
        [JsonProperty("pricing")] public ModelPricing Pricing { get; set; }

        /// <summary>
        /// Required. The context length of the model.
        /// </summary>
        [JsonProperty("context_length")] public int ContextLength { get; set; }

        /// <summary>
        /// Optional. IDK what this is for, but it seems to be a dictionary of string to object.
        /// Official doc says nothing about it. (https://openrouter.ai/docs/api-reference/list-available-models)
        /// </summary>
        [JsonProperty("per_request_limits")] public Dictionary<string, object> PerRequestLimits { get; set; }


        [JsonIgnore] public Api Api => Api.OpenRouter;
        [JsonIgnore] public string OwnedBy => "OpenRouter";
        [JsonIgnore] public int? InputTokenLimit => TopProvider?.ContextLength != null ? (int?)TopProvider.ContextLength : null;
        [JsonIgnore] public int? OutputTokenLimit => TopProvider?.MaxCompletionTokens != null ? (int?)TopProvider.MaxCompletionTokens : null;
        [JsonIgnore] public Modality? InputModality => Architecture?.InputModalities;
        [JsonIgnore] public Modality? OutputModality => Architecture?.OutputModalities;
        [JsonIgnore] public string BaseId => string.Empty;  // there is no fine-tuning in OpenRouter
        [JsonIgnore] public bool? IsFineTuned => false;     // there is no fine-tuning in OpenRouter
        [JsonIgnore] public bool? IsTrainable => false;     // there is no fine-tuning in OpenRouter
        [JsonIgnore] public string CostPerInputToken => Pricing?.Prompt;
        [JsonIgnore] public string CostPerOutputToken => Pricing?.Completion;
        [JsonIgnore] public string CostPerImage => Pricing?.Image;
        [JsonIgnore] public string CostPerRequest => Pricing?.Request;
        [JsonIgnore] public string CostPerInputCacheRead => Pricing?.InputCacheRead;
        [JsonIgnore] public string CostPerInputCacheWrite => Pricing?.InputCacheWrite;
        [JsonIgnore] public string CostPerWebSearch => Pricing?.WebSearch;
        [JsonIgnore] public string CostPerInternalReasoning => Pricing?.InternalReasoning;
    }

    public class ModelArchitecture
    {
        /// <summary>
        /// Required. The input modalities supported by the model.
        /// </summary>
        [JsonProperty("input_modalities")] public Modality InputModalities { get; set; }

        /// <summary>
        /// Required. The output modalities supported by the model.
        /// </summary>
        [JsonProperty("output_modalities")] public Modality OutputModalities { get; set; }

        /// <summary>
        /// Required. The tokenizer used by the model.
        /// </summary>
        [JsonProperty("tokenizer")] public string Tokenizer { get; set; }
    }

    public class TopProvider
    {
        /// <summary>
        /// Required. Whether the model output is moderated.
        /// </summary>
        [JsonProperty("is_moderated")] public bool IsModerated { get; set; }

        [JsonProperty("context_length")] public double? ContextLength { get; set; } // This is the input token limit written in demonic language

        [JsonProperty("max_completion_tokens")] public double? MaxCompletionTokens { get; set; }
    }

    public class ModelPricing
    {
        /// <summary>
        /// Required. Cost per prompt token.
        /// </summary>
        [JsonProperty("prompt")] public string Prompt { get; set; }

        /// <summary>
        /// Required. Cost per completion token.
        /// </summary>
        [JsonProperty("completion")] public string Completion { get; set; }

        /// <summary>
        /// Required. Cost per image.
        /// </summary>
        [JsonProperty("image")] public string Image { get; set; }

        /// <summary>
        /// Required. Cost per request.
        /// </summary>
        [JsonProperty("request")] public string Request { get; set; }

        /// <summary>
        /// Required. Cost for input cache read.
        /// </summary>
        [JsonProperty("input_cache_read")] public string InputCacheRead { get; set; }

        /// <summary>
        /// Required. Cost for input cache write.
        /// </summary>
        [JsonProperty("input_cache_write")] public string InputCacheWrite { get; set; }

        /// <summary>
        /// Required. Cost for web search.
        /// </summary>
        [JsonProperty("web_search")] public string WebSearch { get; set; }

        /// <summary>
        /// Required. Cost for internal reasoning.
        /// </summary>
        [JsonProperty("internal_reasoning")] public string InternalReasoning { get; set; }
    }
}
