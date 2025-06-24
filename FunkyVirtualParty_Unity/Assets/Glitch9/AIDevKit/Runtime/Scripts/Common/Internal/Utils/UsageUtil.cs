using System;
using System.Collections.Generic;
using Glitch9.Collections;

namespace Glitch9.AIDevKit
{
    internal static class UsageUtil
    {
        internal static Usage Merge(this Usage usage, Usage usageToAdd)
        {
            if (usage == null) return usageToAdd;
            if (usageToAdd == null || usageToAdd.IsEmpty) return usage;

            usage.InputTokens += usageToAdd.InputTokens;
            usage.OutputTokens += usageToAdd.OutputTokens;

            if (usageToAdd.usages.IsNotNullOrEmpty())
            {
                usage.usages ??= new();

                foreach (var kvp in usageToAdd.usages)
                {
                    if (kvp.Value <= 0) continue;
                    AIDevKitDebug.Green($"Merging usage: {kvp.Key.GetInspectorName()} - {kvp.Value}");

                    if (usage.usages.ContainsKey(kvp.Key))
                    {
                        usage.usages[kvp.Key] += kvp.Value;
                    }
                    else
                    {
                        usage.usages.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            usage.ResetInspectorTexts();
            return usage;
        }

        internal static string[] CreateInspectorTextParts(ReferencedDictionary<UsageType, int> usages, string[] cached)
        {
            if (cached != null) return cached;
            if (usages.IsNullOrEmpty()) return new[] { "N/A" };

            List<string> parts = new();

            foreach (var kvp in usages)
            {
                if (kvp.Value > 0)
                {
                    parts.Add($"{kvp.Key.GetShortInspectorName()}: {kvp.Value}");
                }
            }

            if (parts.Count == 0) return new[] { "N/A" };
            return parts.ToArray();
        }
    }
}