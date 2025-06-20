using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Glitch9.Editor
{

    // public enum TokenType
    // {
    //     Keyword,
    //     Identifier,
    //     StringLiteral,
    //     Number,
    //     Comment,
    //     Operator,
    //     Whitespace,
    //     Punctuation,
    //     Unknown
    // }

    // public struct Token
    // {
    //     public TokenType Type;
    //     public int Start;
    //     public int Length;

    //     public Token(TokenType type, int start, int length)
    //     {
    //         Type = type;
    //         Start = start;
    //         Length = length;
    //     }
    // }

    // public static class SimpleCSharpTokenizer
    // {
    //     private static readonly HashSet<string> Keywords = new()
    //     {
    //         "using", "namespace", "public", "class", "static", "void", "int", "string", "bool",
    //         "float", "double", "if", "else", "for", "while", "return", "new", "null", "true", "false"
    //     };

    //     public static List<Token> Tokenize(string code)
    //     {
    //         var tokens = new List<Token>();
    //         int i = 0;

    //         while (i < code.Length)
    //         {
    //             char c = code[i];

    //             if (char.IsWhiteSpace(c))
    //             {
    //                 int start = i;
    //                 while (i < code.Length && char.IsWhiteSpace(code[i])) i++;
    //                 tokens.Add(new Token(TokenType.Whitespace, start, i - start));
    //             }
    //             else if (char.IsLetter(c) || c == '_')
    //             {
    //                 int start = i;
    //                 while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_')) i++;
    //                 string word = code.Substring(start, i - start);
    //                 tokens.Add(new Token(Keywords.Contains(word) ? TokenType.Keyword : TokenType.Identifier, start, i - start));
    //             }
    //             else if (char.IsDigit(c))
    //             {
    //                 int start = i;
    //                 while (i < code.Length && char.IsDigit(code[i])) i++;
    //                 tokens.Add(new Token(TokenType.Number, start, i - start));
    //             }
    //             else if (c == '"')
    //             {
    //                 int start = i++;
    //                 while (i < code.Length && code[i] != '"')
    //                 {
    //                     if (code[i] == '\\' && i + 1 < code.Length) i += 2;
    //                     else i++;
    //                 }
    //                 if (i < code.Length) i++; // closing quote
    //                 tokens.Add(new Token(TokenType.StringLiteral, start, i - start));
    //             }
    //             else if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
    //             {
    //                 int start = i;
    //                 i += 2;
    //                 while (i < code.Length && code[i] != '\n') i++;
    //                 tokens.Add(new Token(TokenType.Comment, start, i - start));
    //             }
    //             else if ("{}();.,:+-*/%=&|!<>^".IndexOf(c) >= 0)
    //             {
    //                 tokens.Add(new Token(TokenType.Operator, i, 1));
    //                 i++;
    //             }
    //             else
    //             {
    //                 tokens.Add(new Token(TokenType.Unknown, i, 1));
    //                 i++;
    //             }
    //         }

    //         return tokens;
    //     }

    //     public static string Highlight(string code)
    //     {
    //         var tokens = Tokenize(code);
    //         var sb = new StringBuilder();

    //         foreach (var token in tokens)
    //         {
    //             string text = code.Substring(token.Start, token.Length);
    //             string colorTag = token.Type switch
    //             {
    //                 TokenType.Keyword => "<color=#569CD6>",
    //                 TokenType.Identifier => "<color=#DCDCAA>",
    //                 TokenType.StringLiteral => "<color=#CE9178>",
    //                 TokenType.Number => "<color=#B5CEA8>",
    //                 TokenType.Comment => "<color=#6A9955>",
    //                 TokenType.Operator => "<color=#C586C0>",
    //                 _ => ""
    //             };
    //             if (!string.IsNullOrEmpty(colorTag))
    //                 sb.Append(colorTag).Append(text).Append("</color>");
    //             else
    //                 sb.Append(text);
    //         }

    //         return sb.ToString();
    //     }
    // }

    public class CSharpSyntaxHighlighter : SyntaxHighlighter
    {
        private static readonly string[] blueKeywords =
        {
            "using", "namespace", "class", "public", "private", "protected", "static", "override",
            "void", "int", "string", "true", "false", "null", "new", "finally", "in", "var", "bool",
            "float", "double", "long", "short", "decimal", "byte", "sbyte", "char", "object",
            "dynamic", "async", "await", "this", "base", "get", "set", "add", "remove", "lock",
            "volatile", "readonly", "sealed", "abstract", "virtual", "interface", "enum", "struct",
            "delegate", "event", "checked", "unchecked", "fixed", "unsafe", "is", "as", "sizeof",
            "typeof", "default", "nameof", "params", "ref", "out", "in", "where", "yield",
            "internal", "extern", "partial", "dynamic", "using static", "using alias",
        };

        private static readonly string[] pinkKeywords =
        {
            "return", "if", "else", "while", "for", "foreach", "break", "continue",
            "switch", "case", "default", "try", "catch", "throw"
        };

        private static readonly string[] greenKeywords =
        {
            "Task", "IEnumerable", "IEnumerator", "IDisposable", "Action", "Func",
            "List", "Dictionary", "HashSet", "Queue", "Stack", "Array", "StringBuilder",
            "String", "DateTime", "TimeSpan", "Uri", "Regex", "JsonSerializer",
            "UniTask", "ValueTask", "CancellationToken",
        };

        protected override string HighlightInternal(string code)
        {
            // 1. Extract and mask comments, excluding URLs
            var commentMap = new Dictionary<string, string>();
            int commentIndex = 0;

            // 정규식 설명:
            // (?!...) → Negative lookahead
            // ^\s*// → 줄의 시작에서 공백 후 주석 시작
            // (?!\s*(https?|file):) → http://, https://, file://로 시작하지 않도록 제외
            var commentPattern = @"(?<!:)//(?!\s*(https?|file):).*";

            var commentMatches = Regex.Matches(code, commentPattern);
            foreach (Match match in commentMatches)
            {
                string key = $"§COMMENT§{commentIndex++}§";
                commentMap[key] = $"<color={Colors.Gray}>{match.Value}</color>";
                code = code.Replace(match.Value, key);
            }

            // 2. Extract and mask strings first
            var stringMatches = Regex.Matches(code, "@\"(?:[^\"]|\"\")*\"|\"(?:\\\\.|[^\"\\\\])*\"");
            var stringMap = new Dictionary<string, string>();
            for (int i = 0; i < stringMatches.Count; i++)
            {
                string key = $"§STRING§{i}§";
                stringMap[key] = $"<color={Colors.Orange}>{stringMatches[i].Value}</color>";
                code = code.Replace(stringMatches[i].Value, key);
            }
            var userTypes = Regex.Matches(code, @"\b(class|struct|interface|enum)\s+(\w+)")
              .Cast<Match>()
              .Select(m => m.Groups[2].Value)
              .ToHashSet();

            // 3. Highlight strings (double-quoted)
            code = Regex.Replace(code, "\".*?\"", m => $"<color={Colors.Orange}>{m.Value}</color>");

            // 4. Highlight class or object instantiations
            code = Regex.Replace(code, @"\b(class|new)\s+(\w+)\b", m =>
                $"{m.Groups[1].Value} <color={Colors.Green}>{m.Groups[2].Value}</color>");

            // 4.5. Highlight everything after 'namespace' keyword. Even after dots.
            code = Regex.Replace(code, @"\bnamespace\s+(\w+(?:\.\w+)*)", m =>
            {
                string ns = m.Groups[1].Value;
                return $"namespace <color={Colors.Green}>{ns}</color>";
            });

            // 5. Highlight keywords  
            code = RegexReplaceWordList(code, blueKeywords, Colors.Blue);
            code = RegexReplaceWordList(code, pinkKeywords, Colors.Pink);
            code = RegexReplaceWordList(code, greenKeywords, Colors.Green);

            // 6. Highlight method names
            code = Regex.Replace(code, @"\b(\w+)(?=\()", m =>
                $"<color={Colors.Red}>{m.Groups[1].Value}</color>");

            // 7. Highlight user-defined types elsewhere
            foreach (string type in userTypes)
            {
                code = Regex.Replace(code, $@"\b{type}\b", $"<color={Colors.Green}>{type}</color>");
            }

            // 7.5. Highlight generics (<word>) <> is white and word is green
            code = Regex.Replace(code, @"<(\w+)>", m =>
                $"<color={Colors.White}><color={Colors.Green}>{m.Groups[1].Value}</color></color>");

            // 마지막에 복원
            foreach (var kvp in stringMap)
                code = code.Replace(kvp.Key, kvp.Value);

            foreach (var kvp in commentMap)
                code = code.Replace(kvp.Key, kvp.Value);

            return code;
        }

        private string RegexReplaceWordList(string input, IEnumerable<string> keywords, string color)
        {
            string pattern = $@"\b({string.Join("|", keywords)})\b";
            return Regex.Replace(input, pattern, m => $"<color={color}>{m.Value}</color>");
        }
    }
}
