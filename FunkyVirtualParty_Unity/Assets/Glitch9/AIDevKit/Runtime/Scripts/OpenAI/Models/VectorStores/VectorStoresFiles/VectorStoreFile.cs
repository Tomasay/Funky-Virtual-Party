using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class VectorStoreFile : AIResponse
    {
        /// <summary>
        /// The total number of bytes used by the files in the vector store.
        /// </summary>
        [JsonProperty("usage_bytes")] public long UsageBytes { get; set; }

        /// <summary>
        /// The ID of the vector store that the File is attached to.
        /// </summary>
        [JsonProperty("vector_store_id")] public string VectorStoreId { get; set; }

        /// <summary>
        /// The status of the vector store files batch, which can be either in_progress, completed, cancelled or failed.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        /// <summary>
        /// The last error associated with this vector store file. Will be null if there are no errors.
        /// </summary>
        [JsonProperty("last_error")] public ErrorResponse LastError { get; set; }

        /// <summary>
        /// The strategy used to chunk the file.
        /// </summary>
        [JsonProperty("chunking_strategy")] public ChunkingStrategy ChunkingStrategy { get; set; }
    }


    public class ChunkingStrategy
    {
        /// <summary>
        /// The strategy used to chunk the file.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// The static chunking strategy.
        /// </summary>
        [JsonProperty("static")] public ChunkingStrategyStatic Static { get; set; }
    }


    public class ChunkingStrategyStatic
    {
        /// <summary>
        /// The maximum number of tokens in each chunk. The default value is 800. The minimum value is 100 and the maximum value is 4096.
        /// </summary>
        [JsonProperty("max_chunksize_tokens")] public int MaxChunkSizeTokens { get; set; }

        /// <summary>
        /// The number of tokens that overlap between chunks. The default value is 400.
        /// Note that the overlap must not exceed half of max_chunksize_tokens.
        /// </summary>
        [JsonProperty("chunkoverlap_tokens")] public int ChunkOverlapTokens { get; set; }
    }
}