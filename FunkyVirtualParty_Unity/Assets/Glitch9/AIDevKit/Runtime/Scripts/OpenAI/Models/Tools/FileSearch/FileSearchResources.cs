using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public sealed class FileSearchResources
    {
        /// <summary>
        /// The vector store attached to this thread. 
        /// There can be a maximum of 1 vector store attached to the thread.
        /// </summary>
        [JsonProperty("vector_store_ids")] public string[] VectorStoreIds { get; set; }
        [JsonProperty("vector_stores")] public VectorStore[] VectorStores { get; set; }
    }
}