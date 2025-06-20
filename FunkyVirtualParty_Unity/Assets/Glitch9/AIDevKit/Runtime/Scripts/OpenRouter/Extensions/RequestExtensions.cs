using System;
using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.OpenRouter
{
    /// <summary>
    /// Extension methods for the all OpenAI requests that
    /// calls OpenAiClient's DefaultInstance to process the request.
    /// </summary>
    public static class RequestExtensions
    {
        public static async UniTask<ChatCompletion> ExecuteAsync(this CompletionRequest request)
        {
            return await OpenRouter.DefaultInstance.Completion.CreateAsync(request);
        }

        public static async UniTask StreamAsync(this CompletionRequest request, IChatCompletionStreamHandler streamHandler)
        {
            await OpenRouter.DefaultInstance.Completion.Stream(request, streamHandler);
        }
        public static async UniTask<ChatCompletion> ExecuteAsync(this ChatCompletionRequest request)
        {
            return await OpenRouter.DefaultInstance.ChatCompletion.CreateAsync(request);
        }

        public static async UniTask StreamAsync(this ChatCompletionRequest request, IChatCompletionStreamHandler streamHandler)
        {
            await OpenRouter.DefaultInstance.ChatCompletion.Stream(request, streamHandler);
        }
    }
}