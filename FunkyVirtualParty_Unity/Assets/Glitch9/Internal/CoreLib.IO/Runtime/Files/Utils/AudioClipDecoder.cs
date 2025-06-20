using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public static class AudioClipDecoder
    {
        // outputPath가 존재한다면, 파일을 저장하고 싶다는 의미임으로 temp폴더가 아니라 지정된 outputPath에 저장하도록 한다.
        public static async UniTask<File<AudioClip>> DecodeAsync(byte[] binaryData, AudioFormat format, string outputPath, MIMEType mimeType)
        {
            return mimeType switch
            {
                MIMEType.WAV => await DecodeWavAsync(binaryData, outputPath),
                MIMEType.PCM => await DecodePcmAsync(binaryData, format, outputPath),
                MIMEType.aLaw => await DecodeG711aLawAsync(binaryData, format, outputPath),
                MIMEType.uLaw => await DecodeG711uLawAsync(binaryData, format, outputPath),
                MIMEType.MPEG => await DecodeMpegAsync(binaryData, format, outputPath),
                _ => throw new NotSupportedException($"Unsupported MIME type: {mimeType}"),
            };
        }

        public static async UniTask<File<AudioClip>> DecodeAsync(string base64Encoded, AudioFormat format, string outputPath, MIMEType mimeType)
        => await DecodeAsync(Convert.FromBase64String(base64Encoded), format, outputPath, mimeType);

        private static async UniTask<File<AudioClip>> DecodeWavAsync(byte[] binaryData, string outputPath)
        {
            var name = AudioClipUtil.ParseAudioClipName(outputPath);
            var pcmData = PcmData.FromBytes(binaryData);

            var clip = AudioClip.Create(name, pcmData.Length, pcmData.Channels, pcmData.SampleRate, false);
            clip.SetData(pcmData.Value, 0);
            await AudioFileWriter.WriteFileIfValidPathAsync(binaryData, outputPath);
            return new(clip, outputPath);
        }

        private static async UniTask<File<AudioClip>> DecodePcmAsync(byte[] binaryData, AudioFormat format, string outputPath)
        {
            binaryData = WavUtil.EnsureFileHeader(binaryData, format?.SampleRate ?? SampleRate.Hz44100);
            return await AudioClipLoader.NonWebGLDecodeAsyncINTERNAL(binaryData, outputPath, AudioType.WAV);
        }

        private static async UniTask<File<AudioClip>> DecodeG711aLawAsync(byte[] binaryData, AudioFormat format, string outputPath)
        {
            float[] samples = AudioProcessor.G711aLawToFloatArray(binaryData);
            byte[] pcm = AudioProcessor.FloatTo16BitPCM(samples);
            return await DecodePcmAsync(pcm, format, outputPath);
        }

        private static async UniTask<File<AudioClip>> DecodeG711uLawAsync(byte[] binaryData, AudioFormat format, string outputPath)
        {
            float[] samples = AudioProcessor.G711uLawToFloatArray(binaryData);
            byte[] pcm = AudioProcessor.FloatTo16BitPCM(samples);
            return await DecodePcmAsync(pcm, format, outputPath);
        }

        private static async UniTask<File<AudioClip>> DecodeMpegAsync(byte[] binaryData, AudioFormat format, string outputPath)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return await WebGLAudioProcessor.DecodeAsync(binaryData);
#else
            return await AudioClipLoader.NonWebGLDecodeAsyncINTERNAL(binaryData, outputPath, AudioType.MPEG);
#endif
        }
    }
}
