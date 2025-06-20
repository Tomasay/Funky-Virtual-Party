using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_Google_Models
    {
        internal static readonly Dictionary<string, (int performance, int speed)> Performances = new()
        {
            { "models/aqa", (1, 3) },
            { "models/text-embedding-004", (2, 3) },
            { "models/embedding-001", (2, 3) },
            { "models/embedding-gecko-001", (2, 3) },
            { "models/gemini-embedding-exp", (4, 3) },
            { "models/text-bison-001", (2, 3) },
            { "models/chat-bison-001", (2, 2) },
            { "models/gemini-1.5-flash", (3, 5) },
            { "models/gemini-1.5-pro", (4, 4) },
            { "models/gemini-1.5-flash-8b", (3, 5) },
            { "models/gemini-pro-vision", (2, 2) },
            { "models/gemini-exp-1206", (2, 2) },
            { "models/learnlm-1.5-pro", (5, 4) },
            { "models/imagen-3.0", (4, 2) },
            { "models/gemini-2.0-flash-thinking", (4, 4) },
            { "models/gemini-2.0-flash", (4, 4) },
            { "models/gemini-2.0-flash-lite", (3, 5) },
            { "models/gemini-2.0-flash-live", (4, 3) },
            { "models/gemini-2.0-pro-exp", (5, 3) },
            { "models/learnlm-2.0-flash", (4, 4) },
            { "models/gemma-2", (1, 3) },
            { "models/gemma-3", (2, 4) },
            { "models/gemma-3n", (3, 3) },
            { "models/gemini-2.5-pro", (5, 2) },
            { "models/gemini-2.5-flash", (4, 3) },
            { "models/veo-2.0-generate", (4, 2) },
        };
    }
}