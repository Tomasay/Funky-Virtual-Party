using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Glitch9.Editor
{
    public class UxmlSyntaxHighlighter : SyntaxHighlighter
    {
        protected override string HighlightInternal(string code)
        {
            var masks = new Dictionary<string, string>();
            int maskIndex = 0;

            string Mask(string original, string color, bool escape = false)
            {
                string key = $"§MASK§{maskIndex++}§";
                string content = escape ? EscapeXml(original) : original;
                masks[key] = $"<color={color}>{content}</color>";
                return key;
            }

            // 주석 마스킹
            code = Regex.Replace(code, @"<!--.*?-->", m => Mask(m.Value, Colors.Gray, escape: true), RegexOptions.Singleline);

            // 문자열 마스킹
            code = Regex.Replace(code, "\"(?:\\\\.|[^\"])*?\"", m => Mask(m.Value, Colors.Orange, escape: true));

            // 태그 이름 감싸기 (raw 그대로)
            code = Regex.Replace(code, @"</?(\w+:\w+)", m => Mask(m.Value, Colors.Blue));

            // 속성 이름 감싸기
            code = Regex.Replace(code, @"\b(\w+)=+", m =>
                $"{Mask(m.Groups[1].Value, Colors.Pink)}=");

            // 마스킹 복원 (마지막에만!)
            foreach (var kvp in masks)
                code = code.Replace(kvp.Key, kvp.Value);

            return code;
        }

        private string EscapeXml(string input)
        {
            return input.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;");
            // DO NOT escape double quotes → no .Replace("\"", "&quot;")
        }
    }
}