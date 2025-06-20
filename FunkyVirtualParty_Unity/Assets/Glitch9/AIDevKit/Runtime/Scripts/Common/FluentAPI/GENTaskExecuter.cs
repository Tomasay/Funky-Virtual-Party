using System;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.GENTasks
{
    internal abstract class GENTaskExecuter
    {
        internal abstract Api Api { get; }
        internal virtual UniTask<ChatCompletion> GenerateResponseAsync(GENResponseTask task, Type jsonSchemaType) => throw new InvalidEndpointException(Api, EndpointType.ChatCompletion);
        internal virtual UniTask<GeneratedImage> GenerateImageAsync(GENImageTask task) => throw new InvalidEndpointException(Api, EndpointType.ImageCreation);
        internal virtual UniTask<GeneratedImage> GenerateInpaintAsync(GENInpaintTask task) => throw new InvalidEndpointException(Api, EndpointType.ImageEdit);
        internal virtual UniTask<GeneratedAudio> GenerateSpeechAsync(GENSpeechTask task) => throw new InvalidEndpointException(Api, EndpointType.Speech);
        internal virtual UniTask<Transcript> GenerateTranscriptAsync(GENTranscriptTask task) => throw new InvalidEndpointException(Api, EndpointType.Transcript);
        internal virtual UniTask<GeneratedAudio> GenerateSoundEffectAsync(GENSoundEffectTask task) => throw new InvalidEndpointException(Api, EndpointType.SoundEffect);
        internal virtual UniTask<GeneratedAudio> GenerateVoiceChangeAsync(GENVoiceChangeTask task) => throw new InvalidEndpointException(Api, EndpointType.VoiceChange);
        internal virtual UniTask<GeneratedAudio> GenerateAudioIsolationAsync(GENAudioIsolationTask task) => throw new InvalidEndpointException(Api, EndpointType.AudioIsolation);
        internal virtual UniTask<GeneratedVideo> GenerateVideoAsync(GENVideoTask task) => throw new InvalidEndpointException(Api, EndpointType.Video);
        internal virtual UniTask<SafetyRating[]> GenerateModerationAsync(GENModerationTask task) => throw new InvalidEndpointException(Api, EndpointType.Moderation);

        // Streaming
        internal virtual UniTask StreamResponseAsync(GENResponseTask task, Type jsonSchemaType, IChatCompletionStreamHandler streamHandler) => throw new InvalidEndpointException(Api, EndpointType.ChatCompletion);
        internal virtual UniTask StreamSpeechAsync(GENSpeechTask task, RealtimeAudioPlayer streamAudioPlayer) => throw new InvalidEndpointException(Api, EndpointType.Speech);

        // OpenAI Legacy
        //internal virtual UniTask<GeneratedImage> GenerateImageVariationAsync(GENImageVariationTask task) => throw new InvalidEndpointException(Api, EndpointType.ImageVariation);
        internal virtual UniTask<Transcript> GenerateTranslationAsync(GENTranslationTask task) => throw new InvalidEndpointException(Api, EndpointType.Translation);

        // Internal
        internal virtual UniTask<QueryResponse<IModelData>> ListModelsAsync(Query query) => throw new InvalidEndpointException(Api, EndpointType.ListModels);
        internal virtual UniTask<QueryResponse<IVoiceData>> ListVoicesAsync(Query query) => throw new InvalidEndpointException(Api, EndpointType.ListVoices);
        internal virtual UniTask<QueryResponse<IModelData>> ListCustomModelsAsync(Query query) => throw new InvalidEndpointException(Api, EndpointType.ListCustomModels);
        internal virtual UniTask<QueryResponse<IVoiceData>> ListCustomVoicesAsync(Query query) => throw new InvalidEndpointException(Api, EndpointType.ListCustomVoices);
        internal virtual UniTask<IModelData> RetrieveModelAsync(string modelId) => throw new InvalidEndpointException(Api, EndpointType.RetrieveModel);
        internal virtual UniTask<bool> DeleteModelAsync(string modelId) => throw new InvalidEndpointException(Api, EndpointType.DeleteModel);
    }
}