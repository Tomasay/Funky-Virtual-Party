using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class RubySyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Oranges: strings with double quotes
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Orange}>{m.Value}</color>");

            // Grays: Comments
            code = Regex.Replace(code, "#.*", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Greens: Keywords
            code = Regex.Replace(code, @"\b(alias|and|BEGIN|begin|break|case|class|def|defined|do|else|elsif|END|end|ensure|false|for|if|in|module|next|nil|not|or|redo|rescue|retry|return|self|super|then|true|undef|unless|until|when|while|yield)\b", m => $"<color={Colors.Green}>{m.Value}</color>");

            // Blues: Symbols
            code = Regex.Replace(code, @"(:\w+)", m => $"<color={Colors.Blue}>{m.Value}</color>");

            return code;
        }
    }
}