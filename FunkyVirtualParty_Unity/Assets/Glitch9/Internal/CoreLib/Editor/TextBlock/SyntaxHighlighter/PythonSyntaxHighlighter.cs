using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class PythonSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            code = HighlightKeywords(code, Colors.Blue);
            code = HighlightStrings(code, Colors.Orange);
            code = HighlightComments(code, Colors.Gray);
            code = HighlightNumbers(code, Colors.Pink);
            return code;
        }

        private string HighlightKeywords(string code, string color)
        {
            string[] keywords = new string[]
            {
                "False", "None", "True", "and", "as", "assert", "async", "await", "break", "class", "continue", "def", "del", "elif", "else", "except", "finally", "for", "from", "global", "if", "import", "in", "is", "lambda", "nonlocal", "not", "or", "pass", "raise", "return", "try", "while", "with", "yield"
            };
            return HighlightWords(code, keywords, color);
        }

        private string HighlightStrings(string code, string color)
        {
            return HighlightRegex(code, "\".*?\"", color);
        }

        private string HighlightComments(string code, string color)
        {
            return HighlightRegex(code, "#.*$", color);
        }

        private string HighlightNumbers(string code, string color)
        {
            return HighlightRegex(code, @"\b\d+\b", color);
        }

        private string HighlightWords(string code, string[] words, string color)
        {
            foreach (string word in words)
            {
                code = Regex.Replace(code, $@"\b{word}\b", $"<color={color}>{word}</color>");
            }
            return code;
        }

        private string HighlightRegex(string code, string pattern, string color)
        {
            return Regex.Replace(code, pattern, $"<color={color}>$0</color>");
        }
    }
}