using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class SpeechToTextService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/speech-to-text";

        public SpeechToTextService(ElevenLabs client) : base(client, false)
        {
        }

        public async UniTask<ElevenLabsTranscript> CreateAsync(TranscriptRequest request)
        {
            if (request.Model == null) request.Model = ElevenLabsSettings.DefaultSTT;
            request.MIMEType = MIMEType.MultipartForm;
            return await client.POSTCreateAsync<TranscriptRequest, ElevenLabsTranscript>(kEndpoint, this, request);
        }
    }
}
