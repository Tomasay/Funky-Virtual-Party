
namespace Glitch9.AIDevKit
{
    public class ChatCompletionChunk : AIDevKitChunk<ChatCompletion>
    {
        public override bool IsDone => _isDone;
        public override Usage Usage => Value?.Usage;
        private bool _isDone;

        internal static ChatCompletionChunk Create(ChatCompletion chunk)
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
                ErrorMessage = error ?? "Unknown error",
            };
        }

        internal static ChatCompletionChunk Done(ChatCompletion chunk = null)
        {
            return new ChatCompletionChunk
            {
                Value = chunk,
                _isDone = true,
            };
        }
    }
}