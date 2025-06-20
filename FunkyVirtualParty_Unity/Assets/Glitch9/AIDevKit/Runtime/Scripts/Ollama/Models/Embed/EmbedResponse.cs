using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    /// <summary>
    /// The response from the /embed endpoint.
    /// Contains embedding vectors and metadata.
    /// </summary>
    public class EmbedResponse : OllamaResponse
    {
        /// <summary>
        /// A list of embedding vectors for each input.
        /// </summary>
        [JsonProperty("embeddings")] public List<List<float>> Embeddings { get; set; }
    }
}
