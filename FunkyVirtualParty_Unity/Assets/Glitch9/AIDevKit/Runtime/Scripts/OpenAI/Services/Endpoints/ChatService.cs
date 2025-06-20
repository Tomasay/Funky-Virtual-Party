using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using System;
using System.Linq;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Chat: https://platform.openai.com/docs/api-reference/chat
    /// </summary>
    public class ChatService : CRUDServiceBase<OpenAI>
    {
        public CompletionService Completions { get; }
        public ChatService(OpenAI client) : base(client) => Completions = new CompletionService(client);
    }

    public class CompletionService : CRUDServiceBase<OpenAI>
    {
        private const string kCreateEndpoint = "{ver}/chat/completions";
        private const string kLegacyCreateEndpoint = "{ver}/completions";
        public CompletionService(OpenAI client) : base(client) { }

        public async UniTask<ChatCompletion> CreateAsync(ChatCompletionRequest req)
        {
            try
            {
                if (req.Model == null) req.Model = OpenAISettings.DefaultLLM;

                if (req.Model.IsLegacy)
                {
                    string inputMessage = req.Messages?.Count > 0 ? req.Messages.Last()?.Content : string.Empty;
                    if (string.IsNullOrEmpty(inputMessage)) throw new Exception("Input message is empty.");
                    req.Prompt = inputMessage;
                    req.Messages = null;
                    return await client.POSTCreateAsync<ChatCompletionRequest, ChatCompletion>(kLegacyCreateEndpoint, this, req);
                }

                return await client.POSTCreateAsync<ChatCompletionRequest, ChatCompletion>(kCreateEndpoint, this, req);
            }
            catch (Exception e)
            {
                client.HandleException(e);
                return null;
            }
        }

        public async UniTask StreamAsync(ChatCompletionRequest req, IChatCompletionStreamHandler streamHandler)
        {
            req.StreamHandler = streamHandler.SetFactory(client.CreateChunk);
            req.Stream = true;

            await CreateAsync(req);
        }
    }
}