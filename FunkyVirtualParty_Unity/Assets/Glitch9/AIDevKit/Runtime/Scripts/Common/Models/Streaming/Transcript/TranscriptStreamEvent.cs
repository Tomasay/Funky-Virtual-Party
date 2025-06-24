using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public class TranscriptStreamEvent : IGeneratedResult
    {
        /// <summary>
        /// Required. The type of the event.transcript.text.delta or transcript.text.done
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// Required. The text delta that was additionally transcribed.
        /// </summary>
        [JsonProperty("delta")] public string Delta { get; set; }

        /// <summary>
        /// Required. The text that was transcribed.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Optional. The log probabilities of the individual tokens in the transcription. Only included 
        /// if you create a transcription with the include[] parameter set to logprobs.
        /// </summary>
        [JsonProperty("logprobs")] public List<LogProb> Logprobs { get; set; }

        /// <summary>
        /// Required. Usage statistics for models billed by token usage.
        /// </summary>
        [JsonProperty("usage")] public Usage Usage { get; set; }

        [JsonIgnore] public int Count => 1;
    }
}