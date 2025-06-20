using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class VoiceService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v2/voices";
        private const string kEndpointWithId = "{ver}/models/{0}";

        public VoiceService(ElevenLabs client) : base(client, false) { }
        public UniTask<QueryResponse<ElevenLabsVoiceData>> ListAsync(ElevenLabsQuery query = null, RequestOptions options = null)
        {
            return client.GETListAsync<ElevenLabsQuery, ElevenLabsVoiceData>(kEndpoint, this, query, options);
        }
    }
}
