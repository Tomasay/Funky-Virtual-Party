using System.Collections.Generic;
using Glitch9.IO.Files;

namespace Glitch9.AIDevKit.Google
{
    internal static class GenerativeAIConverter
    {
        internal static ChatChoice ToChatChoice(this Content content)
        {
            if (content == null || content.Parts.IsNullOrEmpty()) return null;

            List<AIDevKit.ContentPart> parts = new();
            List<ToolCall> tools = new();

            foreach (ContentPart part in content.Parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    parts.Add(new TextContentPart(part.Text));
                }
                else if (part.FileData?.Uri != null)
                {
                    parts.Add(ImageContentPart.FromUrl(part.FileData.Uri));
                }
                else if (part.InlineData?.Data != null)
                {
                    var mime = part.InlineData.MimeType;

                    if (mime.IsImage())
                    {
                        parts.Add(ImageContentPart.FromBase64(part.InlineData.Data));
                    }
                    else if (mime.IsAudio())
                    {
                        parts.Add(AudioContentPart.FromBase64(part.InlineData.Data, mime));
                    }
                    else
                    {
                        parts.Add(FileContentPart.FromBase64(part.InlineData.Data, mime));
                    }
                }

                if (part.FunctionCall != null)
                {
                    tools.Add(part.FunctionCall);
                }
            }

            AIDevKit.Content convertedContent = AIDevKit.Content.FromParts(parts);

            return new ChatChoice
            {
                Message = new ResponseMessage
                {
                    Content = convertedContent,
                    Tools = tools.ToArray(),
                },
            };
        }

        internal static AIDevKit.Content Convert(this Content content)
        {
            if (content == null || content.Parts.IsNullOrEmpty()) return null;

            // 멀티파트 구성
            List<AIDevKit.ContentPart> parts = new();

            foreach (ContentPart part in content.Parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    parts.Add(new TextContentPart(part.Text));
                }
                else if (part.FileData?.Uri != null)
                {
                    parts.Add(ImageContentPart.FromUrl(part.FileData.Uri));
                }
                else if (part.InlineData?.Data != null)
                {
                    var mime = part.InlineData.MimeType;

                    if (mime.IsImage())
                    {
                        parts.Add(ImageContentPart.FromBase64(part.InlineData.Data));
                    }
                    else if (mime.IsAudio())
                    {
                        parts.Add(AudioContentPart.FromBase64(part.InlineData.Data, mime));
                    }
                    else
                    {
                        parts.Add(FileContentPart.FromBase64(part.InlineData.Data, mime));
                    }
                }
            }

            return AIDevKit.Content.FromParts(parts);
        }
    }
}