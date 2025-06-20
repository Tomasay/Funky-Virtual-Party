using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class VectorStore : AIResponse
    {
        /// <summary>
        /// The name of the vector store.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The total number of bytes used by the files in the vector store.
        /// </summary>
        [JsonProperty("usage_bytes")] public long UsageBytes { get; set; }

        /// <summary>
        /// ...?
        /// </summary>
        [JsonProperty("file_counts")] public FileCounts FileCounts { get; set; }

        /// <summary>
        /// The status of the vector store, which can be either expired, in_progress, or completed.
        /// A status of completed indicates that the vector store is ready for use.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        /// <summary>
        /// The expiration policy for a vector store.
        /// </summary>
        [JsonProperty("expires_after")] public Expiration ExpiresAfter { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the vector store will expire.
        /// </summary>
        [JsonProperty("last_active_at")] public UnixTime? LastActiveAt { get; set; }
    }
}