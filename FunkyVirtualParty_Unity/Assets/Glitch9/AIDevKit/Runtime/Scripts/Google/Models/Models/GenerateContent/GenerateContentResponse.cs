using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Individual response from {@link GenerativeModel.generateContent} and
    /// {@link GenerativeModel.generateContentStream}.
    /// `generateContentStream()` will return one in each chunk until
    /// the stream is done.
    /// </summary>
    /// <remarks>
    /// This is Google's equivalent of the ChatCompletion object in OpenAI.
    /// </remarks>
    public class GenerateContentResponse
    {
        [JsonProperty("candidates")] public Candidate[] Candidates { get; set; }
        [JsonProperty("promptFeedback")] public PromptFeedback PromptFeedback { get; set; }
        [JsonProperty("usageMetadata")] public Usage Usage { get; set; }

        public override string ToString() => this.GetOutputText();
        public string FirstTextDelta() => this.GetOutputText();

        // Caching
        private List<ContentPart> _parts;
        private List<string> _inlineDataList;

        // Getters
        public List<ContentPart> ToParts()
        {
            if (_parts != null) return _parts;

            _parts = new List<ContentPart>();
            if (Candidates == null || Candidates.Length == 0) return _parts;

            foreach (Candidate candidate in Candidates)
            {
                if (candidate.Content == null
                    || candidate.Content.Parts.IsNullOrEmpty()) continue;

                foreach (ContentPart part in candidate.Content.Parts)
                {
                    _parts.Add(part);
                }
            }
            return _parts;
        }

        public List<string> ToInlineDataList()
        {
            if (_inlineDataList != null) return _inlineDataList;

            _inlineDataList = new List<string>();
            List<ContentPart> parts = ToParts();
            if (parts.IsNullOrEmpty()) return _inlineDataList;

            foreach (ContentPart part in parts)
            {
                string inlineData = part?.InlineData?.Data;
                if (string.IsNullOrEmpty(inlineData)) continue;

                _inlineDataList.Add(inlineData);
            }
            return _inlineDataList;
        }

        public async UniTask<GeneratedImage> ToGeneratedImageAsync(string outputPath)
        {
            List<ContentPart> parts = ToParts();
            if (parts.IsNullOrEmpty()) throw new System.Exception("No images generated.");

            var textures = new List<Texture2D>();
            var paths = new List<string>();
            string savePath = outputPath.ToFullPath();

            for (int i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                if (part == null || part.InlineData == null) continue;
                var texture = ImageDecoder.Decode(part.InlineData.Data);
                string finalPath = OutputPathResolver.AddIndexToPath(savePath, i);
                await texture.SaveTextureToFileAsync(finalPath);
                textures.Add(texture);
                paths.Add(finalPath);
            }

            return new GeneratedImage(textures.ToArray(), paths.ToArray());
        }

        public AIDevKit.Content FirstContent()
        {
            if (Candidates == null || Candidates.Length == 0) return null;

            foreach (Candidate candidate in Candidates)
            {
                if (candidate.Content == null) continue;
                AIDevKit.Content content = candidate.Content.Convert();
                if (content != null) return content;
            }

            return null;
        }

        public ChatChoice[] GetChatChoices()
        {
            if (Candidates == null || Candidates.Length == 0) return null;

            List<ChatChoice> chatChoices = new();

            foreach (Candidate candidate in Candidates)
            {
                if (candidate.Content == null) continue;
                ChatChoice choice = candidate.Content.ToChatChoice();
                if (choice == null) continue;
                chatChoices.Add(choice);
            }

            return chatChoices.ToArray();
        }

        public ToolCall[] GetToolCalls()
        {
            List<ContentPart> parts = ToParts();
            if (parts.IsNullOrEmpty()) return null;

            List<ToolCall> toolCalls = new();

            foreach (ContentPart part in parts)
            {
                if (part.FunctionCall != null)
                {
                    toolCalls.Add(part.FunctionCall);
                }
            }

            return toolCalls.ToArray();
        }
    }
}