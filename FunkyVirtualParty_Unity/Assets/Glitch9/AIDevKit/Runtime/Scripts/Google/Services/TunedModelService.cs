using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Google.Services
{
    public class TunedModelService : CRUDServiceBase<GenerativeAI>
    {
        internal const string kEndpoint = "{ver}/tunedModels";
        internal const string kEndpointWithId = "{ver}/tunedModels/{0}";
        public TunedModelPermissionService Permissions { get; }
        public TunedModelService(GenerativeAI client) : base(client, IsBeta.TunedModels)
        => Permissions = new TunedModelPermissionService(client);
        public async UniTask<TunedModel> CreateAsync(TunedModel req)
        {
            if (string.IsNullOrEmpty(req?.Name))
            {
                return await client.POSTCreateAsync<TunedModel, TunedModel>(kEndpoint, this, req, PathParam.ID(req?.Name));
            }
            return await client.POSTCreateAsync<TunedModel, TunedModel>(kEndpoint, this, req);
        }
        public async UniTask<TunedModel> RetrieveAsync(string id, RequestOptions options = null)
        => await client.GETRetrieveAsync<TunedModel>(kEndpointWithId, this, options, PathParam.ID(id));
        public async UniTask<TunedModel> UpdateAsync(string id, IEnumerable<UpdateMask> updateMasks, RequestOptions options = null)
        => await client.PATCHUpdateAsync<TunedModel>(kEndpointWithId, this, options, updateMasks.ToPathParams(PathParam.ID(id)));
        public async UniTask<bool> DeleteAsync(string id, RequestOptions options = null)
        => await client.DELETEDeleteAsync<TunedModel>(kEndpointWithId, this, options, PathParam.ID(id));
        public async UniTask<QueryResponse<TunedModel>> ListAsync(TokenQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<TokenQuery, TunedModel>(kEndpoint, this, query, options);
        public async UniTask<bool> TransferOwnershipAsync(TransferOwnershipRequest req)
        {
            if (string.IsNullOrEmpty(req.TunedModelId)) throw new Exception("TunedModelId is null");
            RESTResponse res = await client.POSTCreateAsync(kEndpointWithId, this, req, PathParam.ID(req.TunedModelId), PathParam.Method(Methods.TransferOwnership));
            return res?.HasBody ?? false;
        }
        public async UniTask<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest req)
        {
            if (req?.Model == null) throw new ArgumentNullException(nameof(req.Model));
            string modelName = req.Model.Id;
            return await client.POSTCreateAsync<GenerateContentRequest, GenerateContentResponse>(kEndpointWithId, this, req, PathParam.ID(modelName), PathParam.Method(Methods.GenerateContent));
        }
        public async UniTask<GenerateTextResponse> GenerateTextAsync(GenerateTextRequest req)
        {
            if (req?.Model == null) throw new ArgumentNullException(nameof(req.Model));
            string modelName = req.Model.Id;
            return await client.POSTCreateAsync<GenerateTextRequest, GenerateTextResponse>(kEndpointWithId, this, req, PathParam.ID(modelName), PathParam.Method(Methods.GenerateText));
        }
    }

    public class TunedModelPermissionService : CRUDServiceBase<GenerativeAI>
    {
        private const string kEndpoint = "{ver}/models/{0}/permissions";
        private const string kEndpointWithId = "{ver}/models/{0}/permissions/{1}";
        public TunedModelPermissionService(GenerativeAI client) : base(client, IsBeta.TunedModelsPermissions) { }
        public async UniTask<Permission> CreateAsync(Permission req, string corpusId, string tunedModelId)
        => await client.POSTCreateAsync<Permission, Permission>(kEndpoint, this, req, PathParam.ID(corpusId, tunedModelId));
        public async UniTask<Permission> RetrieveAsync(string corpusId, string tunedModelId, string permissionId, RequestOptions options = null)
        => await client.GETRetrieveAsync<Permission>(kEndpointWithId, this, options, PathParam.ID(corpusId, tunedModelId, permissionId));
        public async UniTask<Permission> UpdateAsync(string corpusId, string tunedModelId, string permissionId, IEnumerable<UpdateMask> updateMasks, RequestOptions options = null)
        => await client.PATCHUpdateAsync<Permission>(kEndpointWithId, this, options, updateMasks.ToPathParams(PathParam.ID(corpusId, tunedModelId, permissionId)));
        public async UniTask<bool> DeleteAsync(string corpusId, string tunedModelId, string permissionId, RequestOptions options = null)
        => await client.DELETEDeleteAsync<Permission>(kEndpointWithId, this, options, PathParam.ID(corpusId, tunedModelId, permissionId));
        public async UniTask<QueryResponse<Permission>> ListAsync(string corpusId, string tunedModelId, TokenQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<TokenQuery, Permission>(kEndpoint, this, query, options, PathParam.ID(corpusId, tunedModelId));
    }
}