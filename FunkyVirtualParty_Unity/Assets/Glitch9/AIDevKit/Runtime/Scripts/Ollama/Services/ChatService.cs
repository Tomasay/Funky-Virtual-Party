using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Ollama.Services
{
    public class ChatService : CRUDServiceBase<Ollama>
    {
        private const string kEndpoint = "api/chat";
        public ChatService(Ollama client) : base(client) { }

        public async UniTask<ChatResponse> CreateAsync(ChatCompletionRequest req)
        {
            if (req.Model == null) req.Model = OllamaSettings.DefaultModel;
            req.Stream = false; // Ensure stream is false for non-streaming requests
            return await client.POSTCreateAsync<ChatCompletionRequest, ChatResponse>(kEndpoint, this, req);
        }

        public async UniTask StreamAsync(ChatCompletionRequest req, IChatCompletionStreamHandler streamHandler)
        {
            if (req.Model == null) req.Model = OllamaSettings.DefaultModel;

            req.StreamHandler = streamHandler.SetFactory(client.CreateChunk);
            req.Stream = true;

            await client.POSTCreateAsync<ChatCompletionRequest, ChatDeltaResponse>(kEndpoint, this, req);
        }
    }
}