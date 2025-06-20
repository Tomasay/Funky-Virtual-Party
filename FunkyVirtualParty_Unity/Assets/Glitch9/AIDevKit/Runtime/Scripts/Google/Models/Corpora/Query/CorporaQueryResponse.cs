using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Response from corpora.query containing a list of relevant chunks.
    /// </summary>
    public class CorporaQueryResponse
    {
        /// <summary>
        /// The relevant chunks.
        /// </summary>
        [JsonProperty("relevantChunks")] public RelevantChunk[] RelevantChunks { get; set; }
    }

    /// <summary>
    /// The information for a chunk relevant to a query.
    /// </summary>
    public class RelevantChunk
    {
        /// <summary>
        /// Chunk relevance to the query.
        /// </summary>
        [JsonProperty("chunkRelevanceScore")] public double ChunkRelevanceScore { get; set; }

        /// <summary>
        /// Chunk associated with the query.
        /// </summary>
        [JsonProperty("chunk")] public Chunk Chunk { get; set; }
    }
}