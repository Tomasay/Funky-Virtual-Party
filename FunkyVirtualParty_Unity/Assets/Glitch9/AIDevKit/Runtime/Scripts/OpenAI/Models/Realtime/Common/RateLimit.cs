using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public class RateLimit
    {
        /// <summary>
        /// The name of the rate limit ("requests", "tokens", "input_tokens", "output_tokens").
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The maximum allowed value for the rate limit.
        /// </summary>
        [JsonProperty("limit")] public int Limit { get; set; }

        /// <summary>
        /// The remaining value before the limit is reached.
        /// </summary>
        [JsonProperty("remaining")] public int Remaining { get; set; }

        /// <summary>
        /// Seconds until the rate limit resets.
        /// </summary>
        [JsonProperty("reset_seconds")] public float ResetSeconds { get; set; }
    }
}
