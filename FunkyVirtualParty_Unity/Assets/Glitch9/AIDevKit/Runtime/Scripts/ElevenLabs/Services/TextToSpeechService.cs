using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;
using System;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class TextToSpeechService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/text-to-speech/{0}"; // voiceId
        private const string kStreamEndpoint = "v1/text-to-speech/{0}/stream"; // voiceId

        public TextToSpeechService(ElevenLabs client) : base(client, false)
        {
        }

        public async UniTask<GeneratedAudio> CreateAsync(SpeechRequest request)
        {
            if (request.Model == null) request.Model = ElevenLabsSettings.DefaultTTS;
            if (request.Voice == null) request.Voice = ElevenLabsSettings.DefaultVoice;

            RESTResponse res = await client.POSTCreateAsync(kEndpoint, this, request)
                ?? throw new NullReferenceException("Failed to create speech request.");

            return new(res.AudioOutput, request.OutputPath);
        }

        public async UniTask StreamAsync(SpeechRequest request, RealtimeAudioPlayer streamAudioPlayer)
        {
            if (request.Model == null) request.Model = ElevenLabsSettings.DefaultTTS;
            if (request.Voice == null) request.Voice = ElevenLabsSettings.DefaultVoice;

            ElevenLabsOutputFormat outputFormat = request.OutputFormat ?? ElevenLabsOutputFormat.PCM_24000; // default value 

            if (!outputFormat.ToString().StartsWith("PCM"))
            {
                client.Logger.Warning("Unity does not support streaming MP3 or Opus audio formats. " +
                    "Please use PCM format for streaming audio. " +
                    "The default format is set to PCM 24kHz.");

                outputFormat = ElevenLabsOutputFormat.PCM_24000;
            }

            request.StreamHandler = new PcmAudioStreamHandler(
                audioFormat: outputFormat.ToAudioFormat(),
                onStream: streamAudioPlayer.PushSamples
            );

            await client.POSTCreateAsync(kStreamEndpoint, this, request);
        }
    }
}