using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class AudioFileWriter
    {
        public static async UniTask WriteFileAsync(AudioClip clip, string writeAbsolutePath, AudioType audioType = AudioType.WAV)
        {
            if (clip == null)
            {
                throw new Exception("AudioClip is null.");
            }

            if (string.IsNullOrEmpty(writeAbsolutePath))
            {
                throw new Exception("Local file path is null or empty.");
            }

            byte[] bytes;

            if (audioType == AudioType.WAV)
            {
                bytes = WavUtil.FromAudioClip(clip);
            }
            else
            {
                bytes = clip.EncodeToWAV();
            }

            string dirName = Path.GetDirectoryName(writeAbsolutePath);

            if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
            {
                Debug.Log($"Creating directory: {dirName}");
                Directory.CreateDirectory(dirName);
            }

            Debug.Log($"Writing audio file to {writeAbsolutePath} with type {audioType}");
            await File.WriteAllBytesAsync(writeAbsolutePath, bytes);
            FileUtil.RefreshIfEditor();
        }


        public static async UniTask WriteFileAsync(byte[] audioBytes, string writeAbsolutePath)
        {
            if (audioBytes == null)
            {
                throw new Exception("Failed to write audio file. Audio bytes are null.");
            }

            if (string.IsNullOrWhiteSpace(writeAbsolutePath))
            {
                throw new Exception("Failed to write audio file. Local file path is null or empty.");
            }

            // check if filepath has a valid extension (e.g. .wav, .mp3...)
            string extension = Path.GetExtension(writeAbsolutePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new Exception("Failed to write audio file. Local file path does not have a valid extension.");
            }

            string dirName = Path.GetDirectoryName(writeAbsolutePath);

            if (!string.IsNullOrWhiteSpace(dirName) && !Directory.Exists(dirName))
            {
                Debug.Log($"Creating directory: {dirName}");
                Directory.CreateDirectory(dirName);
            }

            await File.WriteAllBytesAsync(writeAbsolutePath, audioBytes);
            Debug.Log($"Writing audio file to {writeAbsolutePath} with type {extension}");

            FileUtil.RefreshIfEditor();
        }

        public static async UniTask WriteFileIfValidPathAsync(byte[] audioBytes, string writeAbsolutePath)
        {
            if (string.IsNullOrWhiteSpace(writeAbsolutePath)) return;
            await WriteFileAsync(audioBytes, writeAbsolutePath);
        }

    }
}