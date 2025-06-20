using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// A Chunk is a subpart of a Document that is treated as an independent unit for the purposes of vector representation and storage. A Corpus can have a maximum of 1 million Chunks.
    /// Patch request has 'updateMask' query parameter: the list of fields to update. Currently, this only supports updating customMetadata and data.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// Immutable. Identifier. The Chunk resource name. The ID (name excluding the "corpora/*/documents/*/chunks/" prefix) can contain up to 40 characters that are lowercase alphanumeric or dashes (-). The ID cannot start or end with a dash. If the name is empty on create, a random 12-character unique ID will be generated. Example: corpora/{corpus_id}/documents/{document_id}/chunks/123a456b789c
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Required. The content for the Chunk, such as the text string. The maximum number of tokens per chunk is 2043.
        /// </summary>
        [JsonProperty("data")] public ChunkData Data { get; set; }

        /// <summary>
        /// Optional. User provided custom metadata stored as key-value pairs. The maximum number of CustomMetadata per chunk is 20.
        /// </summary>
        [JsonProperty("customMetadata"), JsonConverter(typeof(CustomMetadataConverter))]
        public Dictionary<string, CustomMetadataValue> CustomMetadata { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the Chunk was created.
        /// </summary>
        [JsonProperty("createTime")] public ZuluTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the Chunk was last updated.
        /// </summary>
        [JsonProperty("updateTime")] public ZuluTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. Current state of the Chunk.
        /// </summary>
        [JsonProperty("state")] public State? State { get; set; }

    }

    /// <summary>
    /// Extracted data that represents the Chunk content.
    /// </summary>
    public class ChunkData
    {
        /// <summary>
        /// The Chunk content as a string. The maximum number of tokens per chunk is 2043.
        /// </summary>
        [JsonProperty("stringValue")] public string StringValue { get; set; }
    }

    /// <summary>
    /// States for the lifecycle of a Chunk.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The default value. This value is used if the state is omitted.
        /// </summary>
        [ApiEnum("STATE_UNSPECIFIED")] Unspecified,

        /// <summary>
        /// Chunk is being processed (embedding and vector storage).
        /// </summary>
        [ApiEnum("STATE_PENDING_PROCESSING")] PendingProcessing,

        /// <summary>
        /// Chunk is processed and available for querying.
        /// </summary>
        [ApiEnum("STATE_ACTIVE")] Active,

        /// <summary>
        /// Chunk failed processing.
        /// </summary>
        [ApiEnum("STATE_FAILED")] Failed
    }
}