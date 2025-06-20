using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class ChunkBatchRequest<T> : RequestBody
        where T : IChunkRequest
    {
        /// <summary>
        /// Required. A maximum of 100 requests can be in a batch.
        /// </summary>
        [JsonProperty("requests")] public T[] Requests { get; set; }
    }

    /// <summary>
    /// Interface for all Chunk requests.
    /// </summary>
    public interface IChunkRequest
    {
    }

    /// <summary>
    /// Request to create a Chunk.
    /// </summary>
    public class CreateChunkRequest : IChunkRequest
    {
        /// <summary>
        /// Required. The name of the Document where this Chunk will be created. Example: corpora/my-corpus-123/documents/the-doc-abc
        /// </summary>
        [JsonProperty("parent")] public string Parent { get; set; }

        /// <summary>
        /// Required. The Chunk to create.
        /// </summary>
        [JsonProperty("chunk")] public Chunk Chunk { get; set; }
    }

    /// <summary>
    /// Request to delete a Chunk.
    /// </summary>
    public class DeleteChunkRequest : IChunkRequest
    {
        /// <summary>
        /// Required. The resource name of the Chunk to delete. Example: corpora/my-corpus-123/documents/the-doc-abc/chunks/some-chunk
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }
    }

    /// <summary>
    /// Request to update a Chunk.
    /// </summary>
    public class UpdateChunkRequest : IChunkRequest
    {
        /// <summary>
        /// Required. The Chunk to update.
        /// </summary>
        [JsonProperty("chunk")] public Chunk Chunk { get; set; }

        /// <summary>
        /// Required. The list of fields to update. Currently, this only supports updating customMetadata and data.
        /// This is a comma-separated list of fully qualified names of fields.Example: "user.displayName,photo".
        /// </summary>
        [JsonProperty("updateMask")] public string UpdateMask { get; set; }
    }
}