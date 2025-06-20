using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    internal static class InternalExtensions
    {
        internal static List<File<Texture2D>> ToFiles(this Image[] images, string path)
        {
            if (images.IsNullOrEmpty()) return null;

            List<File<Texture2D>> files = new();

            for (int i = 0; i < images.Length; i++)
            {
                string localPath = path.ToFullPath();
                string filePath = OutputPathResolver.AddIndexToPath(localPath, i);

                files.Add(new File<Texture2D>(filePath));
            }

            return files;
        }

        internal static async UniTask<GeneratedImage> ToGeneratedImageAsync(this Image[] images, string path, ImageFormat? format)
        {
            if (images.IsNullOrEmpty()) return null;

            List<Texture2D> textures = new();
            List<string> paths = new();

            for (int i = 0; i < images.Length; i++)
            {
                var image = images[i];
                (Texture2D texture, string finalPath)? pair = await image.ConvertToTextureAndSaveAsync(path, format, i);
                if (pair != null)
                {
                    textures.Add(pair.Value.texture);
                    paths.Add(pair.Value.finalPath);
                }
            }

            return new GeneratedImage(textures.ToArray(), paths.ToArray());
        }


        private static async UniTask<(Texture2D, string)?> ConvertToTextureAndSaveAsync(this Image image, string path, ImageFormat? format, int index)
        {
            string absolutePath = path.ToFullPath();
            string savePath = OutputPathResolver.AddIndexToPath(absolutePath, index);

            // log file path. it's not saving the file. need to debug
            AIDevKitDebug.Info($"File Save Path: {savePath}");
            // format ??= image.ResolveFileFormat();

            format ??= ImageFormat.Url; // Default to Base64Json if format is not specified (https://platform.openai.com/docs/api-reference/images/create)

            if (format == ImageFormat.Base64Json)
            {
                string base64Json = image.B64Json;
                ThrowIf.IsNullOrWhitespace(base64Json, nameof(image.B64Json));
                Texture2D texture = ImageDecoder.Decode(image.B64Json);
                await texture.SaveTextureToFileAsync(savePath);
                return (texture, savePath);
            }

            if (format == ImageFormat.Url)
            {
                string url = image.Url;
                if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(image.Url), "Url is null or empty.");

                //return await UnityDownloader.DownloadTextureAsync(url, savePath);
                Texture2D texture = await UnityDownloader.DownloadTextureAsync(url, savePath);
                if (texture == null) throw new Exception($"Failed to download texture from URL: {url}");
                return (texture, savePath);
            }

            return null;
        }
    }
}