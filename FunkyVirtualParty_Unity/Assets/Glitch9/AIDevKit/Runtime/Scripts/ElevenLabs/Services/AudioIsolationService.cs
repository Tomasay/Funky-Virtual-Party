
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class AudioIsolationService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/audio-isolation/{0}"; // voiceId
        private const string kStreamEndpoint = "v1/audio-isolation/{0}/stream"; // voiceId

        public AudioIsolationService(ElevenLabs client)
            : base(client, false)
        {
        }

        public async UniTask<GeneratedAudio> CreateAsync(AudioIsolationRequest request)
        {
            request.MIMEType = MIMEType.MultipartForm;
            RESTResponse res = await client.POSTCreateAsync(kEndpoint, this, request);
            if (res == null) return null;
            return new GeneratedAudio(res.AudioOutput, res.OutputPath);
        }

        public async UniTask Stream(AudioIsolationRequest request, RealtimeAudioPlayer streamAudioPlayer)
        {
            request.MIMEType = MIMEType.MultipartForm;
            ElevenLabsOutputFormat outputFormat = ElevenLabsOutputFormat.MP3_44100_128; // default value 

            request.StreamHandler = new PcmAudioStreamHandler(
                audioFormat: outputFormat.ToAudioFormat(),
                onStream: streamAudioPlayer.PushSamples
            );

            await client.POSTCreateAsync<AudioIsolationRequest, GeneratedAudio>(kStreamEndpoint, this, request);

        }
    }
}
