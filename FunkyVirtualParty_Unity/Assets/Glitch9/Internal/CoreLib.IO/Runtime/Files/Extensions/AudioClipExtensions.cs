using System;
using Glitch9.CoreLib.IO.Audio;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class AudioClipExtensions
    {
        public static byte[] EncodeToWAV(this AudioClip clip) => WavUtil.FromAudioClip(clip);

        public static byte[] EncodeToPCM16(this AudioClip audioClip)
        {
            float[] audioData = new float[audioClip.samples];
            audioClip.GetData(audioData, 0);
            return AudioProcessor.FloatTo16BitPCM(audioData);
        }

        public static byte[] EncodeToG711uLaw(this AudioClip audioClip)
        {
            float[] audioData = new float[audioClip.samples];
            audioClip.GetData(audioData, 0);
            return AudioProcessor.FloatToG711uLaw(audioData);
        }

        public static byte[] EncodeToG711aLaw(this AudioClip audioClip)
        {
            float[] audioData = new float[audioClip.samples];
            audioClip.GetData(audioData, 0);
            return AudioProcessor.FloatToG711aLaw(audioData);
        }

        // Converts an AudioClip into base64-encoded PCM16 data
        public static string EncodeToBase64PCM16(this AudioClip audioClip)
        {
            byte[] pcm16Data = audioClip.EncodeToPCM16();
            return Convert.ToBase64String(pcm16Data);
        }

        // Converts an AudioClip into base64-encoded G.711 μ-law data
        public static string EncodeToBase64G711uLaw(this AudioClip audioClip)
        {
            byte[] g711uLawData = audioClip.EncodeToG711uLaw();
            return Convert.ToBase64String(g711uLawData);
        }

        // Converts an AudioClip into base64-encoded G.711 A-law data
        public static string EncodeToBase64G711aLaw(this AudioClip audioClip)
        {
            byte[] g711aLawData = audioClip.EncodeToG711aLaw();
            return Convert.ToBase64String(g711aLawData);
        }

        public static string EncodeToBase64PCM16WAV(this AudioClip clip)
        {
            float[] audioData = new float[clip.samples * clip.channels];
            clip.GetData(audioData, 0);

            byte[] pcm16 = AudioProcessor.FloatTo16BitPCM(audioData);
            int sampleRate = clip.frequency;
            int channels = clip.channels;
            int bitsPerSample = 16;

            byte[] wav = WavUtil.WriteFileHeader(pcm16, sampleRate, channels, bitsPerSample);
            return Convert.ToBase64String(wav);
        }

        /// <summary>
        /// Removes silence from the beginning and end of an audio clip based on a minimum amplitude threshold.
        /// </summary>
        /// <param name="clip">The source AudioClip to trim.</param> 
        /// <param name="threshold">
        /// The amplitude threshold below which audio is considered silence.
        /// Recommended values:
        /// - 0.001f: Very sensitive – retains even the smallest sounds at the start/end.
        /// - 0.005f ~ 0.01f: Normal – good for general speech, TTS, or dialogue trimming.
        /// - 0.02f ~ 0.05f: Aggressive – trims aggressively, useful for noisy input or fast processing.
        /// </param>
        /// <returns>A new AudioClip with silence trimmed from the start and end.</returns>
        public static AudioClip TrimSilence(this AudioClip clip, float threshold = 0.01f)
        {
            var rawData = new float[clip.samples * clip.channels];
            clip.GetData(rawData, 0);

            int startIndex = 0;
            while (startIndex < rawData.Length && Mathf.Abs(rawData[startIndex]) <= threshold)
                startIndex++;

            int endIndex = rawData.Length - 1;
            while (endIndex > startIndex && Mathf.Abs(rawData[endIndex]) <= threshold)
                endIndex--;

            int trimmedLength = endIndex - startIndex + 1;
            if (trimmedLength <= 0)
                return AudioClip.Create(clip.name + "_trimmed", 0, clip.channels, clip.frequency, false);

            var trimmedData = new float[trimmedLength];
            Array.Copy(rawData, startIndex, trimmedData, 0, trimmedLength);

            var trimmedClip = AudioClip.Create(clip.name + "_trimmed", trimmedLength / clip.channels, clip.channels, clip.frequency, false);
            trimmedClip.SetData(trimmedData, 0);

            return trimmedClip;
        }

        public static AudioClip ToAudioClip(this float[] samples, int sampleRate = 16000, int channels = 1, string clipName = "recorded_clip")
        {
            int lengthSamples = samples.Length / channels;

            // AudioClip 생성
            var clip = AudioClip.Create(clipName, lengthSamples, channels, sampleRate, false);

            // 샘플 설정
            clip.SetData(samples, 0);

            return clip;
        }
    }
}