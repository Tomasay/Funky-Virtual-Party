using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Response from calling {@link GenerativeModel.batchEmbedContents}.
    /// </summary>
    public class BatchEmbedContentsResponse
    {
        [JsonProperty("embeddings")] public ContentEmbedding[] Embeddings { get; set; }
    }
}