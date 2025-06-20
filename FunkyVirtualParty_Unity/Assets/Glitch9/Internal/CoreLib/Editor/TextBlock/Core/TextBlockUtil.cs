using System.Linq;
using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    internal class TextBlockUtil
    {
        // Helper method to calculate header level based on the Markdown syntax
        internal static int CalculateHeaderLevel(string headerContent)
        {
            return headerContent.TakeWhile(c => c == '#').Count();
        }

        internal static string ProcessTags(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string result = input;

            // Inline code: `code`
            result = Regex.Replace(result, "`([^`\r\n]+?)`", "<color=cyan>$1</color>");

            // Bold: **text**
            result = Regex.Replace(result, @"\*\*(.+?)\*\*", "<b>$1</b>");

            // Italic: *text*
            result = Regex.Replace(result, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");

            // Strikethrough: ~~text~~
            result = Regex.Replace(result, @"~~(.+?)~~", "<s>$1</s>");

            // Quatation Mark: "text"
            result = Regex.Replace(result, @"""(.+?)""", "<color=yellow>$1</color>");

            return result;
        }

        internal static string GetExtension(string language)
        {
            string languageLower = language.ToLowerInvariant();

            if (languageLower == "csharp" || languageLower == "cs")
            {
                return "csharp";
            }
            else if (languageLower == "javascript" || languageLower == "js")
            {
                return "javascript";
            }
            else if (languageLower == "python" || languageLower == "py")
            {
                return "python";
            }
            else if (languageLower == "html")
            {
                return "html";
            }
            else if (languageLower == "css")
            {
                return "css";
            }
            else if (languageLower == "java")
            {
                return "java";
            }
            else if (languageLower == "ruby")
            {
                return "ruby";
            }
            else if (languageLower == "cpp" || languageLower == "c++")
            {
                return "cpp";
            }
            else if (languageLower == "typescript" || languageLower == "ts")
            {
                return "typescript";
            }
            else if (languageLower == "dart")
            {
                return "dart";
            }
            else if (languageLower == "kotlin")
            {
                return "kotlin";
            }
            else if (languageLower == "swift")
            {
                return "swift";
            }
            else if (languageLower == "objective-c" || languageLower == "objc")
            {
                return "objective-c";
            }
            else if (languageLower == "uxml")
            {
                return "uxml";
            }
            else if (languageLower == "shadergraph")
            {
                return "shadergraph";
            }
            else
            {
                return language;
            }
        }
    }
}