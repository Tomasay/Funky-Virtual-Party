using System;
using System.Text;
using Vector2 = UnityEngine.Vector2;
using Vector4 = UnityEngine.Vector4;

namespace Glitch9
{
    public static partial class StringExtensions
    {
        private const string kTag = nameof(StringExtensions);

        public static bool LengthCheck(this string text, int minLength, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                LogService.Warning(kTag, "Text is null or empty.");
                return false;
            }

            if (text.Length < minLength)
            {
                LogService.Warning(kTag, $"Text is too short. Min: {minLength}, Current: {text.Length}");
                return false;
            }

            if (text.Length > maxLength)
            {
                LogService.Warning(kTag, $"Text is too long. Max: {maxLength}, Current: {text.Length}");
                return false;
            }

            return true;
        }

        public static int ToInt(this string text)
        {
            if (int.TryParse(text, out int result))
            {
                return result;
            }
            else
            {
                LogService.Error(kTag, $"Failed to parse int: {text}");
                return 0;
            }
        }

        public static string RemoveLineBreaks(this string text)
        {
            return text.Replace("\r", "").Replace("\n", "");
        }

        public static string FormatArg(this string text, params object[] args)
        {
            try
            {
                return args.IsNullOrEmpty() ? text : string.Format(text, args);
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                return text;
            }
        }

        public static string VectorToString(this Vector2 vector)
        {
            return $"{vector.x},{vector.x}";
        }

        public static string VectorToString(this Vector4 vector)
        {
            return $"{vector.x},{vector.x},{vector.z},{vector.w}";
        }

        public static Vector2 StringToVector2(this string vectorString)
        {
            string[] components = vectorString.Split(',');

            if (components.Length == 2 && float.TryParse(components[0], out float x) && float.TryParse(components[1], out float y))
            {
                return new Vector2(x, y);
            }
            else
            {
                LogService.Error(kTag, $"Failed to parse vector string: {vectorString}");
                return Vector2.zero;
            }
        }

        public static Vector4 StringToVector4(this string vectorString)
        {
            string[] components = vectorString.Split(',');

            if (components.Length == 4 && float.TryParse(components[0], out float x) && float.TryParse(components[1], out float y) && float.TryParse(components[2], out float z) && float.TryParse(components[3], out float w))
            {
                return new Vector4(x, y, z, w);
            }
            else
            {
                LogService.Error(kTag, $"Failed to parse vector string: {vectorString}");
                return Vector4.zero;
            }
        }

        public static string[] SafeSplit(this string text, char separator)
        {
            if (string.IsNullOrEmpty(text)) return new string[0];
            if (!text.Contains(separator))
            {
                LogService.Warning(kTag, $"'{text}' does not contain '{separator}'");
                return new string[] { text };
            }
            return text.Split(separator);
        }

        public static bool Search(this string text, string keyword)
        {
            if (string.IsNullOrEmpty(text)) return false;
            if (string.IsNullOrEmpty(keyword)) return false;
            return text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static StringBuilder AppendBlue(this StringBuilder builder, string text)
        {
            return builder.Append($"<color=blue>{text}</color>");
        }

        public static StringBuilder AppendRed(this StringBuilder builder, string text)
        {
            return builder.Append($"<color=red>{text}</color>");
        }

        public static StringBuilder AppendGreen(this StringBuilder builder, string text)
        {
            return builder.Append($"<color=green>{text}</color>");
        }

        public static string FixSpaces(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // 대문자 뒤에 공백이 없는 경우 공백 추가
            // 하지만 대문자가 2번이상 연속되는 경우는 공백 추가하지 않음

            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (char.IsUpper(c))
                    {
                        if (i > 0 && text[i - 1] != ' ' && (i == text.Length - 1 || text[i + 1] != ' ')) sb.Append(' ');
                    }
                    sb.Append(c);
                }

                return sb.ToString();
            }
        }

        public static string JoinWithSpace(this string[] texts)
        {
            if (texts.IsNullOrEmpty()) return string.Empty;
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    sb.Append(texts[i]);
                    if (i < texts.Length - 1) sb.Append(' ');
                }
                return sb.ToString();
            }
        }

        public static string RemovePrefix(this string text, string prefix)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(prefix)) return text;

            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return text[prefix.Length..];
            }

            return text;
        }

        public static string GetLastSegment(this string text, char separator = '/')
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // if separator is '/', it can also be a backslash '\'
            if (separator == '/') text = text.Replace('\\', '/');

            int lastIndex = text.LastIndexOf(separator);
            if (lastIndex < 0 || lastIndex == text.Length - 1)
            {
                return text; // No segment found or the last character is the separator
            }

            return text[(lastIndex + 1)..];
        }

        public static string[] ToArray(this string text)
        {
            return string.IsNullOrEmpty(text) ? Array.Empty<string>() : new[] { text };
        }

        public static string OrDefault(this string value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }
}