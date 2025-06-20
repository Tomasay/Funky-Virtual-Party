using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Vector Stores: https://platform.openai.com/docs/api-reference/vector-stores
    /// </summary>
    public class VectorStoreService : CRUDService<OpenAI, VectorStore, VectorStoreRequest, CursorQuery>
    {
        private const string kEndpoint = "{ver}/vector_stores";
        public VectorStoreFileService Files { get; }
        public VectorStoreFilesBatchService FilesBatches { get; }

        public VectorStoreService(OpenAI client, params RESTHeader[] extraHeaders) : base(kEndpoint, client, extraHeaders)
        {
            Files = new VectorStoreFileService(client, extraHeaders);
            FilesBatches = new VectorStoreFilesBatchService(client, extraHeaders);
        }
    }
}