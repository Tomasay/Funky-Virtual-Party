using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.Ollama
{
    /// <summary>
    /// Extension methods for the all OpenAI requests that
    /// calls OpenAiClient's DefaultInstance to process the request.
    /// </summary>
    public static class RequestExtensions
    {
        public static async UniTask<GenerateResponse> ExecuteAsync(this CompletionRequest request)
        {
            return await Ollama.DefaultInstance.Generate.CreateAsync(request);
        }

        public static async UniTask StreamAsync(this CompletionRequest request, IChatCompletionStreamHandler streamHandler)
        {
            await Ollama.DefaultInstance.Generate.StreamAsync(request, streamHandler);
        }

        public static async UniTask<ChatResponse> ExecuteAsync(this ChatCompletionRequest request)
        {
            return await Ollama.DefaultInstance.Chat.CreateAsync(request);
        }

        public static async UniTask StreamAsync(this ChatCompletionRequest request, IChatCompletionStreamHandler listener)
        {
            await Ollama.DefaultInstance.Chat.StreamAsync(request, listener);
        }

        public static async UniTask StreamAsync(this CreateModelRequest req, IChatCompletionStreamHandler streamHandler)
        {
            await Ollama.DefaultInstance.Models.CreateModelAsync(req, streamHandler);
        }
    }
}