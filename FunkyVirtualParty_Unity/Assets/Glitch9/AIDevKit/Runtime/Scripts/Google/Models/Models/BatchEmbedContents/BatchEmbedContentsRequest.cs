using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Generates multiple embeddings from the model given input text in a synchronous call.
    /// </summary>
    public class BatchEmbedContentsRequest : GenerativeAIRequest
    {
        /// <summary>
        /// Required.
        /// Embed requests for the batch.
        /// The model in each of these requests must match the model specified BatchEmbedContentsRequest.model.
        /// </summary>
        [JsonProperty("requests")] public EmbedContentRequest[] Requests { get; set; }
    }
}