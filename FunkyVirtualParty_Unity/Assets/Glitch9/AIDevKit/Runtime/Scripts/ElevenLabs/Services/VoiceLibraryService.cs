using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class VoiceLibraryService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/shared-voices";
        public VoiceLibraryService(ElevenLabs client) : base(client, false)
        {
        }
        public async UniTask<QueryResponse<ElevenLabsSharedVoiceData>> ListAsync(ElevenLabsQuery query = null, RequestOptions options = null)
        {
            return await client.GETListAsync<ElevenLabsQuery, ElevenLabsSharedVoiceData>(kEndpoint, this, query, options);
        }
    }
}
