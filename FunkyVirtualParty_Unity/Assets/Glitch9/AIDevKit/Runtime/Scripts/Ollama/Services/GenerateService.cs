using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Ollama.Services
{
    public class GenerateService : CRUDServiceBase<Ollama>
    {
        private const string kEndpoint = "api/generate";
        public GenerateService(Ollama client) : base(client) { }

        public async UniTask<GenerateResponse> CreateAsync(CompletionRequest req)
        {
            if (req.Model == null) req.Model = OllamaSettings.DefaultModel;
            req.Stream = false; // Ensure stream is false for non-streaming requests
            return await client.POSTCreateAsync<CompletionRequest, GenerateResponse>(kEndpoint, this, req);
        }

        public async UniTask StreamAsync(CompletionRequest req, IChatCompletionStreamHandler streamHandler)
        {
            if (req.Model == null) req.Model = OllamaSettings.DefaultModel;

            req.StreamHandler = streamHandler.SetFactory(client.CreateChunk);
            req.Stream = true;

            await CreateAsync(req);
        }
    }
}