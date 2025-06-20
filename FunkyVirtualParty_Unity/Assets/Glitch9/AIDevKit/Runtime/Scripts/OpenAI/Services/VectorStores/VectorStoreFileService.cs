using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Vector Store Files: https://platform.openai.com/docs/api-reference/vector-stores-files
    /// </summary>
    public class VectorStoreFileService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/vector_stores/{0}/files";
        private const string kEndpointWithId = "{ver}/vector_stores/{0}/files/{1}";

        public VectorStoreFileService(OpenAI client, params RESTHeader[] extraHeaders) : base(client, extraHeaders)
        {
        }

        public async UniTask<VectorStoreFile> CreateAsync(string vectorStoreId, VectorStoreFileRequest req)
        {
            return await client.POSTCreateAsync<VectorStoreFileRequest, VectorStoreFile>(kEndpoint, this, req, PathParam.ID(vectorStoreId));
        }

        public async UniTask<VectorStoreFile> RetrieveAsync(string vectorStoreId, string fileId, RequestOptions options = null)
        {
            return await client.GETRetrieveAsync<VectorStoreFile>(kEndpointWithId, this, options, PathParam.ID(vectorStoreId, fileId));
        }

        public async UniTask<QueryResponse<VectorStoreFile>> ListAsync(string vectorStoreId, CursorQuery query = null, RequestOptions options = null)
        {
            return await client.GETListAsync<CursorQuery, VectorStoreFile>(kEndpoint, this, query, options, PathParam.ID(vectorStoreId));
        }

        public async UniTask<bool> DeleteAsync(string vectorStoreId, string fileId, RequestOptions options = null)
        {
            return await client.DELETEDeleteAsync<VectorStoreFile>(kEndpointWithId, this, options, PathParam.ID(vectorStoreId, fileId));
        }
    }
}