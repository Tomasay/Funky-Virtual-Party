using System.Collections.Generic;
using System.Text;

namespace Glitch9.Editor.CodeGen
{
    internal static class CodeGenStringBuilderExtensions
    {
        const int kIndentSpace = 4;

        public static StringBuilder AppendLineWithIndent(this StringBuilder sb, string value, int indentLevel, int? spaces)
        {
            sb.Append(' ', spaces ?? kIndentSpace * indentLevel);
            sb.AppendLine(value);
            return sb;
        }

        public static StringBuilder AppendWithIndent(this StringBuilder sb, string value, int indentLevel, int? spaces)
        {
            sb.Append(' ', spaces ?? kIndentSpace * indentLevel);
            sb.Append(value);
            return sb;
        }

        public static StringBuilder AppendComments(this StringBuilder sb, List<CodeGenComment> comments, int indentLevel, int? spaces)
        {
            if (!comments.IsNullOrEmpty())
            {
                foreach (var comment in comments)
                {
                    sb.AppendLine(comment.Build(indentLevel, spaces));
                }
            }
            return sb;
        }

        public static StringBuilder AppendAttributes(this StringBuilder sb, List<CodeGenAttribute> attributes, int indentLevel, int? spaces)
        {
            if (!attributes.IsNullOrEmpty())
            {
                foreach (var attribute in attributes)
                {
                    sb.AppendLine(attribute.Build(indentLevel, spaces));
                }
            }
            return sb;
        }
    }
}