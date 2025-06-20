using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glitch9
{
    public enum TextCase
    {
        Unset,
        CamelCase,
        PascalCase,
        SnakeCase,
        KebabCase,
        TitleCase,
        LowerCase,
        UpperCase
    }

    public static partial class StringExtensions
    {
        public static string ConvertToCase(this string text, TextCase stringCase)
        {
            return stringCase switch
            {
                TextCase.CamelCase => text.ToCamelCase(),
                TextCase.PascalCase => text.ToPascalCase(),
                TextCase.SnakeCase => text.ToSnakeCase(),
                TextCase.KebabCase => text.ToKebabCase(),
                TextCase.TitleCase => text.ToTitleCase(),
                TextCase.LowerCase => text?.ToLowerInvariant() ?? string.Empty,
                TextCase.UpperCase => text?.ToUpperInvariant() ?? string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(stringCase), stringCase, null),
            };
        }

        public static string ToCamelCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var words = SplitToWords(text);
            if (words.Count == 0) return string.Empty;

            return words[0].ToLowerInvariant() +
                   string.Concat(words.Skip(1).Select(w => w.CapFirstChar()));
        }

        public static string ToPascalCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return string.Concat(SplitToWords(text).Select(w => w.CapFirstChar()));
        }

        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsUpper(c))
                {
                    if (i > 0 && !char.IsUpper(text[i - 1])) sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            string result = sb.ToString();
            result = Regex.Replace(result, "_{2,}", "_");
            return result.Trim('_').ToLowerInvariant().Replace(" ", "");
        }

        public static string ToKebabCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return Regex.Replace(text, @"(\B[A-Z])", "-$1").ToLowerInvariant();
        }

        // public static string ToTitleCase(this string text)
        // {
        //     if (string.IsNullOrEmpty(text)) return string.Empty;
        //     // _ and empty space are treated as word separators
        //     // and the first letter of each word is capitalized
        //     // var words = Regex.Split(text, "(_+)")
        //     //     .Where(w => !string.IsNullOrWhiteSpace(w) && w != "_");
        //     var words = Regex.Split(text, @"[\s_-]+")
        //         .Where(w => !string.IsNullOrWhiteSpace(w) && w != "_");
        //     if (words.Count() == 0) return string.Empty;
        //     return string.Join(" ", words.Select(w => w.CapFirstChar()));
        // }

        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // Step 1: PascalCase → Insert space before uppercase letters (not at the start)
            string spaced = Regex.Replace(text, "(?<!^)([A-Z])", " $1");

            // Step 2: Split on spaces, underscores, hyphens
            var words = Regex.Split(spaced, @"[\s_-]+")
                .Where(w => !string.IsNullOrWhiteSpace(w) && w != "_");

            if (!words.Any()) return string.Empty;

            return string.Join(" ", words.Select(w => w.CapFirstChar()));
        }


        public static string CapFirstChar(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length == 1
                ? char.ToUpperInvariant(text[0]).ToString()
                : char.ToUpperInvariant(text[0]) + text[1..].ToLowerInvariant();
        }

        public static string CapFirstChars(this string text, char separator = ' ')
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var words = text.Split(separator);
            return string.Join(separator.ToString(), words.Select(w => w.CapFirstChar()));
        }

        public static string UncapFirstChar(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length == 1
                ? char.ToLowerInvariant(text[0]).ToString()
                : char.ToLowerInvariant(text[0]) + text[1..].ToLowerInvariant();
        }

        private static List<string> SplitToWords(string input)
        {
            return input.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
