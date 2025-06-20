using System.IO;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class AudioClipUtil
    {
        // float array 데이터를 이용해 AudioClip 생성
        internal static AudioClip CreateAudioClipFromFloatArray(float[] audioData, int sampleRate, int channels, string name = "decoded_audio_clip")
        {
            AudioClip audioClip = AudioClip.Create(name, audioData.Length, channels, sampleRate, false);
            audioClip.SetData(audioData, 0);
            return audioClip;
        }

        internal static string ParseAudioClipName(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath)) return "temp_audioClip";
            string fileName = Path.GetFileNameWithoutExtension(outputPath);
            if (string.IsNullOrEmpty(fileName)) return "temp_audioClip";
            return fileName;
        }
    }
}