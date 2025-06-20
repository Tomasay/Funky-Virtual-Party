using System;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    internal static class TextResponseConverter
    {
        internal static async UniTask<RESTResponse<TResBody>> ConvertAsync<TResBody>(
            RESTRequest request,
            RESTResponse<TResBody> response,
            string textResult,
            string contentType,
            RESTClient client)
        {
            if (!request.IgnoreLogs)
            {
                client.Logger.ReqVerbose("Download Mode: Text");
                client.Logger.ResBody(textResult);
            }

            response.TextOutput = textResult;
            MIMEType mimeType = MIMETypeUtil.ParseApiValue(contentType);
            string outputPath = request.OutputPath?.ToFullPath();

            if (mimeType.IsAudio())
            {
                File<AudioClip> file = await AudioClipDecoder.DecodeAsync(textResult, request.OutputAudioFormat, outputPath, mimeType);
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
            else
            {
                switch (mimeType)
                {
                    case MIMEType.Xml or MIMEType.CSV or MIMEType.HTML or MIMEType.MultipartForm:
                        client.Logger.Error($"{mimeType} is not supported. Result object will not be created.");
                        return response;

                    case MIMEType.Json or MIMEType.WWWForm:
                    default:  // fallback (e.g. JSON or unknown) 
                        TResBody body = DeserializeResponseBody<TResBody>(textResult, client);
                        response.Body = body;
                        return response;
                }
            }

            return response;
        }

        private static TResBody DeserializeResponseBody<TResBody>(string jsonString, RESTClient client)
        {
            if (client.JsonSettings == null)
            {
                client.Logger.Error("Deserialize failed. JSON settings are null.");
                return default;
            }

            if (string.IsNullOrEmpty(jsonString))
            {
                client.Logger.Error("JSON string is null or empty.");
                return default;
            }

            Type type = typeof(TResBody);
            string typeName;

            if (type.IsGenericType)
            {
                string genericName = type.GetGenericArguments()[0].Name;
                typeName = $"{type.Name.Split('`')[0]}<{genericName}>";
            }
            else
            {
                typeName = type.Name;
            }

            try
            {
                // return JsonConvert.DeserializeObject<TResponse>(jsonString, client.JsonSettings);
                return (TResBody)JsonConvert.DeserializeObject(jsonString, typeof(TResBody), client.JsonSettings);
            }
            catch (JsonReaderException e)
            {
                client.Logger.Error(
                    $"Failed to deserialize <color=yellow>{typeName}</color> at " +
                    $"line {e.LineNumber}, position {e.LinePosition}.\n" +
                    $"Message: {e.Message}");
                return default;
            }
        }
    }
}