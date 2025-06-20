using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Messages: https://platform.openai.com/docs/api-reference/messages
    /// </summary>
    public class MessageService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/threads/{0}/messages";
        private const string kEndpointWithId = "{ver}/threads/{0}/messages/{1}";
        public MessageService(OpenAI client, params RESTHeader[] extraHeaders) : base(client, extraHeaders) { }


        public async UniTask<ThreadMessage> CreateAsync(string threadId, ThreadMessageRequest req)
        {
            try
            {
                ThrowIf.ArgumentIsNull(req);
                ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));

                if (req.UploadFiles.IsNotNullOrEmpty())
                {
                    List<string> fileIds = new();
                    List<string> imageFileIds = new();

                    foreach (IFile i in req.UploadFiles)
                    {
                        if (i == null) continue;

                        if (i is RawFile file)
                        {
                            FileUploadRequest uploadReq = new FileUploadRequest.Builder().SetFile(file, UploadPurpose.Assistants).Build();
                            OpenAIFile uploadRes = await client.Files.UploadAsync(uploadReq) ?? throw new Exception("Failed to upload file.");
                            fileIds.Add(uploadRes.Id);
                        }
                        else if (i is File<Texture2D> image)
                        {
                            FileUploadRequest uploadReq = new FileUploadRequest.Builder().SetFile(image, UploadPurpose.Assistants).Build();
                            OpenAIFile uploadRes = await client.Files.UploadAsync(uploadReq) ?? throw new Exception("Failed to upload image file.");
                            imageFileIds.Add(uploadRes.Id);
                        }
                    }

                    req.SetFileIds(fileIds);
                    req.SetImageFileIds(imageFileIds);
                }

                return await client.POSTCreateAsync<ThreadMessageRequest, ThreadMessage>(kEndpoint, this, req, PathParam.ID(threadId));
            }
            catch (Exception e)
            {
                client.HandleException(e);
                return null;
            }
        }

        public async UniTask<QueryResponse<ThreadMessage>> ListAsync(string threadId, CursorQuery query = null, RequestOptions options = null)
        {
            ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
            return await client.GETListAsync<CursorQuery, ThreadMessage>(kEndpoint, this, query, options, PathParam.ID(threadId));
        }

        public async UniTask<ThreadMessage> RetrieveAsync(string threadId, string messageId, RequestOptions options = null)
        {
            ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
            ThrowIf.IsNullOrEmpty(messageId, nameof(messageId));
            return await client.GETRetrieveAsync<ThreadMessage>(kEndpointWithId, this, options, PathParam.ID(threadId, messageId));
        }

        public async UniTask<ThreadMessage> UpdateAsync(string threadId, string messageId, ThreadMessageRequest req)
        {
            ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
            ThrowIf.IsNullOrEmpty(messageId, nameof(messageId));
            return await client.POSTUpdateAsync<ThreadMessageRequest, ThreadMessage>(kEndpointWithId, this, req, PathParam.ID(threadId, messageId));
        }

        public async UniTask<bool> DeleteAsync(string threadId, string messageId, RequestOptions options = null)
        {
            ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
            ThrowIf.IsNullOrEmpty(messageId, nameof(messageId));
            return await client.DELETEDeleteAsync<ThreadMessage>(kEndpointWithId, this, options, PathParam.ID(threadId, messageId));
        }

        public async UniTask<QueryResponse<MessageFile>> ListFilesAsync(string threadId, string messageId, CursorQuery query = null, RequestOptions options = null)
        {
            ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
            ThrowIf.IsNullOrEmpty(messageId, nameof(messageId));
            return await client.GETListAsync<CursorQuery, MessageFile>(kEndpoint, this, query, options, PathParam.ID(threadId, messageId));
        }

        public async UniTask<MessageFile> RetrieveFileAsync(string threadId, string messageId, string fileId, RequestOptions options = null)
        {
            ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
            ThrowIf.IsNullOrEmpty(messageId, nameof(messageId));
            ThrowIf.IsNullOrEmpty(fileId, nameof(fileId));
            return await client.GETRetrieveAsync<MessageFile>(kEndpointWithId, this, options, PathParam.ID(threadId, messageId, fileId));
        }
    }
}