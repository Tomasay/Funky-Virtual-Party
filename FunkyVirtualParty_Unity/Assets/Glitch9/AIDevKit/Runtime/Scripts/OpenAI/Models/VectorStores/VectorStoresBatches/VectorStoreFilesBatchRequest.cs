using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class VectorStoreFilesBatchRequest : AIRequest
    {
        /// <summary>
        /// [Required] The ID of the vector store that the File is attached to.
        /// </summary>
        [JsonProperty("file_ids")] public List<string> FileIds { get; set; }

        /// <summary>
        /// The strategy used to chunk the file.
        /// </summary>
        [JsonProperty("chunking_strategy")] public ChunkingStrategy ChunkingStrategy { get; set; }

        public class Builder : ModelRequestBuilder<Builder, VectorStoreFilesBatchRequest>
        {
            public Builder SetFileIds(List<string> fileIds)
            {
                _req.FileIds = fileIds;
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