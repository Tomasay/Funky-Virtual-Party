using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
    public class GeneratedImageBlob
    {
        [JsonProperty("mimeType")] public MIMEType MimeType { get; set; }
        [JsonProperty("bytesBase64Encoded")] public string Base64Image { get; set; }
    }

    public class PredictionResponse
    {
        [JsonProperty("predictions")] public List<GeneratedImageBlob> GeneratedImages { get; set; }
        [JsonIgnore] private GeneratedImage _generatedImage;

        public bool Validate<T>()
        {
            if (GeneratedImages == null || GeneratedImages.Count == 0)
                throw new System.Exception("No images generated.");
            return true;
        }

        public async UniTask<GeneratedImage> ToGeneratedImageAsync(string outputPath)
        {
            if (_generatedImage != null) return _generatedImage;
            if (GeneratedImages == null || GeneratedImages.Count == 0)
                throw new System.Exception("No images generated.");

            List<Texture2D> textures = new(GeneratedImages.Count);
            List<string> paths = new(GeneratedImages.Count);
            string savePath = outputPath.ToFullPath();

            for (int i = 0; i < GeneratedImages.Count; i++)
            {
                string base64 = GeneratedImages[i]?.Base64Image;
                Texture2D texture = ImageDecoder.Decode(base64);

                string finalPath = OutputPathResolver.AddIndexToPath(savePath, i);

                await texture.SaveTextureToFileAsync(finalPath);
                textures.Add(texture);
                paths.Add(finalPath);
            }

            _generatedImage = new GeneratedImage(textures.ToArray(), paths.ToArray());
            return _generatedImage;
        }
    }
}