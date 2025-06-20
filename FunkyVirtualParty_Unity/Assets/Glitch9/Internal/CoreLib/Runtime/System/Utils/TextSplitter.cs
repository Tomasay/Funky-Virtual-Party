using System;
using System.Collections.Generic;
using System.Text;

namespace Glitch9
{
    public static class TextSplitter
    {
        // List of common conjunctions/connectors
        private static readonly HashSet<string> kConjunctions = new(StringComparer.OrdinalIgnoreCase)
        {
            "and", "but", "or", "so", "because", "then", "however", "therefore", "thus", "moreover", "although", "though", "yet"
        };

        public static string SplitToParagraphs(string input, int linebreaks = 2)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                int parenDepth = 0;
                int consecutiveLineBreaks = 0;
                bool justAddedBreak = false;
                bool paragraphStart = true;
                string breaks = new('\n', linebreaks);

                for (int i = 0; i < input.Length; i++)
                {
                    char c = input[i];

                    if (c == '\n')
                    {
                        consecutiveLineBreaks++;
                        if (consecutiveLineBreaks <= linebreaks)
                            sb.Append(c);

                        if (consecutiveLineBreaks >= linebreaks)
                            paragraphStart = true;

                        continue;
                    }
                    else
                    {
                        consecutiveLineBreaks = 0;
                    }

                    // 문단 시작 시 선행 공백 제거
                    if (paragraphStart && char.IsWhiteSpace(c))
                        continue;

                    paragraphStart = false;
                    sb.Append(c);

                    if (c == '(') parenDepth++;
                    else if (c == ')') parenDepth = Math.Max(parenDepth - 1, 0);

                    if (parenDepth == 0 && (c == '.' || c == '!' || c == '?'))
                    {
                        if (i + 1 >= input.Length || char.IsWhiteSpace(input[i + 1]))
                        {
                            if (!justAddedBreak)
                            {
                                sb.Append(breaks);
                                justAddedBreak = true;
                                paragraphStart = true;
                            }
                        }
                        else
                        {
                            justAddedBreak = false;
                        }
                    }
                    else
                    {
                        justAddedBreak = false;
                    }
                }

                return sb.ToString().TrimEnd('\n', '\r');
            }
        }

        public static string[] SplitSmart(string input, int maxLineLength = 80)
        {
            if (string.IsNullOrEmpty(input) || maxLineLength <= 0) return new[] { input };

            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                List<string> lines = new List<string>();
                int lastSplitIndex = 0;

                for (int i = 0; i < input.Length; i++)
                {
                    if (i - lastSplitIndex >= maxLineLength)
                    {
                        int splitAt = FindBestSplit(input, lastSplitIndex, i);

                        if (splitAt <= lastSplitIndex)
                            splitAt = i; // Fallback: force split

                        lines.Add(input.Substring(lastSplitIndex, splitAt - lastSplitIndex).Trim());
                        lastSplitIndex = splitAt;
                    }
                }

                if (lastSplitIndex < input.Length)
                {
                    lines.Add(input.Substring(lastSplitIndex).Trim());
                }

                return lines.ToArray();
            }
        }

        private static int FindBestSplit(string text, int start, int end)
        {
            // Priority 1: Punctuation
            for (int i = end; i > start; i--)
            {
                if (IsPunctuation(text[i - 1]))
                    return i;
            }

            // Priority 2: Comma
            for (int i = end; i > start; i--)
            {
                if (text[i - 1] == ',')
                    return i;
            }

            // Priority 3: Conjunction (only after spaces)
            for (int i = end; i > start + 1; i--)
            {
                if (text[i - 1] == ' ')
                {
                    string word = GetWordAfterSpace(text, i);
                    if (kConjunctions.Contains(word))
                        return i;
                }
            }

            // Priority 4: Space
            for (int i = end; i > start; i--)
            {
                if (text[i - 1] == ' ')
                    return i;
            }

            return -1;
        }

        private static bool IsPunctuation(char c)
        {
            return c == '.' || c == '!' || c == '?' || c == ':';
        }

        private static string GetWordAfterSpace(string text, int spaceIndex)
        {
            int end = text.IndexOf(' ', spaceIndex);
            if (end == -1) end = text.Length;

            return text.Substring(spaceIndex, end - spaceIndex).Trim();
        }
    }
}