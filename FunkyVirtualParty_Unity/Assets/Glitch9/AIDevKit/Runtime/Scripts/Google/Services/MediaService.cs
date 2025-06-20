using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Google.Services
{
    public class MediaService : CRUDServiceBase<GenerativeAI>
    {
        internal const string kUploadEndpoint = "upload/{ver}/files";
        internal const string kUploadMetadataEndpoint = "{ver}/files";
        public MediaService(GenerativeAI client) : base(client, IsBeta.MediaUpload) { }
        public async UniTask<GoogleFile> UploadAsync(GoogleFileUploadRequest req)
        => await client.POSTCreateAsync<GoogleFileUploadRequest, GoogleFile>(kUploadEndpoint, this, req, RequestOptions.MultipartFormData);
        public async UniTask<GoogleFile> UploadMetadataAsync(GoogleFile req)
        => await client.POSTCreateAsync<GoogleFile, GoogleFile>(kUploadMetadataEndpoint, this, req);
    }
}