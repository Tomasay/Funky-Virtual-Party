using System.Linq;

namespace Glitch9.AIDevKit
{
    public class StructuredOutput<T> : CompletionResult<T> where T : class
    {
        public static implicit operator T(StructuredOutput<T> generatedObject) => generatedObject.values?.FirstOrDefault();
        public static implicit operator T[](StructuredOutput<T> generatedObject) => generatedObject.values;
        internal StructuredOutput(T response, Usage usage) : base(response, usage) { }
        internal StructuredOutput(T[] response, Usage usage) : base(response, usage) { }
        internal StructuredOutput(T value, ToolCall[] toolCalls, Usage usage = null) : base(value, toolCalls, usage) { }
        internal StructuredOutput(T[] values, ToolCall[] toolCalls, Usage usage = null) : base(values, toolCalls, usage) { }
    }
}
