using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Google.Services
{
    public class CorporaService : CRUDService<GenerativeAI, Corpus, CorporaQueryRequest, TokenQuery>
    {
        private const string kEndpoint = "{ver}/corpora";
        public CorporaDocumentService Document { get; }
        public CorporaPermissionService Permission { get; }
        public CorporaService(GenerativeAI client) : base(kEndpoint, client, IsBeta.Corpora)
        {
            Document = new CorporaDocumentService(client);
            Permission = new CorporaPermissionService(client);
        }
        public async UniTask<CorporaQueryResponse> QueryAsync(CorporaQueryRequest req)
        => await client.POSTCreateAsync<CorporaQueryRequest, CorporaQueryResponse>(kEndpoint, this, req);
    }

    public class CorporaPermissionService : CRUDServiceBase<GenerativeAI>
    {
        private const string kEndpoint = "{ver}/corpora/{0}/permissions";
        private const string kEndpointWithId = "{ver}/corpora/{0}/permissions/{1}";
        public CorporaPermissionService(GenerativeAI client) : base(client, IsBeta.CorporaPermissions) { }
        public async UniTask<Permission> CreateAsync(Permission req)
        => await client.POSTCreateAsync<Permission, Permission>(kEndpoint, this, req);
        public async UniTask<bool> DeleteAsync(string id, RequestOptions options = null)
        => await client.DELETEDeleteAsync<Permission>(kEndpointWithId, this, options, PathParam.ID(id));
        public async UniTask<Permission> RetrieveAsync(string id, RequestOptions options = null)
        => await client.GETRetrieveAsync<Permission>(kEndpointWithId, this, options, PathParam.ID(id));
        public async UniTask<QueryResponse<Permission>> ListAsync(TokenQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<TokenQuery, Permission>(kEndpoint, this, query, options);
        public async UniTask<Permission> UpdateAsync(string id, IEnumerable<UpdateMask> updateMasks, RequestOptions options = null)
        => await client.PATCHUpdateAsync<Permission>(kEndpointWithId, this, options, updateMasks.ToPathParams(PathParam.ID(id)));
    }

    public class CorporaDocumentService : CRUDServiceBase<GenerativeAI>
    {
        private const string kEndpoint = "{ver}/corpora/{0}/documents";
        private const string kEndpointWithId = "{ver}/corpora/{0}/documents/{1}";
        public CorporaDocumentChunkService Chunks { get; }
        public CorporaDocumentService(GenerativeAI client) : base(client, IsBeta.CorporaDocuments)
        => Chunks = new CorporaDocumentChunkService(client);
        public async UniTask<Document> CreateAsync(Document req, string corpusId, RequestOptions options = null)
        => await client.POSTCreateAsync<Document, Document>(kEndpoint, this, req, options, PathParam.ID(corpusId));
        public async UniTask<bool> DeleteAsync(string corpusId, string documentId, RequestOptions options = null)
        => await client.DELETEDeleteAsync<Document>(kEndpointWithId, this, options, PathParam.ID(corpusId, documentId));
        public async UniTask<Document> RetrieveAsync(string corpusId, string documentId, RequestOptions options = null)
        => await client.GETRetrieveAsync<Document>(kEndpointWithId, this, options, PathParam.ID(corpusId, documentId));
        public async UniTask<QueryResponse<Document>> ListAsync(string corpusId, TokenQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<TokenQuery, Document>(kEndpoint, this, query, options, PathParam.ID(corpusId));
        public async UniTask<Document> UpdateAsync(string corpusId, string documentId, IEnumerable<UpdateMask> updateMasks, RequestOptions options = null)
        => await client.PATCHUpdateAsync<Document>(kEndpointWithId, this, options, updateMasks.ToPathParams(PathParam.ID(corpusId, documentId)));
        public async UniTask<CorporaQueryResponse> QueryAsync(CorporaQueryRequest req)
        => await client.POSTCreateAsync<CorporaQueryRequest, CorporaQueryResponse>(kEndpoint, this, req);
    }

    public class CorporaDocumentChunkService : CRUDServiceBase<GenerativeAI>
    {
        private const string kEndpoint = "{ver}/corpora/{0}/documents/{1}/chunks";
        private const string kEndpointWithId = "{ver}/corpora/{0}/documents/{1}/chunks/{2}";
        public CorporaDocumentChunkService(GenerativeAI client) : base(client, IsBeta.CorporaDocumentsChunks) { }
        public async UniTask<Chunk> CreateAsync(Chunk req) => await client.POSTCreateAsync<Chunk, Chunk>(kEndpoint, this, req);
        public async UniTask<bool> DeleteAsync(string corpusId, string documentId, string chunkId, RequestOptions options = null)
        => await client.DELETEDeleteAsync<Chunk>(kEndpointWithId, this, options, PathParam.ID(corpusId, documentId, chunkId));
        public async UniTask<Chunk> RetrieveAsync(string corpusId, string documentId, string chunkId, RequestOptions options = null)
        => await client.GETRetrieveAsync<Chunk>(kEndpointWithId, this, options, PathParam.ID(corpusId, documentId, chunkId));
        public async UniTask<QueryResponse<Chunk>> ListAsync(string corpusId, string documentId, TokenQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<TokenQuery, Chunk>(kEndpoint, this, query, options, PathParam.ID(corpusId, documentId));
        public async UniTask<Chunk> UpdateAsync(string corpusId, string documentId, string chunkId, IEnumerable<UpdateMask> updateMasks, RequestOptions options = null)
        => await client.PATCHUpdateAsync<Chunk>(kEndpointWithId, this, options, updateMasks.ToPathParams(PathParam.ID(corpusId, documentId, chunkId)));
        public async UniTask<QueryResponse<Chunk>> CreateBatchAsync(ChunkBatchRequest<CreateChunkRequest> req, string corpusId, string documentId)
        {
            return await client.POSTCreateAsync<ChunkBatchRequest<CreateChunkRequest>, QueryResponse<Chunk>>(kEndpoint,
                this, req,
                PathParam.ID(corpusId, documentId),
                PathParam.Method(Methods.BatchCreate));
        }
        public async UniTask<bool> DeleteBatchAsync(ChunkBatchRequest<DeleteChunkRequest> req, string corpusId, string documentId)
        {
            RESTResponse deleteResult = await client.POSTCreateAsync<ChunkBatchRequest<DeleteChunkRequest>, RESTResponse>(kEndpointWithId,
                this, req,
                PathParam.ID(corpusId, documentId),
                PathParam.Method(Methods.BatchDelete));

            return deleteResult.HasBody;
        }
        public async UniTask<QueryResponse<Chunk>> UpdateBatchAsync(ChunkBatchRequest<UpdateChunkRequest> req, string corpusId, string documentId)
        {
            return await client.POSTCreateAsync<ChunkBatchRequest<UpdateChunkRequest>, QueryResponse<Chunk>>(kEndpoint,
                this, req,
                PathParam.ID(corpusId, documentId),
                PathParam.Method(Methods.BatchUpdate));
        }
    }
}