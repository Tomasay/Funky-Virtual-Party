using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_OpenAI_Models
    {
        internal static readonly Dictionary<string, ModelPrice[]> Prices = new()
        { 
            // legacy models
            { "babbage-002", ModelPrice.PerIO(0.0000004, 0.0000004) },
            { "davinci-002", ModelPrice.PerIO(0.000002, 0.000002) },

            // not legacy, but old models
            { "gpt-3.5-turbo-1106", ModelPrice.PerIO(0.000001, 0.000002) },
            { "gpt-3.5-turbo-0613", ModelPrice.PerIO(0.0000015, 0.000002) },
            { "gpt-3.5-0301", ModelPrice.PerIO(0.0000015, 0.000002) },
            { "gpt-3.5-turbo-instruct", ModelPrice.PerIO(0.0000015, 0.000002) },
            { "gpt-3.5-turbo-16k", ModelPrice.PerIO(0.000003, 0.000004) },
            { "gpt-3.5-turbo", ModelPrice.PerIO(0.0000005, 0.0000015) },

            { "gpt-4-0125-preview", ModelPrice.PerIO(0.00001, 0.00003) },
            { "gpt-4-1106-preview", ModelPrice.PerIO(0.00001, 0.00003) },
            { "gpt-4-1106-vision-preview", ModelPrice.PerIO(0.00001, 0.00003) },
            { "gpt-4-turbo", ModelPrice.PerIO(0.00001, 0.00003) },

            { "chatgpt-4o-latest", ModelPrice.PerIO(0.000005, 0.000015) }, 

            // new models      
            { "gpt-4.5-preview", ModelPrice.PerICO(0.000075, 0.0000375, 0.00015) },

            { "gpt-4.1-nano", ModelPrice.PerICO(0.0000001, 0.000000025, 0.0000004) },
            { "gpt-4.1-mini", new ModelPrice[] {
                new (UsageType.InputToken, 0.0000004),
                new (UsageType.CachedInputToken, 0.0000001),
                new (UsageType.OutputToken, 0.0000016),
                new (UsageType.WebSearchLow, 0.025),
                new (UsageType.WebSearch, 0.0275),
                new (UsageType.WebSearchHigh, 0.03),
            }},

            { "gpt-4.1", new ModelPrice[] {
                new (UsageType.InputToken, 0.000002),
                new (UsageType.CachedInputToken, 0.0000005),
                new (UsageType.OutputToken, 0.000008),
                new (UsageType.WebSearchLow, 0.03),
                new (UsageType.WebSearch, 0.035),
                new (UsageType.WebSearchHigh, 0.05),
            }},

            { "o4-mini", ModelPrice.PerICO(0.0000011, 0.000000275, 0.0000044) },

            { "o3-mini", ModelPrice.PerICO(0.0000011, 0.00000055, 0.0000044) },
            { "o3", ModelPrice.PerICO(0.00001, 0.0000025, 0.00004) },

            { "o1-pro", ModelPrice.PerIO(0.00015, 0.0006) },
            { "o1-mini", ModelPrice.PerICO(0.0000011, 0.00000055, 0.0000044) },
            { "o1", ModelPrice.PerICO(0.000015, 0.0000075, 0.00006) },

            { "computer-use-preview", ModelPrice.PerIO(0.000003, 0.000012) }, 

            // image models
            { "dall-e-2", new ModelPrice[] {
                new (UsageType.ImageSD256, 0.016),
                new (UsageType.ImageSD512, 0.018),
                new (UsageType.ImageSD1024, 0.02),
            }},
            { "dall-e-3", new ModelPrice[] {
                new (UsageType.ImageSD1024, 0.04),
                new (UsageType.ImageSD1792, 0.08),
                new (UsageType.ImageHD1024, 0.08),
                new (UsageType.ImageHD1792, 0.12),
            }},
            { "gpt-image-1", new ModelPrice[] {
                new (UsageType.ImageLow1024, 0.011),
                new (UsageType.ImageLow1536, 0.016),
                new (UsageType.ImageMedium1024, 0.042),
                new (UsageType.ImageMedium1536, 0.063),
                new (UsageType.ImageHigh1024, 0.167),
                new (UsageType.ImageHigh1536, 0.25),
            }},
                
            // embedding models
            { "text-embedding-3-small", ModelPrice.PerInputToken(0.00000002) },
            { "text-embedding-3-large", ModelPrice.PerInputToken(0.00000013) },
            { "text-embedding-ada-002", ModelPrice.PerInputToken(0.0000001) },

            // moderation models
            { "omni-moderation", ModelPrice.Free() },
            { "text-moderation", ModelPrice.Free() },

            // tts models 
            { "tts-1", ModelPrice.PerCharacter(0.000015) },
            { "tts-1-hd", ModelPrice.PerCharacter(0.00003) },  

            // stt models 
            { "whisper-1", ModelPrice.PerMinute(0.006) },

            { "gpt-4o-mini-audio-preview", ModelPrice.PerIO(0.00000015, 0.0000006) },
            { "gpt-4o-mini-realtime-preview", ModelPrice.PerICO(0.0000006, 0.0000003, 0.0000024) },
            { "gpt-4o-mini-transcribe", ModelPrice.PerMinute(0.003, true) },
            { "gpt-4o-mini-tts", ModelPrice.PerMinute(0.015, true) },
            //{ "gpt-4o-mini-search-preview", ModelPrice.PerInputOutput(0.00000015, 0.0000006) },
            //{ "gpt-4o-mini", ModelPrice.PerInputCachedInputOutput(0.00000015, 0.000000075, 0.0000006) },
            { "gpt-4o-mini-search-preview", new ModelPrice[] {
                new (UsageType.InputToken, 0.00000015),
                new (UsageType.OutputToken, 0.0000006),
                new (UsageType.WebSearchLow, 0.025),
                new (UsageType.WebSearch, 0.0275),
                new (UsageType.WebSearchHigh, 0.03),
            }},
            { "gpt-4o-mini", new ModelPrice[] {
                new (UsageType.InputToken, 0.00000015),
                new (UsageType.CachedInputToken, 0.000000075),
                new (UsageType.OutputToken, 0.0000006),
                new (UsageType.WebSearchLow, 0.025),
                new (UsageType.WebSearch, 0.0275),
                new (UsageType.WebSearchHigh, 0.03),
            }},

            { "gpt-4o-audio-preview", ModelPrice.PerIO(0.0000025, 0.00001) },
            { "gpt-4o-realtime-preview", ModelPrice.PerICO(0.000005, 0.0000025, 0.00002) },
            { "gpt-4o-transcribe", ModelPrice.PerMinute(0.006, true) },
            { "gpt-4o-search-preview", new ModelPrice[] {
                new (UsageType.InputToken, 0.0000025),
                new (UsageType.OutputToken, 0.00001),
                new (UsageType.WebSearchLow, 0.03),
                new (UsageType.WebSearch, 0.035),
                new (UsageType.WebSearchHigh, 0.05),
            }},
            { "gpt-4o", new ModelPrice[] {
                new (UsageType.InputToken, 0.0000025),
                new (UsageType.CachedInputToken, 0.00000125),
                new (UsageType.OutputToken, 0.00001),
                new (UsageType.WebSearchLow, 0.03),
                new (UsageType.WebSearch, 0.035),
                new (UsageType.WebSearchHigh, 0.05),
            }},

            { "gpt-4o-mini-audio", ModelPrice.PerICO(0.00000015, 0.000000075, 0.0000006) },

            { "gpt-4-32k", ModelPrice.PerIO(0.00006, 0.00012) },
            { "gpt-4", ModelPrice.PerIO(0.00003, 0.00006) },

            { "codex-mini-latest", ModelPrice.PerICO(0.0000015, 0.000000375, 0.000006) },   // added new on 2025-06-15  
        };

    }
}