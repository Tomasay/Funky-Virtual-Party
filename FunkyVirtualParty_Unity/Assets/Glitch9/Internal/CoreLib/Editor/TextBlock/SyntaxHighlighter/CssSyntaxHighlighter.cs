using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class CssSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Grays: Comments
            code = Regex.Replace(code, @"/\*.*?\*/", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Blues: Property names
            code = Regex.Replace(code, @"(\w+)(?=\s*:)", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Reds: Property values
            code = Regex.Replace(code, @"(?<=:).*?(?=[;{])", m => $"<color={Colors.Red}>{m.Value}</color>");

            return code;
        }
    }
}