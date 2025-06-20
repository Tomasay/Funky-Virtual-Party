using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// Request body for converting text to sound effects using ElevenLabs API.
    /// </summary>
    public class SoundEffectRequest : AudioRequest
    {
        /// <summary>
        /// Text to convert into a sound effect.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Optional. Length of the resulting sound effect in seconds. Must be between 0.5 and 22 seconds (inclusive).
        /// If not set, the duration will be automatically determined based on the prompt.
        /// </summary>
        [JsonProperty("duration_seconds")] public double? DurationSeconds { get; set; }

        /// <summary>
        /// Optional. Value between 0 and 1 that determines how strongly the prompt should be followed.
        /// Higher values will follow the prompt more strictly but may reduce the diversity of generations.
        /// Default is 0.3.
        /// </summary>
        [JsonProperty("prompt_influence")] public double? PromptInfluence { get; set; }
    }
}
