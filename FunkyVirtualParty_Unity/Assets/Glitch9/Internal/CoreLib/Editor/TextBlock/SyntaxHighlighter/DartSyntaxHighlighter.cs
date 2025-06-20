using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class DartSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Grays: Comments
            code = Regex.Replace(code, @"//.*", m => $"<color={Colors.Gray}>{m.Value}</color>");
            code = Regex.Replace(code, @"/\*.*?\*/", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Blues: Keywords
            code = Regex.Replace(code, @"\b(abstract|as|assert|async|await|break|case|catch|class|const|continue|default|deferred|do|dynamic|else|enum|export|extends|extension|external|factory|false|final|finally|for|Function|get|if|implements|import|in|is|library|mixin|new|null|on|operator|part|rethrow|return|set|static|super|switch|sync|this|throw|true|try|typedef|var|void|while|with|yield)\b", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Reds: Strings
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Red}>{m.Value}</color>");

            // Oranges: Preprocessor directives
            code = Regex.Replace(code, @"#.*", m => $"<color={Colors.Orange}>{m.Value}</color>");

            return code;
        }
    }
}