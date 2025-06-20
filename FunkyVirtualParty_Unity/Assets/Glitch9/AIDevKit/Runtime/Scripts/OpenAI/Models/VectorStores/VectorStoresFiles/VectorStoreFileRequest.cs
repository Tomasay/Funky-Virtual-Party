using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class VectorStoreFileRequest : AIRequest
    {
        /// <summary>
        /// [Required] The ID of the vector store that the File is attached to.
        /// </summary>
        [JsonProperty("file_id")] public string FileId { get; set; }

        /// <summary>
        /// The strategy used to chunk the file.
        /// </summary>
        [JsonProperty("chunking_strategy")] public ChunkingStrategy ChunkingStrategy { get; set; }

        public class Builder : ModelRequestBuilder<Builder, VectorStoreFileRequest>
        {
            public Builder SetFileId(string fileId)
            {
                _req.FileId = fileId;
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