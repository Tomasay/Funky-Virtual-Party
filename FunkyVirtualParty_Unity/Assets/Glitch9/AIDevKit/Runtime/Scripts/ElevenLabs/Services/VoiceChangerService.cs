using System;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class VoiceChangerService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/speech-to-speech/{0}"; // voiceId
        private const string kStreamEndpoint = "v1/speech-to-speech/{0}/stream"; // voiceId

        public VoiceChangerService(ElevenLabs client) : base(client, false)
        {
        }

        public async UniTask<GeneratedAudio> CreateAsync(VoiceChangerRequest request)
        {
            //request.Model ??= ElevenLabsSettings.DefaultVCM;
            if (request.Model == null) request.Model = ElevenLabsSettings.DefaultVCM;
            if (request.Voice == null) request.Voice = ElevenLabsSettings.DefaultVoice;
            request.MIMEType = MIMEType.MultipartForm;
            RESTResponse res = await client.POSTCreateAsync(kEndpoint, this, request) ?? throw new NullReferenceException("Failed to create voice changer request.");
            return new(res.AudioOutput, request.OutputPath);
        }

        public async UniTask Stream(VoiceChangerRequest request, RealtimeAudioPlayer player)
        {
            ElevenLabsOutputFormat outputFormat = request.OutputFormat ?? ElevenLabsOutputFormat.MP3_44100_128; // default value 

            request.StreamHandler = new PcmAudioStreamHandler(
                audioFormat: outputFormat.ToAudioFormat(),
                onStream: player.PushSamples
            );

            if (request.Model == null) request.Model = ElevenLabsSettings.DefaultVCM;
            if (request.Voice == null) request.Voice = ElevenLabsSettings.DefaultVoice;
            request.MIMEType = MIMEType.MultipartForm;
            await client.POSTCreateAsync<VoiceChangerRequest, GeneratedAudio>(kStreamEndpoint, this, request);
        }
    }
}