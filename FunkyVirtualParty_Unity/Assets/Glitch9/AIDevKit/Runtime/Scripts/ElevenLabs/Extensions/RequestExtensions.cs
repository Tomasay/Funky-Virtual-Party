
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public static class RequestExtensions
    {
        public static async UniTask<GeneratedAudio> ExecuteAsync(this SpeechRequest request)
        {
            return await ElevenLabs.DefaultInstance.TextToSpeech.CreateAsync(request);
        }

        public static async UniTask StreamAsync(this SpeechRequest request, RealtimeAudioPlayer streamAudioPlayer)
        {
            await ElevenLabs.DefaultInstance.TextToSpeech.StreamAsync(request, streamAudioPlayer);
        }

        public static async UniTask<ElevenLabsTranscript> ExecuteAsync(this TranscriptRequest request)
        {
            return await ElevenLabs.DefaultInstance.SpeechToText.CreateAsync(request);
        }

        public static async UniTask<GeneratedAudio> ExecuteAsync(this SoundEffectRequest request)
        {
            return await ElevenLabs.DefaultInstance.SoundEffects.CreateAsync(request);
        }

        public static async UniTask<QueryResponse<ElevenLabsSharedVoiceData>> ExecuteAsync(this ElevenLabsQuery request)
        {
            return await ElevenLabs.DefaultInstance.VoiceLibrary.ListAsync(request);
        }

        public static async UniTask<GeneratedAudio> ExecuteAsync(this VoiceChangerRequest request)
        {
            return await ElevenLabs.DefaultInstance.VoiceChanger.CreateAsync(request);
        }

        public static async UniTask<GeneratedAudio> ExecuteAsync(this AudioIsolationRequest request)
        {
            return await ElevenLabs.DefaultInstance.AudioIsolation.CreateAsync(request);
        }
    }
}