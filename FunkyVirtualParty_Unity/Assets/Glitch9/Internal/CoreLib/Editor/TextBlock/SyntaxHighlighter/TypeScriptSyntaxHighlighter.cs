using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class TypeScriptSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Oranges: strings with double quotes
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Orange}>{m.Value}</color>");

            // Grays: Comments
            code = Regex.Replace(code, @"//.*", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Greens: class names (e.g., class MyClass) and object names (after 'new' keyword)
            code = Regex.Replace(code, @"\b(class|new)\s+(\w+)\b", m => $"{m.Groups[1].Value} <color={Colors.Green}>{m.Groups[2].Value}</color>");

            // Blues: import, package, class, public, private, protected, static, void, int, String, true, false, null, new, finally, in, var, boolean, float, double, long
            code = Regex.Replace(code, @"\b(import|package|class|public|private|protected|static|void|int|String|true|false|null|new|finally|in|var|boolean|float|double|long)\b", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Pinks: return, if, else, while, for, break, continue, switch, case, default, try, catch, throw
            code = Regex.Replace(code, @"\b(return|if|else|while|for|break|continue|switch|case|default|try|catch|throw)\b", m => $"<color={Colors.Pink}>{m.Value}</color>");

            // Yellows: method names (identifiers followed by a parenthesis)
            code = Regex.Replace(code, @"\b(\w+)(?=\()", m => $"<color={Colors.Red}>{m.Groups[1].Value}</color>");
            return code;
        }
    }
}