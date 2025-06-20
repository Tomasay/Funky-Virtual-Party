using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenRouter.Services
{
    public class ChatCompletionService : CRUDServiceBase<OpenRouter>
    {
        private const string kEndpoint = "api/v1/chat/completions";
        public ChatCompletionService(OpenRouter client) : base(client) { }

        public async UniTask<ChatCompletion> CreateAsync(ChatCompletionRequest req)
        {
            if (req.Model == null) req.Model = OpenRouterSettings.DefaultLLM;
            return await client.POSTCreateAsync<ChatCompletionRequest, ChatCompletion>(kEndpoint, this, req);
        }

        public async UniTask Stream(ChatCompletionRequest req, IChatCompletionStreamHandler streamHandler)
        {
            if (req.Model == null) req.Model = OpenRouterSettings.DefaultLLM;

            req.Stream = true;
            req.StreamHandler = streamHandler.SetFactory(client.CreateChunk);
            await CreateAsync(req);
        }
    }
}