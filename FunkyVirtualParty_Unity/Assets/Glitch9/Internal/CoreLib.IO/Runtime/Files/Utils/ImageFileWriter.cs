using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class ImageFileWriter
    {
        public static async UniTask WriteFileAsync(Texture2D tex, string writeAbsolutePath)
        {
            if (tex == null)
            {
                throw new Exception("Texture2D is null.");
            }

            if (string.IsNullOrEmpty(writeAbsolutePath))
            {
                throw new Exception("Local file path is null or empty.");
            }

            byte[] bytes = tex.EncodeToPNG(); // or EncodeToJPG() for JPEG format

            string dirName = Path.GetDirectoryName(writeAbsolutePath);

            if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
            {
                Debug.Log($"Creating directory: {dirName}");
                Directory.CreateDirectory(dirName);
            }

            Debug.Log($"Writing image file to {writeAbsolutePath}");
            await File.WriteAllBytesAsync(writeAbsolutePath, bytes);
            FileUtil.RefreshIfEditor();
        }
    }
}