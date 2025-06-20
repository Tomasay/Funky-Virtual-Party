using System.Collections.Generic;
using System.Text.RegularExpressions;
using Glitch9.Editor.UIToolkit;
using UnityEngine.UIElements;

namespace Glitch9.Editor
{
    public partial class TextBlock
    {
        private static readonly Dictionary<int, List<TextBlock>> _cachedTextBlocks = new();

        private static bool TryGetTextBlocks(string text, out List<TextBlock> textBlocks)
        {
            textBlocks = null;
            if (string.IsNullOrEmpty(text)) return false;

            int hashCode = text.GetHashCode();
            if (_cachedTextBlocks.TryGetValue(hashCode, out textBlocks))
            {
                return true;
            }

            // If not cached, create a new list  
            textBlocks = new List<TextBlock>();
            int lastIndex = 0;

            // Regex Version 2
            MatchCollection matches = Regex.Matches(text,
                @"(###\s+(?<header>.+))" +
                @"|(>\s+(?<quote>.+))" +
                @"|(```(?<language>\w+)\s*(?<code>[\s\S]*?)\s*```)" +
                @"|(^\s*(?<ulist_header>\d+\.\s+\*\*.+?\*\*):)" +
                @"|^\s*[-*+]\s+(?<ulist>.+)",
                RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                int matchStartIndex = match.Index;

                // Add plain text before the match
                if (matchStartIndex > lastIndex)
                {
                    string plainText = text.Substring(lastIndex, matchStartIndex - lastIndex).Trim();
                    if (!string.IsNullOrEmpty(plainText)) textBlocks.Add(TextBlock.Text(TextBlockUtil.ProcessTags(plainText)));
                }

                if (match.Groups["header"].Success)
                {
                    string headerContent = match.Groups["header"].Value.Trim();
                    if (!string.IsNullOrEmpty(headerContent)) textBlocks.Add(TextBlock.Header(headerContent, TextBlockUtil.CalculateHeaderLevel(headerContent)));
                }
                else if (match.Groups["ulist_header"].Success)
                {
                    string rawHeader = match.Groups["ulist_header"].Value.Trim();
                    if (!string.IsNullOrEmpty(rawHeader))
                    {
                        string cleanHeader = Regex.Replace(rawHeader, @"\*\*(.+?)\*\*", "$1");
                        textBlocks.Add(TextBlock.UListHeader(cleanHeader)); // Assuming level 0 for list headers
                    }
                }
                else if (match.Groups["ulist"].Success)
                {
                    string listContent = match.Groups["ulist"].Value.Trim();
                    if (!string.IsNullOrEmpty(listContent)) textBlocks.Add(TextBlock.UList(TextBlockUtil.ProcessTags(listContent)));
                }
                else if (match.Groups["code"].Success)
                {
                    string language = match.Groups["language"].Value;
                    string code = match.Groups["code"].Value.Trim();
                    if (!string.IsNullOrEmpty(code)) textBlocks.Add(TextBlock.CodeBlock(language, code));
                }
                else if (match.Groups["quote"].Success)
                {
                    string quoteContent = match.Groups["quote"].Value.Trim();
                    if (!string.IsNullOrEmpty(quoteContent)) textBlocks.Add(TextBlock.Quote(TextBlockUtil.ProcessTags(quoteContent)));
                }

                lastIndex = match.Index + match.Length;
            }

            // Add any remaining plain text after the last match
            if (lastIndex < text.Length)
            {
                string plainText = text.Substring(lastIndex).Trim();
                if (!string.IsNullOrEmpty(plainText)) textBlocks.Add(TextBlock.Text(plainText));
            }

            return _cachedTextBlocks.TryAdd(hashCode, textBlocks);
        }

        public static void DrawIMGUI(List<TextBlock> textBlocks, float maxWidth)
        {
            TextBlock prevBlock = null;
            foreach (TextBlock textBlock in textBlocks)
            {
                TextBlockIMGUI.Draw(textBlock, prevBlock, maxWidth);
                prevBlock = textBlock;
            }
        }

        public static void DrawIMGUI(string text, float maxWidth)
        {
            if (string.IsNullOrEmpty(text) || !TryGetTextBlocks(text, out List<TextBlock> textBlocks)) return;
            DrawIMGUI(textBlocks, maxWidth);
        }

        public static VisualElement BuildTextBlockElement(List<TextBlock> textBlocks)
        {
            if (textBlocks == null || textBlocks.Count == 0) return null;

            VisualElement root = new();
            UIToolkitUtil.InitializeRootElement(root, "TextBlock", "styles_text_block_marker");
            root.AddToClassList("text-block-root");

            TextBlock prevBlock = null;
            foreach (TextBlock textBlock in textBlocks)
            {
                TextBlockUIToolkit.Draw(root, textBlock, prevBlock);
                prevBlock = textBlock;
            }

            return root;
        }

        public static VisualElement BuildTextBlockElement(string text)
        {
            if (string.IsNullOrEmpty(text) || !TryGetTextBlocks(text, out List<TextBlock> textBlocks)) return null;
            return BuildTextBlockElement(textBlocks);
        }
    }
}