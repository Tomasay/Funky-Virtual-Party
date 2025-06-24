
namespace Glitch9.AIDevKit
{
    public abstract class AIDevKitChunk<T> where T : class
    {
        public T Value;
        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);
        public string ErrorMessage;
        public abstract bool IsDone { get; }
        public abstract Usage Usage { get; }
    }
}