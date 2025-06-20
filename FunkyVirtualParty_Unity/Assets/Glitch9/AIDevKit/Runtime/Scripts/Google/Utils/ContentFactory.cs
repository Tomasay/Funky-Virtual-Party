using System;
using System.Collections.Generic;
using System.Linq;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
    internal class ContentFactory
    {
        internal static Content CreateSystemInstruction(string @params)
        {
            ContentPart[] parts = new[] { new ContentPart() { Text = @params } };
            return new Content(ChatRole.System, parts);
        }

        public static Content CreateUserContent(string prompt, IEnumerable<File<Texture2D>> imageFiles = null)
        {
            bool invalidPrompt = string.IsNullOrEmpty(prompt);

            List<ContentPart> parts = new();

            if (!invalidPrompt)
            {
                ContentPart textPart = new()
                {
                    Text = prompt
                };
                parts.Add(textPart);
            }

            if (imageFiles != null)
            {
                foreach (File<Texture2D> imageFile in imageFiles)
                {
                    ContentPart imagePart = new()
                    {
                        InlineData = new Blob()
                        {
                            MimeType = MIMETypeUtil.ParseFromPath(imageFile.Name),
                            Data = Convert.ToBase64String(imageFile.ReadAllBytes())
                        }
                    };
                    parts.Add(imagePart);
                }
            }

            return new Content()
            {
                Parts = parts.ToArray(),
                Role = ChatRole.User
            };
        }

        public static Content CreateUserContent(IEnumerable<ContentPart> parts)
        {
            return new Content()
            {
                Parts = parts.ToArray(),
                Role = ChatRole.User
            };
        }
    }
}
