using System.Linq;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Represents a generated text result from an AI model.
    /// </summary>
    public class GeneratedText : CompletionResult<string>
    {
        public static implicit operator string(GeneratedText GeneratedText) => GeneratedText?.values?.FirstOrDefault();
        public static implicit operator string[](GeneratedText GeneratedText) => GeneratedText?.values;

        internal GeneratedText(string response, Usage usage) : base(response, usage) { }
        internal GeneratedText(string[] response, Usage usage) : base(response, usage) { }
        internal GeneratedText(string value, ToolCall[] toolCalls, Usage usage = null) : base(value, toolCalls, usage) { }
        internal GeneratedText(string[] values, ToolCall[] toolCalls, Usage usage = null) : base(values, toolCalls, usage) { }
    }
}