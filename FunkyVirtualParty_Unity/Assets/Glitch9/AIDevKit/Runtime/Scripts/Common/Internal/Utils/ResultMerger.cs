using System;
using System.Collections.Generic;
using System.Linq;

namespace Glitch9.AIDevKit
{
    internal static class ResultMerger
    {
        internal static TResult Merge<TResult>(List<TResult> results)
        {
            if (results == null || results.Count == 0) return default;
            if (results.Count == 1) return results[0];

            AIDevKitDebug.Mark(15);

            // --- ChatCompletion ---
            if (typeof(TResult) == typeof(ChatCompletion))
            {
                List<ChatChoice> mergedChoices = new(); //ChatCompletion.Merge(results.Cast<ChatCompletion>().ToList());
                Usage mergedUsage = UsageFactory.Empty();

                foreach (ChatCompletion result in results.Cast<ChatCompletion>())
                {
                    if (result == null || result.IsEmpty) continue;

                    mergedChoices.AddRange(result.Choices);
                    mergedUsage = mergedUsage.Merge(result.Usage);
                }

                return (TResult)(object)ChatCompletionFactory.Create(mergedChoices.ToArray(), mergedUsage);
            }

            // --- GeneratedText ---
            if (typeof(TResult) == typeof(GeneratedText))
            {
                GeneratedText merged = new(
                    results.Cast<GeneratedText>().SelectMany(r => r.Values).ToArray(),
                    results.Cast<GeneratedText>().Select(r => r.Usage).Aggregate((a, b) => a.Merge(b))
                );
                return (TResult)(object)merged;
            }

            // --- GeneratedImage ---
            if (typeof(TResult) == typeof(GeneratedImage))
            {
                AIDevKitDebug.Mark(1333);
                GeneratedImage merged = new(
                    results.Cast<GeneratedImage>().SelectMany(r => r.Values).ToArray(),
                    results.Cast<GeneratedImage>().SelectMany(r => r.Paths).ToArray(),
                    results.Cast<GeneratedImage>().Select(r => r.Usage).Aggregate((a, b) => a.Merge(b))
                );
                return (TResult)(object)merged;
            }

            // --- GeneratedAudio ---
            if (typeof(TResult) == typeof(GeneratedAudio))
            {
                GeneratedAudio merged = new(
                    results.Cast<GeneratedAudio>().SelectMany(r => r.Values).ToArray(),
                    results.Cast<GeneratedAudio>().SelectMany(r => r.Paths).ToArray(),
                    results.Cast<GeneratedAudio>().Select(r => r.Usage).Aggregate((a, b) => a.Merge(b))
                );
                return (TResult)(object)merged;
            }

            // --- GeneratedVideo ---
            if (typeof(TResult) == typeof(GeneratedVideo))
            {
                GeneratedVideo merged = new(
                    results.Cast<GeneratedVideo>().SelectMany(r => r.Values).ToArray(),
                    results.Cast<GeneratedVideo>().SelectMany(r => r.Paths).ToArray(),
                    results.Cast<GeneratedVideo>().Select(r => r.Usage).Aggregate((a, b) => a.Merge(b))
                );
                return (TResult)(object)merged;
            }

            // --- Moderation ---
            if (typeof(TResult) == typeof(Moderation))
            {
                Moderation merged = new(
                    results.Cast<Moderation>().SelectMany(r => r.Values).ToArray(),
                    results.Cast<Moderation>().Select(r => r.Usage).Aggregate((a, b) => a.Merge(b))
                );
                return (TResult)(object)merged;
            }

            // --- Transcript ---
            // Not supported for merging

            throw new NotSupportedException($"Merging not supported for type {typeof(TResult).Name}");
        }
    }
}