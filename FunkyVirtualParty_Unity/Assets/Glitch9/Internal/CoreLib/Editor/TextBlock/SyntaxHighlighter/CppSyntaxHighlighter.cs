using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class CppSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            // Grays: Comments
            code = Regex.Replace(code, @"//.*", m => $"<color={Colors.Gray}>{m.Value}</color>");
            code = Regex.Replace(code, @"/\*.*?\*/", m => $"<color={Colors.Gray}>{m.Value}</color>");

            // Blues: Keywords
            code = Regex.Replace(code, @"\b(alignas|alignof|and|and_eq|asm|auto|bitand|bitor|bool|break|case|catch|char|char16_t|char32_t|class|compl|concept|const|constexpr|const_cast|continue|decltype|default|delete|do|double|dynamic_cast|else|enum|explicit|export|extern|false|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|noexcept|not|not_eq|nullptr|operator|or|or_eq|private|protected|public|register|reinterpret_cast|requires|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|true|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while|xor|xor_eq)\b", m => $"<color={Colors.Blue}>{m.Value}</color>");

            // Reds: Strings
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Red}>{m.Value}</color>");

            // Oranges: Preprocessor directives
            code = Regex.Replace(code, @"#.*", m => $"<color={Colors.Orange}>{m.Value}</color>");

            return code;
        }
    }
}