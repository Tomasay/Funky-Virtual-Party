using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>  
    /// OpenAI-style reasoning effort setting
    /// o-series models only
    /// </summary>
    public enum ReasoningEffort
    {
        [ApiEnum("Medium", "medium")] Medium,
        [ApiEnum("Low", "low")] Low,
        [ApiEnum("High", "high")] High
    }

    public class ReasoningOptions
    {
        /// <summary>
        /// Optional. OpenAI-style reasoning effort setting
        /// </summary>
        [JsonProperty("effort")] public ReasoningEffort? Effort { get; set; }

        /// <summary>
        /// Optional. Non-OpenAI-style reasoning effort setting. 
        /// Cannot be used simultaneously with effort.
        /// </summary>
        [JsonProperty("max_tokens")] public int? MaxTokens { get; set; }

        /// <summary>
        /// Optional. Whether to exclude reasoning from the response
        /// Defaults to false
        /// </summary>
        [JsonProperty("exclude")] public bool? Exclude { get; set; }
    }
}