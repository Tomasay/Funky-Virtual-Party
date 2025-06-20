using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class ModelService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/models";
        public ModelService(ElevenLabs client) : base(client, false) { }
        public async UniTask<QueryResponse<ElevenLabsModelData>> ListAsync(ElevenLabsQuery query = null, RequestOptions options = null)
        {
            return await client.GETListAsync<ElevenLabsQuery, ElevenLabsModelData>(kEndpoint, this, query, options);
        }
    }
}