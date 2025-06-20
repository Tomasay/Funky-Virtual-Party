using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Ollama
{
    /// <summary>
    /// It's a stream of JSON objects.
    /// </summary>
    public class ModelStreamResponse
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("digest")] public string Digest { get; set; }
        [JsonProperty("total")] public long? Total { get; set; }
        [JsonProperty("completed")] public long? Completed { get; set; }
    }

    /// <summary>
    /// This is not LIST. This is GET.
    /// Response object containing a list of models available in the local system.
    /// </summary>
    public class ListModelsResponse
    {
        /// <summary>
        /// Required. List of models available.
        /// </summary>
        [JsonProperty("models")] public List<OllamaModelData> Models { get; set; }
    }
}
