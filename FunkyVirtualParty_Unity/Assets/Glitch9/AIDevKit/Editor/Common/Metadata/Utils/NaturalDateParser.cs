using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class NaturalDateParser
    {
        // Matches: March 25th, 2025 (within a sentence)
        private static readonly Regex Format1 = new(@"\b(?<month>\w+)\s(?<day>\d{1,2})(st|nd|rd|th)?,\s(?<year>\d{4})\b", RegexOptions.IgnoreCase);

        // Matches: October of 2024 (within a sentence)
        private static readonly Regex Format2 = new(@"\b(?<month>\w+)\s+of\s+(?<year>\d{4})\b", RegexOptions.IgnoreCase);

        internal static UnixTime? ResolveTimeFromDescription(string description)
        {
            if (TryParseNaturalDate(description, DateTimeKind.Local, out DateTime result)) return new UnixTime(result, false);
            return null;
        }

        private static bool TryParseNaturalDate(string input, DateTimeKind kind, out DateTime result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            // Try Format 1
            var match1 = Format1.Match(input);
            if (match1.Success)
            {
                string month = match1.Groups["month"].Value;
                string day = match1.Groups["day"].Value;
                string year = match1.Groups["year"].Value;

                string normalized = $"{month} {day}, {year}";
                DateTime.TryParseExact(normalized, "MMMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

                result = DateTime.SpecifyKind(result, kind);
                Debug.Log($"[Local] {result} / [UTC] {result.ToUniversalTime()} / [Kind] {result.Kind}");

                return true;
            }

            // Try Format 2
            var match2 = Format2.Match(input);
            if (match2.Success)
            {
                string month = match2.Groups["month"].Value;
                string year = match2.Groups["year"].Value;

                string normalized = $"{month} 1, {year}";
                DateTime.TryParseExact(normalized, "MMMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

                result = DateTime.SpecifyKind(result, kind);
                Debug.Log($"[Local] {result} / [UTC] {result.ToUniversalTime()} / [Kind] {result.Kind}");

                return true;
            }

            //Debug.LogWarning($"NaturalDateParser: Unable to parse date from input: {input}");
            return false;
        }
    }
}