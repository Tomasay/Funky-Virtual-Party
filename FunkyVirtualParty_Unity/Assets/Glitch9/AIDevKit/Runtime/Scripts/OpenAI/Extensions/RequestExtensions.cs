using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Extension methods for the all OpenAI requests that
    /// calls OpenAiClient's DefaultInstance to process the request.
    /// </summary>
    public static class RequestExtensions
    {
        public static UniTask<ChatCompletion> ExecuteAsync(this ChatCompletionRequest request)
            => OpenAI.DefaultInstance.Chat.Completions.CreateAsync(request);

        public static UniTask StreamAsync(this ChatCompletionRequest request)
            => OpenAI.DefaultInstance.Chat.Completions.StreamAsync(request);

        public static UniTask<GeneratedImage> ExecuteAsync(this ImageCreationRequest request)
            => OpenAI.DefaultInstance.Images.CreateAsync(request);

        public static UniTask<GeneratedImage> ExecuteAsync(this ImageEditRequest request)
            => OpenAI.DefaultInstance.Images.EditAsync(request);

        public static UniTask<GeneratedImage> ExecuteAsync(this ImageVariationRequest request)
            => OpenAI.DefaultInstance.Images.CreateVariationAsync(request);

        public static UniTask<GeneratedAudio> ExecuteAsync(this SpeechRequest request)
            => OpenAI.DefaultInstance.Audio.Speech.CreateAsync(request);

        public static UniTask StreamAsync(this SpeechRequest request, StreamingAudioPlayer streamingAudioPlayer)
            => OpenAI.DefaultInstance.Audio.Speech.StreamAsync(request, streamingAudioPlayer);

        public static UniTask<OpenAITranscript> ExecuteAsync(this TranscriptionRequest request)
            => OpenAI.DefaultInstance.Audio.Transcriptions.CreateAsync(request);

        public static UniTask StreamAsync(this TranscriptionRequest request)
            => OpenAI.DefaultInstance.Audio.Transcriptions.StreamAsync(request);

        public static UniTask<string> ExecuteAsync(this TranslationRequest request)
            => OpenAI.DefaultInstance.Audio.Translations.CreateAsync(request);

        public static UniTask<SafetyRating[]> ExecuteAsync(this ModerationRequest request)
            => OpenAI.DefaultInstance.Moderations.CreateAsync(request);

        public static UniTask<Embedding> ExecuteAsync(this EmbeddingRequest request)
            => OpenAI.DefaultInstance.Embeddings.CreateAsync(request);

        public static UniTask<FineTuningJob> ExecuteAsync(this FineTuningRequest request)
            => OpenAI.DefaultInstance.FineTuning.Jobs.CreateAsync(request);
    }
}