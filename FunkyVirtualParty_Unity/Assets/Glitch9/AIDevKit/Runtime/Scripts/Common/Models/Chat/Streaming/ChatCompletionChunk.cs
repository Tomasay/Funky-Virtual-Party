
namespace Glitch9.AIDevKit
{
    public class ChatCompletionChunk
    {
        internal ChatCompletion Value;
        internal bool isDone;
        internal bool isError;
        internal string errorMessage;
        internal Usage Usage => Value?.Usage;

        internal static ChatCompletionChunk Chunk(ChatCompletion chunk)
        {
            return new ChatCompletionChunk
            {
                Value = chunk,
            };
        }

        internal static ChatCompletionChunk Error(string error)
        {
            return new ChatCompletionChunk
            {
                isError = true,
                errorMessage = error ?? "Unknown error",
            };
        }

        internal static ChatCompletionChunk Done()
        {
            return new ChatCompletionChunk
            {
                isDone = true,
            };
        }
    }
}