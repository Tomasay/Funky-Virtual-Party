using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    internal static class RealtimeAudioFormatter
    {
        internal static async UniTask<string> AudioToBase64EncodedAudio(RealtimeAudioFormat audioFormat, string audioFilePath)
        {
            return audioFormat switch
            {
                RealtimeAudioFormat.PCM16 => await AudioProcessor.ProcessPCM16Audio(audioFilePath),
                RealtimeAudioFormat.G711_ULAW => await AudioProcessor.ProcessG711uLawAudio(audioFilePath),
                RealtimeAudioFormat.G711_ALAW => await AudioProcessor.ProcessG711aLawAudio(audioFilePath),
                _ => await AudioProcessor.ProcessPCM16Audio(audioFilePath),
            };
        }

        internal static string AudioToBase64EncodedAudio(RealtimeAudioFormat audioFormat, AudioClip audioClip)
        {
            return audioFormat switch
            {
                RealtimeAudioFormat.PCM16 => audioClip.EncodeToBase64PCM16(),
                RealtimeAudioFormat.G711_ULAW => audioClip.EncodeToBase64G711uLaw(),
                RealtimeAudioFormat.G711_ALAW => audioClip.EncodeToBase64G711aLaw(),
                _ => audioClip.EncodeToBase64PCM16(),
            };
        }

        internal static string AudioToBase64EncodedAudio(RealtimeAudioFormat audioFormat, float[] audioData)
        {
            return audioFormat switch
            {
                RealtimeAudioFormat.PCM16 => AudioProcessor.FloatArrayToPCM16Base64(audioData),
                RealtimeAudioFormat.G711_ULAW => AudioProcessor.FloatArrayToG711uLawBase64(audioData),
                RealtimeAudioFormat.G711_ALAW => AudioProcessor.FloatArrayToG711aLawBase64(audioData),
                _ => AudioProcessor.FloatArrayToPCM16Base64(audioData),
            };
        }

        internal static AudioClip Base64EncodedAudioToAudioClip(RealtimeAudioFormat audioFormat, string base64EncodedAudio, int sampleRate = 44100, int channels = 2)
        {
            return audioFormat switch
            {
                RealtimeAudioFormat.PCM16 => AudioProcessor.PCM16Base64ToAudioClip(base64EncodedAudio, sampleRate, channels),
                RealtimeAudioFormat.G711_ULAW => AudioProcessor.G711uLawBase64ToAudioClip(base64EncodedAudio, sampleRate, channels),
                RealtimeAudioFormat.G711_ALAW => AudioProcessor.G711aLawBase64ToAudioClip(base64EncodedAudio, sampleRate, channels),
                _ => AudioProcessor.PCM16Base64ToAudioClip(base64EncodedAudio, sampleRate, channels),
            };
        }

        internal static float[] Base64EncodedAudioToAudioData(RealtimeAudioFormat audioFormat, string base64EncodedAudio)
        {
            return audioFormat switch
            {
                RealtimeAudioFormat.PCM16 => AudioProcessor.PCM16ToFloatArray(base64EncodedAudio),
                RealtimeAudioFormat.G711_ULAW => AudioProcessor.G711uLawToFloatArray(base64EncodedAudio),
                RealtimeAudioFormat.G711_ALAW => AudioProcessor.G711aLawToFloatArray(base64EncodedAudio),
                _ => AudioProcessor.PCM16ToFloatArray(base64EncodedAudio),
            };
        }
    }
}