using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Embeddings Service: https://platform.openai.com/docs/api-reference/embeddings
    /// </summary>
    public class EmbeddingService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/embeddings";
        public EmbeddingService(OpenAI client) : base(client) { }

        /// <summary>
        /// Creates an embedding vector representing the input text.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async UniTask<Embedding> CreateAsync(EmbeddingRequest req)
        {
            req.Model ??= OpenAISettings.DefaultEMB;
            return await client.POSTCreateAsync<EmbeddingRequest, Embedding>(kEndpoint, this, req);
        }
    }
}