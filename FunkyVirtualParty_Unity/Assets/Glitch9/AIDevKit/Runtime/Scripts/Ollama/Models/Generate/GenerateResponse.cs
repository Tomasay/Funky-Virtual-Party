using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    public class GenerateResponse : OllamaTextResponse
    {
        /// <summary>
        /// Required. The generated response content.
        /// </summary>
        [JsonProperty("response")] public string Response { get; set; }
        public override string ToString() => Response;
        public string FirstTextDelta() => Response;
    }
}