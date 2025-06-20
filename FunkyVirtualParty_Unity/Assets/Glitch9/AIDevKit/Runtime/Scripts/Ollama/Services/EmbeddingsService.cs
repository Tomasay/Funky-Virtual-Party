using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Ollama.Services
{
    public class EmbeddingsService : CRUDServiceBase<Ollama>
    {
        private const string kEndpoint = "/api/embeddings";
        public EmbeddingsService(Ollama client) : base(client) { }

        /// <summary>
        /// Generate embeddings from a model
        /// </summary>
        /// <returns></returns>
        public async UniTask<EmbedResponse> CreateAsync(EmbedRequest request)
        {
            return await client.POSTCreateAsync<EmbedRequest, EmbedResponse>(kEndpoint, this, request);
        }
    }
}