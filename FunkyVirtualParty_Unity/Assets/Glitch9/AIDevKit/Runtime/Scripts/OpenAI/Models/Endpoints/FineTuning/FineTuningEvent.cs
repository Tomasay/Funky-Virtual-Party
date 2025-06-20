
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Fine-tuning job event object
    /// </summary>
    public class FineTuningEvent : AIResponse
    {
        [JsonProperty("level")] public string Level { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}