
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Files Service: https://platform.openai.com/docs/api-reference/files
    /// </summary>
    public class FileService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/files";
        private const string kEndpointWithId = "{ver}/files/{0}";
        private const string kFileRefEndpoint = "{ver}/files/{0}/content";

        public FileService(OpenAI client) : base(client) { }

        public async UniTask<bool> DeleteAsync(string objectId, RequestOptions options = null)
        {
            return await client.DELETEDeleteAsync<OpenAIFile>(kEndpointWithId, this, options, PathParam.ID(objectId));
        }

        public async UniTask<QueryResponse<OpenAIFile>> ListAsync(CursorQuery query = null, RequestOptions options = null)
        {
            return await client.GETListAsync<CursorQuery, OpenAIFile>(kEndpoint, this, query, options);
        }

        /// <summary>
        /// Request to retrieve a file from the OpenAI API.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async UniTask<FileRef> RetrieveFileContentAsync(string fileId, RequestOptions options = null)
        {
            return await client.GETRetrieveAsync<FileRef>(kFileRefEndpoint, this, options, PathParam.ID(fileId));
        }

        public async UniTask<OpenAIFile> RetrieveAsync(string objectId, RequestOptions options = null)
        {
            return await client.GETRetrieveAsync<OpenAIFile>(kEndpointWithId, this, options, PathParam.ID(objectId));
        }

        // Added 2024-05-23 by Munchkin
        /// <summary>
        /// Request to upload a file to the OpenAI API.
        /// </summary>
        /// <param name="req">
        /// The request object to send to the API.
        /// </param>
        /// <returns>
        /// An object containing the FileId.
        /// </returns>
        public async UniTask<OpenAIFile> UploadAsync(FileUploadRequest req)
        {
            return await client.POSTCreateAsync<FileUploadRequest, OpenAIFile>(kEndpoint, this, req);
        }

        /// <summary>
        /// Request to upload a file to the OpenAI API.
        /// </summary>
        /// <param name="file">
        /// The file to upload.
        /// </param>
        /// <param name="purpose">
        /// The purpose of the file.
        /// </param>
        /// <returns>
        /// An object containing the FileId.
        /// </returns>
        public async UniTask<OpenAIFile> UploadAsync(IFile file, UploadPurpose purpose)
        {
            FileUploadRequest req = new FileUploadRequest.Builder().SetFile(file, purpose).Build();
            return await UploadAsync(req);
        }
    }
}