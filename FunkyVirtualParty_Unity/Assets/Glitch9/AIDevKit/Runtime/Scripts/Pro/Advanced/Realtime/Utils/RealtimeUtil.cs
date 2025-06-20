using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public static class RealtimeUtil
    {
        private const string RealtimeWebSocketUrl = "wss://api.openai.com/v1/realtime";
        public static string ResolveRealtimeUrl(Model model) => RealtimeWebSocketUrl + "?model=" + model;


        public static UniTask<string> InputAudioToBase64EncodedAudio(RealtimeAudioFormat inputFormat, string audioFilePath)
        {
            return RealtimeAudioFormatter.AudioToBase64EncodedAudio(inputFormat, audioFilePath);
        }

        public static string InputAudioToBase64EncodedAudio(RealtimeAudioFormat inputFormat, AudioClip audioClip)
        {
            return RealtimeAudioFormatter.AudioToBase64EncodedAudio(inputFormat, audioClip);
        }

        public static string InputAudioToBase64EncodedAudio(RealtimeAudioFormat inputFormat, float[] audioData)
        {
            return RealtimeAudioFormatter.AudioToBase64EncodedAudio(inputFormat, audioData);
        }

        public static AudioClip Base64EncodedAudioToAudioClip(RealtimeAudioFormat outputFormat, string base64EncodedAudio, int unitySampleRate, int unityChannels)
        {
            return RealtimeAudioFormatter.Base64EncodedAudioToAudioClip(outputFormat, base64EncodedAudio, unitySampleRate, unityChannels);
        }

        public static float[] Base64EncodedAudioToAudioData(RealtimeAudioFormat outputFormat, string base64EncodedAudio)
        {
            return RealtimeAudioFormatter.Base64EncodedAudioToAudioData(outputFormat, base64EncodedAudio);
        }

        public static int GetChannelCount(AudioSpeakerMode audioSpeakerMode)
        {
            return audioSpeakerMode switch
            {
                AudioSpeakerMode.Mono => 1,
                AudioSpeakerMode.Stereo => 2,
                AudioSpeakerMode.Quad => 4,
                AudioSpeakerMode.Surround => 5,
                AudioSpeakerMode.Mode5point1 => 6,
                AudioSpeakerMode.Mode7point1 => 8,
                AudioSpeakerMode.Prologic => 2,
                _ => 2,
            };
        }

        /// <summary>
        /// Resample audio data from the Realtime API to match Unity's audio settings, considering the number of channels.
        /// </summary>
        /// <param name="audioData">Original audio data</param>
        /// <param name="outputFormat">Original audio format (contains sample rate)</param>
        /// <param name="unitySampleRate">Target Unity sample rate</param>
        /// <param name="unityChannels">Number of audio channels</param>
        /// <returns>Resampled audio data</returns>
        public static float[] ResampleAudio(float[] audioData, RealtimeAudioFormat outputFormat, int unitySampleRate, int unityChannels)
        {
            int originalSampleRate = outputFormat.GetSampleRate();
            int originalChannels = outputFormat.GetChannelCount();
            return ResampleAudio(audioData, originalSampleRate, unitySampleRate, originalChannels, unityChannels);
        }

        /// <summary>
        /// Resample audio data from the Realtime API to match Unity's audio settings, considering the number of channels.
        /// </summary>
        /// <param name="audioData">Original audio data</param>
        /// <param name="originalSampleRate">Original sample rate</param>
        /// <param name="unitySampleRate">Target Unity sample rate</param>
        /// <param name="originalChannels">Original audio data channel count</param>
        /// <param name="unityChannels">Unity audio output channel count</param>
        /// <returns>Resampled audio data</returns>
        public static float[] ResampleAudio(float[] audioData, int originalSampleRate, int unitySampleRate, int originalChannels, int unityChannels)
        {
            int samplesPerChannel = audioData.Length / originalChannels;
            int newSamplesPerChannel = (int)((float)samplesPerChannel * unitySampleRate / originalSampleRate);
            float[] resampledData = new float[newSamplesPerChannel * unityChannels];

            for (int i = 0; i < newSamplesPerChannel; i++)
            {
                for (int c = 0; c < unityChannels; c++)
                {
                    int originalChannelIndex = c % originalChannels; // Handle mismatched channel counts
                    float interpFactor = (float)i * originalSampleRate / unitySampleRate;
                    int originalIndex = Mathf.FloorToInt(interpFactor) * originalChannels + originalChannelIndex;
                    resampledData[i * unityChannels + c] = audioData[originalIndex];
                }
            }

            return resampledData;
        }
    }
}