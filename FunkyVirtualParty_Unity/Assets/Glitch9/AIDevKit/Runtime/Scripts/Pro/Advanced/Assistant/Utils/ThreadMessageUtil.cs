using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal static class ThreadMessageUtil
    {
        internal static ThreadMessageRequest CreateMessageRequest(string textPrompt)
        {
            textPrompt = textPrompt.Trim().RemoveLineBreaks();
            return new ThreadMessageRequest(textPrompt);
        }

        internal static ThreadMessageRequest CreateMessageRequest(string textPrompt, params File<Texture2D>[] imageFiles)
        {
            textPrompt = textPrompt.Trim().RemoveLineBreaks();
            return new ThreadMessageRequest(textPrompt, uploadFiles: imageFiles);
        }

        internal static ThreadMessageRequest CreateMessageRequest(string textPrompt, params string[] imageUrls)
        {
            textPrompt = textPrompt.Trim().RemoveLineBreaks();
            return new ThreadMessageRequest(textPrompt, imageUrls: imageUrls);
        }
    }
}

