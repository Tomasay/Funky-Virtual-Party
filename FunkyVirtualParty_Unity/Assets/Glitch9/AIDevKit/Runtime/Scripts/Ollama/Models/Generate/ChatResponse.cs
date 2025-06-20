using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    public class ChatResponse : OllamaTextResponse
    {
        /// <summary>
        /// Required. The response message.
        /// </summary>
        [JsonProperty("message")] public ResponseMessage Message { get; set; }

        public override string ToString() => Message?.ToString();
    }

    public class ChatDeltaResponse : OllamaTextResponse
    {
        /// <summary>
        /// Required. The response message.
        /// </summary>
        [JsonProperty("message")] public ChatDelta Delta { get; set; }
        [JsonProperty("done")] public bool? Done { get; set; }

        public string FirstTextDelta() => Delta?.ToString();
        public override string ToString() => Delta?.ToString();
    }
}