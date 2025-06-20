using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Generates an embedding from the model given an input Content.
    /// </summary>
    public class EmbedContentRequest : GenerativeAIRequest
    {
        /// <summary>
        /// Required. The content to embed. Only the parts.text fields will be counted.
        /// </summary>
        [JsonProperty("content")] public Content Content { get; set; }

        /// <summary>
        /// Optional. Optional task type for which the embeddings will be used. Can only be set for models/embedding-001.
        /// </summary>
        [JsonProperty("taskType")] public TaskType? TaskType { get; set; }

        /// <summary>
        /// Optional. An optional title for the text. Only applicable when TaskType is RETRIEVAL_DOCUMENT.
        /// Note: Specifying a title for RETRIEVAL_DOCUMENT provides better quality embeddings for retrieval.
        /// </summary>
        [JsonProperty("title")] public string Title { get; set; }

        /// <summary>
        /// Optional. Optional reduced dimension for the output embedding. If set, excessive values in the output embedding are truncated from the end. Supported by newer models since 2024, and the earlier model (models/embedding-001) cannot specify this value.
        /// </summary>
        [JsonProperty("outputDimensionality")] public int? OutputDimensionality { get; set; }
    }
}