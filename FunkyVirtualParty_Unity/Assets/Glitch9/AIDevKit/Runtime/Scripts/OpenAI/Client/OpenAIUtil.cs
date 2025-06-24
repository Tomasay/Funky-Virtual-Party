using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit.OpenAI
{
    internal static class OpenAIUtil
    {
        internal static IEnumerable<ChatCompletionChunk> BuildChatCompletionChunk(string sseString)
        {
            if (string.IsNullOrEmpty(sseString)) yield break;

            JsonSerializerSettings jsonSettings = OpenAI.DefaultInstance.JsonSettings;

            if (ErrorResponseDetector.IsErrorResponse(sseString, jsonSettings, out string errorMessage))
            {
                yield return ChatCompletionChunk.Error(errorMessage);
                yield break;
            }

            SSEParser parser = OpenAI.DefaultInstance.SSEParser;
            var data = parser.Parse(sseString);

            if (data.IsNullOrEmpty()) yield break;

            foreach (var (field, result) in data)
            {
                if (field == SSEField.Error)
                {
                    yield return ChatCompletionChunk.Error(result);
                    yield break;
                }

                if (field != SSEField.Data || string.IsNullOrEmpty(result))
                {
                    yield return null;
                }

                if (parser.IsDone(result))
                {
                    yield return ChatCompletionChunk.Done();
                    yield break;
                }

                ChatCompletion c = JsonConvert.DeserializeObject<ChatCompletion>(result, jsonSettings);
                yield return ChatCompletionChunk.Create(c);
            }
        }

        internal static IEnumerable<TranscriptChunk> BuildTranscriptChunk(string sseString)
        {
            if (string.IsNullOrEmpty(sseString)) yield break;

            JsonSerializerSettings jsonSettings = OpenAI.DefaultInstance.JsonSettings;

            if (ErrorResponseDetector.IsErrorResponse(sseString, jsonSettings, out string errorMessage))
            {
                yield return TranscriptChunk.Error(errorMessage);
                yield break;
            }

            SSEParser parser = OpenAI.DefaultInstance.SSEParser;
            var data = parser.Parse(sseString);

            if (data.IsNullOrEmpty()) yield break;

            foreach (var (field, result) in data)
            {
                if (field == SSEField.Error)
                {
                    yield return TranscriptChunk.Error(result);
                    yield break;
                }

                if (field != SSEField.Data || string.IsNullOrEmpty(result))
                {
                    yield return null;
                }

                // if (parser.IsDone(result))
                // {
                //     yield return ChatCompletionChunk.Done();
                //     yield break;
                // }

                TranscriptStreamEvent c = JsonConvert.DeserializeObject<TranscriptStreamEvent>(result, jsonSettings);
                yield return TranscriptChunk.Create(c);
            }
        }

        internal static JToken CreateThreadMessageContentJToken(List<ContentPartWrapper> partWrappers, JsonSerializer serializer)
        {
            AIDevKitDebug.Info("[OpenAIUtils] Creating ThreadMessage content JToken...");

            if (partWrappers.IsNullOrEmpty()) return null;

            var array = new JArray();

            foreach (var partWrapper in partWrappers)
            {
                if (partWrapper == null) continue;

                JToken token;
                // check if part is a string or object

                if (partWrapper.IsString)
                {
                    ContentPart textPart = new TextContentPart(partWrapper.ToString());
                    token = JToken.FromObject(textPart, serializer);
                }
                else
                {
                    var part = partWrapper.ToPart();
                    if (part == null)
                    {
                        AIDevKitDebug.Warning("[CreateThreadMessageContentJToken] Skipping part with null ContentPart.");
                        continue;
                    }
                    token = JToken.FromObject(part, serializer);
                }

                array.Add(token);
            }

            return array;
        }
    }
}