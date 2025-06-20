using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class VectorStoreRequest : AIRequest
    {
        /// <summary>
        /// A list of File IDs that the vector store should use. Useful for tools like file_search that can access files.
        /// </summary>
        [JsonProperty("file_ids")] public string[] FileIds { get; set; }

        /// <summary>
        /// The name of the vector store.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The expiration policy for a vector store.
        /// </summary>
        [JsonProperty("expires_after")] public Expiration ExpiresAfter { get; set; }

        /// <summary>
        /// The chunking strategy used to chunk the file(s). If not set, will use the auto strategy.
        /// Only applicable if file_ids is non-empty.
        /// </summary>
        [JsonProperty("chunking_strategy")] public ChunkingStrategy ChunkingStrategy { get; set; }

        public class Builder : ModelRequestBuilder<Builder, VectorStoreRequest>
        {
            public Builder SetFileIds(string[] fileIds)
            {
                _req.FileIds = fileIds;
                return this;
            }

            public Builder SetName(string name)
            {
                _req.Name = name;
                return this;
            }

            public Builder SetExpiresAfter(Expiration expiresAfter)
            {
                _req.ExpiresAfter = expiresAfter;
                return this;
            }

            public Builder SetChunkingStrategy(ChunkingStrategy chunkingStrategy)
            {
                _req.ChunkingStrategy = chunkingStrategy;
                return this;
            }
        }
    }
}