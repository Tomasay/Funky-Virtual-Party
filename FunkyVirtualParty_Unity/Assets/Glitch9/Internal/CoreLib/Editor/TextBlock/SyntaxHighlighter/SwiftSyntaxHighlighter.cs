using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class SwiftSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Grays: Comments
            code = Regex.Replace(code, @"//.*", m => $"<color={Colors.Gray}>{m.Value}</color>");
            code = Regex.Replace(code, @"/\*.*?\*/", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Blues: Keywords
            code = Regex.Replace(code, @"\b(associatedtype|class|deinit|enum|extension|fileprivate|func|import|init|inout|internal|let|open|operator|private|protocol|public|static|struct|subscript|typealias|var)\b", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Reds: Strings
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Red}>{m.Value}</color>");

            // Oranges: Preprocessor directives
            code = Regex.Replace(code, @"#.*", m => $"<color={Colors.Orange}>{m.Value}</color>");

            return code;
        }
    }
}