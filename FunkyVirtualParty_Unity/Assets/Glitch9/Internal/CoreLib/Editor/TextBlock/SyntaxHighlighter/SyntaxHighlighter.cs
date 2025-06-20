using System;
using System.Collections.Generic;
using System.Text;
using SHA256 = System.Security.Cryptography.SHA256;

namespace Glitch9.Editor
{
    public abstract class SyntaxHighlighter
    {
        private static readonly Dictionary<string, SyntaxHighlighter> _syntaxHighlighters = new();
        private static readonly Dictionary<string, string> _cachedHighlightedCode = new();

        public static string Highlight(string language, string code)
        {
            if (string.IsNullOrEmpty(language) || string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }

            string hash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(code)));

            if (_cachedHighlightedCode.TryGetValue(hash, out string highlightedCode))
            {
                return highlightedCode;
            }

            SyntaxHighlighter syntaxHighlighter = GetSyntaxHighlighter(language);
            highlightedCode = syntaxHighlighter.HighlightInternal(code);
            _cachedHighlightedCode[hash] = highlightedCode;

            return highlightedCode;
        }

        private static SyntaxHighlighter GetSyntaxHighlighter(string language)
        {
            if (!_syntaxHighlighters.TryGetValue(language, out SyntaxHighlighter syntaxHighlighter))
            {
                switch (language)
                {
                    case "csharp":
                        syntaxHighlighter = new CSharpSyntaxHighlighter();
                        break;
                    case "java":
                        syntaxHighlighter = new JavaSyntaxHighlighter();
                        break;
                    case "python":
                        syntaxHighlighter = new PythonSyntaxHighlighter();
                        break;
                    case "javascript":
                        syntaxHighlighter = new JavaScriptSyntaxHighlighter();
                        break;
                    case "typescript":
                        syntaxHighlighter = new TypeScriptSyntaxHighlighter();
                        break;
                    case "html":
                        syntaxHighlighter = new HtmlSyntaxHighlighter();
                        break;
                    case "ruby":
                        syntaxHighlighter = new RubySyntaxHighlighter();
                        break;
                    case "css":
                        syntaxHighlighter = new CssSyntaxHighlighter();
                        break;
                    case "cpp":
                        syntaxHighlighter = new CppSyntaxHighlighter();
                        break;
                    case "objective-c":
                        syntaxHighlighter = new ObjectiveCSyntaxHighlighter();
                        break;
                    case "swift":
                        syntaxHighlighter = new SwiftSyntaxHighlighter();
                        break;
                    case "kotlin":
                        syntaxHighlighter = new KotlinSyntaxHighlighter();
                        break;
                    case "dart":
                        syntaxHighlighter = new DartSyntaxHighlighter();
                        break;
                    case "uxml":
                        syntaxHighlighter = new UxmlSyntaxHighlighter();
                        break;
                }

                _syntaxHighlighters[language] = syntaxHighlighter;
            }

            return syntaxHighlighter;
        }

        protected static class Colors
        {
            internal const string Blue = "#4a90e2ff";
            internal const string Green = "#26f0b9ff";
            internal const string Pink = "#ff5a5aff";
            internal const string Red = "red";
            internal const string Orange = "#f5a623ff";
            internal const string Gray = "#929292ff";
            public const string Purple = "#C586C0";
            public const string LightGreen = "#B5CEA8";
            public const string Teal = "#4EC9B0";
            public const string Yellow = "#DCDCAA";
            public const string White = "#FFFFFF";
        }

        protected abstract string HighlightInternal(string code);
    }
}