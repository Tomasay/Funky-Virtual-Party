using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    internal static class BinaryResponseConverter
    {
        internal static async UniTask<RESTResponse> ConvertAsync(
            RESTRequest request,
            RESTResponse response,
            byte[] result,
            string contentType,
            RESTClient client)
        {
            if (!request.IgnoreLogs)
            {
                client.Logger.ReqVerbose("Download Mode: Binary");
            }

            response.BinaryOutput = result;
            MIMEType mimeType = MIMETypeUtil.ParseApiValue(contentType);
            string outputPath = request.OutputPath?.ToFullPath();

            // Image 
            if (mimeType == MIMEType.GIF)
            {
                client.Logger.Error("GIF is not supported. Texture2D will not be created.");
            }
            else if (mimeType.IsImage())
            {
                response.ImageOutput = ImageDecoder.Decode(result);
            }
            // Audio
            else if (mimeType.IsAudio())
            {
                File<AudioClip> file = await AudioClipDecoder.DecodeAsync(result, request.OutputAudioFormat, outputPath, mimeType);
                if (file == null)
                {
                    client.Logger.Error($"Failed to decode {mimeType} audio from the HTTP response.");
                }
                else
                {
                    response.AudioOutput = file.Asset;
                    response.OutputPath = file.FullPath;
                }
            }
            // Unknown
            else
            {
                client.Logger.Warning($"Unsupported MIME type: {mimeType}. Attempting to save as binary file.");
                response.FileOutput = await FileUtil.CreateRawFileAsync(result, outputPath);
            }

            await UniTask.Delay(1000);
            return response;
        }
    }
}