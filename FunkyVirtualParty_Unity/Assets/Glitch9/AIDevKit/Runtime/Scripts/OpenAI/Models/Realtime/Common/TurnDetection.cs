using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    /// <summary>
    /// Configuration for turn detection. Can be set to null to turn off.
    /// </summary>
    public class TurnDetection
    {
        /// <summary>
        /// Type of turn detection, only "server_vad" is currently supported.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; } = "server_vad";

        /// <summary>
        /// Activation threshold for VAD (0.0 to 1.0).
        /// </summary>
        [JsonProperty("threshold")] public float Threshold { get; set; } = 0.5f;

        /// <summary>
        /// Amount of audio to include before speech starts (in milliseconds).
        /// </summary>
        [JsonProperty("prefix_padding_ms")] public int PrefixPaddingMs { get; set; } = 200;

        /// <summary>
        /// Duration of silence to detect speech stop (in milliseconds).
        /// </summary>
        [JsonProperty("silence_duration_ms")] public int SilenceDurationMs { get; set; } = 500;

    }
}