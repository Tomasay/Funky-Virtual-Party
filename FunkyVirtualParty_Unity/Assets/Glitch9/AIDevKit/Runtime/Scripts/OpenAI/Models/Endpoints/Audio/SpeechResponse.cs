using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class SpeechAudioDeltaEvent
    {
        /// <summary>
        /// Required. The type of the event. Always speech.audio.delta.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// Required. A chunk of Base64-encoded audio data.
        /// </summary>
        [JsonProperty("audio")] public string Base64Audio { get; set; }
    }

    public class SpeechAudioDoneEvent
    {
        /// <summary>
        /// Required. The type of the event. Always speech.audio.done.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// Required. Token usage statistics for the request.
        /// </summary>
        [JsonProperty("usage")] public Usage Usage { get; set; }
    }
}