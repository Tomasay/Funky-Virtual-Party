
namespace Glitch9.Editor
{
    public enum BlockType
    {
        Text,
        CodeBlock,
        Header,
        Quote,
        UList,
        Hyperlink,
    }

    public partial class TextBlock
    {
        public string language;
        public string content;
        public BlockType type;
        public int headerLevel;
        public string arg;
        public bool IsCopied { get; set; }
        public bool IsEmpty => string.IsNullOrEmpty(content);

        public static TextBlock CodeBlock(string language, string code)
        {
            return new TextBlock
            {
                language = language,
                content = code,
                type = BlockType.CodeBlock
            };
        }

        public static TextBlock Text(string text) // plain text
        {
            return new TextBlock
            {
                content = text,
                type = BlockType.Text,
            };
        }

        public static TextBlock Header(string content, int headerLevel = 0)
        {
            return new TextBlock
            {
                content = content,
                type = BlockType.Header,
                headerLevel = headerLevel
            };
        }

        public static TextBlock Quote(string content)
        {
            return new TextBlock
            {
                content = content,
                type = BlockType.Quote
            };
        }

        public static TextBlock UListHeader(string content)
        {
            return new TextBlock
            {
                content = content,
                type = BlockType.Header,
                headerLevel = 0
            };
        }

        public static TextBlock UList(string content)
        {
            return new TextBlock
            {
                content = content,
                type = BlockType.UList
            };
        }

        public static TextBlock Hyperlink(string content, string url)
        {
            return new TextBlock
            {
                content = content,
                type = BlockType.Hyperlink,
                arg = url
            };
        }
    }
}