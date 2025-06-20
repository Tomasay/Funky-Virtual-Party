using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_OpenAI_Models
    {
        /* Data from the website

        model: performance, speed
        
        o4-mini: 4,3
        o3: 5,1
        o3-pro: 5,1
        o3-mini: 4,3
        o1: 4,1
        o1-mini: 3,2
        o1-pro: 4,1

        gpt-4.1: 4,3
        gpt-4o: 3,3
        gpt-4o audio: 3,3
        chatgpt-4o: 3,3

        o4-mini: 4,3
        gpt-4.1-mini: 3,4
        gpt-4.1-nano: 2,5
        o3-mini: 4,3
        gpt-4o-mini: 2,4
        gpt-4o-mini audio: 2,4
        o1-mini: 3,2

        gpt-4.0-realtime: 3,4
        gpt-4.0-mini-realtime: 2,5

        gpt-image-1: 4,1
        dalle-3: 3,2
        dalle-2: 1,2

        gpt-4o-mini-tts: 4,4
        tts-1: 2,4
        tts-1-hd: 3,3

        gpt-4o-transcribe: 4,3
        gpt-4o-mini-transcribe: 3,4
        whisper-1: 2,3

        gpt-4o-search-preview-2025-03-11: 3,3
        gpt-4o-mini-search-preview-2025-03-11: 2,4
        computer-use-preview-2025-03-11: 2,2
        codex-mini-latest: 4,3

        text-embedding-3-small: 2,3
        text-embedding-3-large: 3,2
        text-embedding-ada-002: 1,2

        omni-moderation-2024-09-26: 3,3
        text-moderation-007: 2,3

        gpt-4-turbo-2024-04-09: 2,3
        gpt-4-0613: 2,3
        gpt-3.5-turbo-0125: 1,2
        babbage-002: 1,3
        davinci-002: 1,3 

        */

        internal static readonly Dictionary<string, (int performance, int speed)> Performances = new()
        {
            { "o4-mini", (4, 3) },
            { "o3", (5, 1) },
            { "o3-pro", (5, 1) },
            { "o3-mini", (4, 3) },
            { "o1", (4, 1) },
            { "o1-mini", (3, 2) },
            { "o1-pro", (4, 1) },

            { "gpt-4.1", (4, 3) },
            { "gpt-4o-mini", (2, 4) },
            { "gpt-4.1-mini", (3, 4) },
            { "gpt-4.1-nano", (2, 5) },
            { "gpt-4o-mini-audio", (2, 4) },
            { "gpt-4o", (3, 3) },
            { "gpt-4o-audio", (3, 3) },
            { "chatgpt-4o", (3, 3) },

            { "gpt-4o-realtime", (3, 4) },
            { "gpt-4o-mini-realtime", (2, 5) },

            { "gpt-image-1", (4, 1) },
            { "dall-e-3", (3, 2) },
            { "dall-e-2", (1, 2) },

            { "gpt-4o-mini-tts", (4, 4) },
            { "tts-1", (2, 4) },
            { "tts-1-hd", (3, 3) },

            { "gpt-4o-transcribe", (4, 3) },
            { "gpt-4o-mini-transcribe", (3, 4) },
            { "whisper-1", (2, 3) },

            { "gpt-4o-search", (3, 3) },
            { "gpt-4o-mini-search", (2, 4) },
            { "computer-use", (2, 2) },
            { "codex-mini", (4, 3) },

            { "text-embedding-3-small", (2, 3) },
            { "text-embedding-3-large", (3, 2) },
            { "text-embedding-ada", (1, 2) },

            { "omni-moderation", (3, 3) },
            { "text-moderation", (2, 3) },

            { "gpt-4-turbo", (2, 3) },
            { "gpt-4", (2, 3) },
            { "gpt-3.5-turbo", (1, 2) },
            { "babbage-002", (1, 3) },
            { "davinci-002", (1, 3) },
        };

    }
}