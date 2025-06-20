using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;

namespace Glitch9.CoreLib.IO.Audio
{
    public static class AudioProcessor
    {
        /// <summary>
        /// Process an audio file and return the base64 encoded audio data (PCM16).
        /// </summary>
        public static async UniTask<string> ProcessPCM16Audio(string audioFilePath)
        {
            AudioClip audioClip = await AudioClipLoader.LoadFullPathOrUrlAsync(audioFilePath);

            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio file.");
                return null;
            }

            return audioClip.EncodeToBase64PCM16();
        }

        /// <summary>
        /// Process an audio file and return the base64 encoded audio data (G.711 μ-law).
        /// </summary>
        public static async UniTask<string> ProcessG711uLawAudio(string audioFilePath)
        {
            AudioClip audioClip = await AudioClipLoader.LoadFullPathOrUrlAsync(audioFilePath);

            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio file.");
                return null;
            }

            return audioClip.EncodeToBase64G711uLaw();
        }

        /// <summary>
        /// Process an audio file and return the base64 encoded audio data (G.711 A-law).
        /// </summary>
        public static async UniTask<string> ProcessG711aLawAudio(string audioFilePath)
        {
            AudioClip audioClip = await AudioClipLoader.LoadFullPathOrUrlAsync(audioFilePath);

            if (audioClip == null)
            {
                Debug.LogError("Failed to load audio file.");
                return null;
            }

            return audioClip.EncodeToBase64G711aLaw();
        }


        public static string FloatArrayToPCM16Base64(float[] floatArray)
        {
            byte[] pcm16Data = FloatTo16BitPCM(floatArray);
            return Convert.ToBase64String(pcm16Data);
        }

        public static string FloatArrayToG711uLawBase64(float[] floatArray)
        {
            byte[] g711uLawData = FloatToG711uLaw(floatArray);
            return Convert.ToBase64String(g711uLawData);
        }

        public static string FloatArrayToG711aLawBase64(float[] floatArray)
        {
            byte[] g711aLawData = FloatToG711aLaw(floatArray);
            return Convert.ToBase64String(g711aLawData);
        }

        public static byte[] FloatTo16BitPCM(float[] floatArray)
        {
            byte[] buffer = new byte[floatArray.Length * 2];
            for (int i = 0; i < floatArray.Length; i++)
            {
                short pcm = (short)(Mathf.Clamp(floatArray[i], -1f, 1f) * short.MaxValue);
                buffer[i * 2] = (byte)(pcm & 0xFF);             // Little endian: LSB
                buffer[i * 2 + 1] = (byte)((pcm >> 8) & 0xFF);  // MSB
            }
            return buffer;
        }

        // Converts a float array of audio data to G.711 μ-law byte array
        public static byte[] FloatToG711uLaw(float[] floatArray)
        {
            byte[] g711uLawData = new byte[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
            {
                short pcmValue = (short)(Mathf.Clamp(floatArray[i], -1f, 1f) * 32768);
                g711uLawData[i] = EncodeG711uLaw(pcmValue);
            }
            return g711uLawData;
        }

        // Converts a float array of audio data to G.711 A-law byte array
        public static byte[] FloatToG711aLaw(float[] floatArray)
        {
            byte[] g711aLawData = new byte[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
            {
                short pcmValue = (short)(Mathf.Clamp(floatArray[i], -1f, 1f) * 32768);
                g711aLawData[i] = EncodeG711aLaw(pcmValue);
            }
            return g711aLawData;
        }

        // Encode a single PCM16 sample to G.711 μ-law
        internal static byte EncodeG711uLaw(short pcm16)
        {
            // Implementing G.711 μ-law encoding algorithm
            const int BIAS = 0x84;
            const int MAX = 0x7FFF;

            int sign = (pcm16 >> 8) & 0x80; // Extract sign
            if (sign != 0)
                pcm16 = (short)-pcm16; // Get magnitude

            pcm16 += BIAS;
            if (pcm16 > MAX) pcm16 = MAX;

            int exponent = 7;
            int mantissa;
            for (int expMask = 0x4000; (pcm16 & expMask) == 0 && exponent > 0; exponent--, expMask >>= 1) { }

            mantissa = (pcm16 >> (exponent + 3)) & 0x0F;
            byte g711uLawByte = (byte)(sign | (exponent << 4) | mantissa);
            return (byte)~g711uLawByte;
        }

        // Encode a single PCM16 sample to G.711 A-law
        internal static byte EncodeG711aLaw(short pcm16)
        {
            // Implementing G.711 A-law encoding algorithm
            const int MAX = 0x7FFF;

            int sign = (pcm16 >> 8) & 0x80; // Extract sign
            if (sign != 0)
                pcm16 = (short)-pcm16; // Get magnitude

            if (pcm16 > MAX) pcm16 = MAX;

            int exponent = 7;
            int mantissa;
            for (int expMask = 0x1000; (pcm16 & expMask) == 0 && exponent > 0; exponent--, expMask >>= 1) { }

            mantissa = (pcm16 >> ((exponent == 0) ? 4 : (exponent + 3))) & 0x0F;
            byte g711aLawByte = (byte)((exponent << 4) | mantissa);
            return (byte)(sign | g711aLawByte ^ 0x55); // A-law is biased differently
        }

        // Base64로 인코딩된 PCM16 데이터를 AudioClip으로 변환
        public static AudioClip PCM16Base64ToAudioClip(string base64EncodedString, int sampleRate, int channels)
        {
            byte[] pcm16Data = Convert.FromBase64String(base64EncodedString);
            float[] audioData = PCM16ToFloatArray(pcm16Data);

            return AudioClipUtil.CreateAudioClipFromFloatArray(audioData, sampleRate, channels);
        }

        // Base64로 인코딩된 G711uLaw 데이터를 AudioClip으로 변환
        public static AudioClip G711uLawBase64ToAudioClip(string base64EncodedString, int sampleRate, int channels)
        {
            byte[] g711uLawData = Convert.FromBase64String(base64EncodedString);
            float[] audioData = G711uLawToFloatArray(g711uLawData);

            return AudioClipUtil.CreateAudioClipFromFloatArray(audioData, sampleRate, channels);
        }

        // Base64로 인코딩된 G711aLaw 데이터를 AudioClip으로 변환
        public static AudioClip G711aLawBase64ToAudioClip(string base64EncodedString, int sampleRate, int channels)
        {
            byte[] g711aLawData = Convert.FromBase64String(base64EncodedString);
            float[] audioData = G711aLawToFloatArray(g711aLawData);

            return AudioClipUtil.CreateAudioClipFromFloatArray(audioData, sampleRate, channels);
        }

        public static float[] PCM16ToFloatArray(string base64EncodedString)
        {
            byte[] pcm16Data = Convert.FromBase64String(base64EncodedString);
            return PCM16ToFloatArray(pcm16Data);
        }

        public static float[] G711uLawToFloatArray(string base64EncodedString)
        {
            byte[] g711uLawData = Convert.FromBase64String(base64EncodedString);
            return G711uLawToFloatArray(g711uLawData);
        }

        public static float[] G711aLawToFloatArray(string base64EncodedString)
        {
            byte[] g711aLawData = Convert.FromBase64String(base64EncodedString);
            return G711aLawToFloatArray(g711aLawData);
        }

        // PCM16 byte array를 float array로 변환
        public static float[] PCM16ToFloatArray(byte[] pcm16Bytes)
        {
            int sampleCount = pcm16Bytes.Length / 2;
            float[] floatArray = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(pcm16Bytes, i * 2);
                floatArray[i] = sample / 32768f; // PCM16을 float(-1 ~ 1)로 변환
            }

            return floatArray;
        }

        // G.711 μ-law byte array를 float array로 변환
        public static float[] G711uLawToFloatArray(byte[] g711uLawBytes)
        {
            float[] floatArray = new float[g711uLawBytes.Length];

            for (int i = 0; i < g711uLawBytes.Length; i++)
            {
                floatArray[i] = DecodeG711uLaw(g711uLawBytes[i]) / 32768f;
            }

            return floatArray;
        }

        // G.711 A-law byte array를 float array로 변환
        public static float[] G711aLawToFloatArray(byte[] g711aLawBytes)
        {
            float[] floatArray = new float[g711aLawBytes.Length];

            for (int i = 0; i < g711aLawBytes.Length; i++)
            {
                floatArray[i] = DecodeG711aLaw(g711aLawBytes[i]) / 32768f;
            }

            return floatArray;
        }

        // G.711 μ-law 디코딩
        private static short DecodeG711uLaw(byte ulawByte)
        {
            ulawByte = (byte)~ulawByte;
            int sign = ulawByte & 0x80;
            int exponent = (ulawByte >> 4) & 0x07;
            int mantissa = ulawByte & 0x0F;
            int sample = ((mantissa << 3) | 0x84) << (exponent + 2);
            return (short)(sign != 0 ? -sample : sample);
        }

        // G.711 A-law 디코딩
        private static short DecodeG711aLaw(byte alawByte)
        {
            alawByte ^= 0x55;
            int sign = alawByte & 0x80;
            int exponent = (alawByte & 0x70) >> 4;
            int mantissa = alawByte & 0x0F;

            int sample = (mantissa << 4) + 8;
            if (exponent != 0)
                sample += 0x100;
            if (exponent > 1)
                sample <<= exponent - 1;

            return (short)(sign == 0 ? sample : -sample);
        }
    }
}
