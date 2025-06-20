using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class VectorStoreFilesBatch : AIResponse
    {
        /// <summary>
        /// The ID of the vector store that the File is attached to.
        /// </summary>
        [JsonProperty("vector_store_id")] public string VectorStoreId { get; set; }

        /// <summary>
        /// The status of the vector store files batch, which can be either in_progress, completed, cancelled or failed.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        /// <summary>
        /// ...?
        /// </summary>
        [JsonProperty("file_counts")] public FileCounts FileCounts { get; set; }
    }
}