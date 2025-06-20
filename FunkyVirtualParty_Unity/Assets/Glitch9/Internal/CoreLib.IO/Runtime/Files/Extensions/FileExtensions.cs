using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class FileExtensions
    {
        public static List<TFile> GetList<TFile>(this List<IFile> files) where TFile : IFile
        {
            if (files.IsNullOrEmpty()) return new();
            List<TFile> filteredFiles = new();
            foreach (var file in files)
            {
                if (file is TFile tFile)
                {
                    filteredFiles.Add(tFile);
                }
            }
            return filteredFiles;
        }

        public static async UniTask SaveTextureToFileAsync(this Texture2D texture, string path, CancellationToken cancellationToken = default)
        {
            byte[] textureAsBytes = texture.EncodeToPNG();
            await SaveTextureToFileAsync(textureAsBytes, path, cancellationToken);
        }

        public static async UniTask SaveTextureToFileAsync(this byte[] textureAsBytes, string path, CancellationToken cancellationToken = default)
        {
            if (textureAsBytes == null || textureAsBytes.Length == 0)
            {
                Debug.LogError("Texture bytes are null or empty.");
                return;
            }

            // get directory 
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                SafeCreateDirectory(directory);
            }

            if (path == null)
            {
                Debug.LogError("Path is null.");
                return;
            }

            try
            {
                await System.IO.File.WriteAllBytesAsync(path, textureAsBytes, cancellationToken);
                Debug.Log($"Texture saved to {path}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save texture to {path}: {ex.Message}");
            }
        }

        private static void SafeCreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (path.StartsWith("Assets/")) path = path[7..];

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}