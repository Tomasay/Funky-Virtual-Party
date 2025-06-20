
using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Vector Store Files: https://platform.openai.com/docs/api-reference/vector-stores-files
    /// </summary>
    public class VectorStoreFilesBatchService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/vector_stores/{0}/file_batches";
        private const string kEndpointWithId = "{ver}/vector_stores/{0}/file_batches/{1}";

        // cancel: https://api.openai.com/v1/vector_stores/{vector_store_id}/file_batches/{batch_id}/cancel
        private const string kCancelEndpoint = "{ver}/vector_stores/{0}/file_batches/{1}/cancel";

        public VectorStoreFilesBatchService(OpenAI client, params RESTHeader[] extraHeaders) : base(client, extraHeaders)
        {
        }

        public async UniTask<VectorStoreFilesBatch> CreateAsync(string vectorStoreId, VectorStoreFilesBatchRequest req)
        {
            return await client.POSTCreateAsync<VectorStoreFilesBatchRequest, VectorStoreFilesBatch>(kEndpoint, this, req, PathParam.ID(vectorStoreId));
        }

        public async UniTask<VectorStoreFilesBatch> Retrieve(string vectorStoreId, string batchId, RequestOptions options = null)
        {
            return await client.GETRetrieveAsync<VectorStoreFilesBatch>(kEndpointWithId, this, options, PathParam.ID(vectorStoreId, batchId));
        }

        public async UniTask<QueryResponse<VectorStoreFilesBatch>> ListAsync(string vectorStoreId, CursorQuery query = null, RequestOptions options = null)
        {
            return await client.GETListAsync<CursorQuery, VectorStoreFilesBatch>(kEndpoint, this, query, options, PathParam.ID(vectorStoreId));
        }

        public async UniTask<VectorStoreFilesBatch> CancelAsync(string vectorStoreId, string batchId, RequestOptions options = null)
        {
            return await client.POSTCreateAsync<VectorStoreFilesBatch>(kCancelEndpoint, this, options, PathParam.ID(vectorStoreId, batchId));
        }
    }
}