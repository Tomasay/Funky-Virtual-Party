using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenRouter.Services
{
    public class ModelService : CRUDServiceBase<OpenRouter>
    {
        private const string kEndpoint = "/api/v1/models"; // GET  

        public ModelService(OpenRouter client) : base(client) { }

        public async UniTask<QueryResponse<OpenRouterModelData>> ListAsync()
        {
            return await client.GETListAsync<Query, OpenRouterModelData>(kEndpoint, this);
        }
    }
}