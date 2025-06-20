using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Extension methods for the all OpenAI requests that
    /// calls OpenAiClient's DefaultInstance to process the request.
    /// </summary>
    public static class RequestExtensions
    {
        public static UniTask<ChatCompletion> ExecuteAsync(this ChatCompletionRequest request)
        {
            return OpenAI.DefaultInstance.Chat.Completions.CreateAsync(request);
        }

        public static UniTask StreamAsync(this ChatCompletionRequest request, IChatCompletionStreamHandler streamHandler)
        {
            return OpenAI.DefaultInstance.Chat.Completions.StreamAsync(request, streamHandler);
        }

        public static UniTask<GeneratedImage> ExecuteAsync(this ImageCreationRequest request)
        {
            return OpenAI.DefaultInstance.Images.CreateAsync(request);
        }

        public static UniTask<GeneratedImage> ExecuteAsync(this ImageEditRequest request)
        {
            return OpenAI.DefaultInstance.Images.EditAsync(request);
        }

        public static UniTask<GeneratedImage> ExecuteAsync(this ImageVariationRequest request)
        {
            return OpenAI.DefaultInstance.Images.CreateVariationAsync(request);
        }

        public static UniTask<GeneratedAudio> ExecuteAsync(this SpeechRequest request)
        {
            return OpenAI.DefaultInstance.Audio.Speech.CreateAsync(request);
        }

        public static UniTask<OpenAITranscript> ExecuteAsync(this TranscriptionRequest request)
        {
            return OpenAI.DefaultInstance.Audio.Transcriptions.CreateAsync(request);
        }

        public static UniTask<string> ExecuteAsync(this TranslationRequest request)
        {
            return OpenAI.DefaultInstance.Audio.Translations.CreateAsync(request);
        }

        public static UniTask<SafetyRating[]> ExecuteAsync(this ModerationRequest request)
        {
            return OpenAI.DefaultInstance.Moderations.CreateAsync(request);
        }

        public static UniTask<Embedding> ExecuteAsync(this EmbeddingRequest request)
        {
            return OpenAI.DefaultInstance.Embeddings.CreateAsync(request);
        }

        public static UniTask<FineTuningJob> ExecuteAsync(this FineTuningRequest request)
        {
            return OpenAI.DefaultInstance.FineTuning.Jobs.CreateAsync(request);
        }
    }
}