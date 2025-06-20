using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Glitch9.AIDevKit.Editor
{
    internal static class ModelNameResolver
    {
        private static readonly Dictionary<string, string> _priorityParsingRules = new()
        {
            { "gpt", "GPT" },
            { "chatgpt", "ChatGPT" },
            { "tts", "TTS" },
            { "dall-e", "DALL·E" },
            { "dall·e", "DALL·E" },
            { "hd", "HD" },
            { "deekseek", "DeepSeek" },
        };

        internal static string ExtractDateString(string input)
        {
            var match = Regex.Match(input, @"\b\d{4}-\d{2}-\d{2}\b");
            return match.Success ? match.Value : null;
        }

        internal static string ResolveFromId(string id)
        {
            string name = id.GetLastSegment();
            string dateString = ExtractDateString(name);  // 날짜 형식 (yyyy-MM-dd)이 포함되어 있는지 확인 

            if (dateString != null) name = name.Replace(dateString, "").Trim();

            // Parse the name using the priority parsing rules
            foreach (var rule in _priorityParsingRules)
            {
                if (name.Contains(rule.Key, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Replace(rule.Key, rule.Value, StringComparison.OrdinalIgnoreCase);
                }
            }

            List<string> parts = name.Split('-').ToList();

            if (parts.Count > 1 &&
                parts[0].Contains("GPT", StringComparison.OrdinalIgnoreCase) &&
                char.IsDigit(parts[1][0]))
            {
                // Combine first two parts
                string newFirstElement = $"{parts[0]}-{parts[1]}";

                parts[0] = newFirstElement;
                parts.RemoveAt(1); // Remove the second part
            }

            name = JoinToTitleCase(parts); // Join the parts with spaces and convert to title case

            if (dateString != null) name = name.Trim() + " (" + dateString + ")"; // Add the date if it was found in ()
            if (ModelMetadataUtil.IsOModel(name)) name = name.UncapFirstChar();

            return name;
        }

        private static string JoinToTitleCase(List<string> parts)
        {
            if (parts.Count == 0) return string.Empty;

            return string.Join(" ", parts.Select(w => IgnoreTitleCase(w) ? w : w.CapFirstChar()));
        }

        private static bool IgnoreTitleCase(string word)
        {
            foreach (var rule in _priorityParsingRules)
            {
                if (word.Contains(rule.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static string RemoveColonPrefix(string name, int nameIndex = 1)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            if (name.Contains(':')) return name.Split(':')[nameIndex].Trim();
            return name;
        }
    }
}