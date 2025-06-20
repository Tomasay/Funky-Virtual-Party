using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class KotlinSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Grays: Comments
            code = Regex.Replace(code, @"//.*", m => $"<color={Colors.Gray}>{m.Value}</color>");
            code = Regex.Replace(code, @"/\*.*?\*/", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Blues: Keywords
            code = Regex.Replace(code, @"\b(as|as?|break|class|continue|do|else|false|for|fun|if|in|interface|is|!is|null|object|package|return|super|this|throw|true|try|typealias|val|var|when|while)\b", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Reds: Strings
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Red}>{m.Value}</color>");

            // Oranges: Preprocessor directives
            code = Regex.Replace(code, @"#.*", m => $"<color={Colors.Orange}>{m.Value}</color>");

            return code;
        }
    }
}