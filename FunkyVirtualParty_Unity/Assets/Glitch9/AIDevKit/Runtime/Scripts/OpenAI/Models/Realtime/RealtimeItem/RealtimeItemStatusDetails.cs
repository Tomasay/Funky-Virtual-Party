
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public class RealtimeItemStatusDetails
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("error")] public AIDevKit.ErrorResponse Error { get; set; }
    }
}