using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenRouter.Services
{
    public class CompletionService : CRUDServiceBase<OpenRouter>
    {
        private const string kEndpoint = "api/v1/completions";

        public CompletionService(OpenRouter client) : base(client) { }

        public async UniTask<ChatCompletion> CreateAsync(CompletionRequest req)
        {
            if (req.Model == null) req.Model = OpenRouterSettings.DefaultLLM;
            return await client.POSTCreateAsync<CompletionRequest, ChatCompletion>(kEndpoint, this, req);
        }

        public async UniTask Stream(CompletionRequest req, IChatCompletionStreamHandler streamHandler)
        {
            if (req.Model == null) req.Model = OpenRouterSettings.DefaultLLM;
            req.Stream = true;
            req.StreamHandler = streamHandler.SetFactory(client.CreateChunk);
            await CreateAsync(req);
        }
    }
}