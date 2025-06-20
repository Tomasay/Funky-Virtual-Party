using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_Google_Models
    {
        internal static readonly Dictionary<string, ModelPrice[]> Prices = new()
        {
            { "imagen-3", ModelPrice.PerImage(0.03) },
            { "veo-2.0-generate-001", ModelPrice.PerMinute(0.35 * 60) },
            { "gemini-2.5-flash-preview", new ModelPrice[] {
                new (UsageType.InputToken, 0.00000015),
                new (UsageType.OutputToken, 0.0000006),
                new (UsageType.ReasoningToken, 0.0000035),
            }},
            { "gemini-2.5-pro-preview", new ModelPrice[] {
                new (UsageType.InputToken, 0.0000025),
                new (UsageType.OutputToken, 0.000015),
                new (UsageType.CachedInputToken, 0.000000625),
            }},
            { "gemini-2.0-flash-lite", ModelPrice.PerIO( 0.000000075, 0.0000003) },
            { "gemini-2.0-flash", new ModelPrice[] {
                new (UsageType.InputToken, 0.0000001),
                new (UsageType.OutputToken, 0.0000004),
                new (UsageType.CachedInputToken, 0.000000025),
            }},
            { "gemini-1.5-flash-8b", new ModelPrice[] {
                new (UsageType.InputToken, 0.000000075),
                new (UsageType.OutputToken, 0.0000003),
                new (UsageType.CachedInputToken, 0.00000002),
            }},
            { "gemini-1.5-flash", new ModelPrice[] {
                new (UsageType.InputToken, 0.00000015),
                new (UsageType.OutputToken, 0.0000006),
                new (UsageType.CachedInputToken, 0.0000000375),
            }},
            { "gemini-1.5-pro", new ModelPrice[] {
                new (UsageType.InputToken, 0.0000025),
                new (UsageType.OutputToken, 0.00001),
                new (UsageType.CachedInputToken, 0.000000625),
            }},
            { "gemma-3", ModelPrice.Free() },
            { "embedding", ModelPrice.Free() },
        };

    }
}