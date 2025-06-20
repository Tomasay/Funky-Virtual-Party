using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// The response to an EmbedContentRequest.
    /// </summary>
    public class EmbedContentResponse
    {
        /// <summary>
        /// Output only. The embedding generated from the input content.
        /// </summary>
        [JsonProperty("embedding")] public ContentEmbedding Embedding { get; set; }
    }
}