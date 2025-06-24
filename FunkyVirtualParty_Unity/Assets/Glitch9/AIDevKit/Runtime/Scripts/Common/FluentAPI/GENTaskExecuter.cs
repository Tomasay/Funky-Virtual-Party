using System;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.GENTasks
{
    internal abstract class GENTaskExecuter
    {
        internal abstract Api Api { get; }

        internal virtual UniTask<ChatCompletion> GenerateResponseAsync(GENResponseTask task, Type jsonSchemaType)
        => throw new NotSupportedEndpointException(Api, RequestType.ChatCompletion);

        internal virtual UniTask<GeneratedImage> GenerateImageAsync(GENImageTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.Image);

        internal virtual UniTask<GeneratedImage> GenerateInpaintAsync(GENInpaintTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.ImageInpaint);

        internal virtual UniTask<GeneratedAudio> GenerateSpeechAsync(GENSpeechTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.Speech);

        internal virtual UniTask<Transcript> GenerateTranscriptAsync(GENTranscriptTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.Transcript);

        internal virtual UniTask<GeneratedAudio> GenerateSoundEffectAsync(GENSoundEffectTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.SoundEffect);

        internal virtual UniTask<GeneratedAudio> GenerateVoiceChangeAsync(GENVoiceChangeTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.VoiceChange);

        internal virtual UniTask<GeneratedAudio> GenerateAudioIsolationAsync(GENAudioIsolationTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.AudioIsolation);

        internal virtual UniTask<GeneratedVideo> GenerateVideoAsync(GENVideoTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.Video);

        internal virtual UniTask<SafetyRating[]> GenerateModerationAsync(GENModerationTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.Moderation);

        // Streaming
        internal virtual UniTask StreamResponseAsync(GENResponseTask task, Type jsonSchemaType, ChatCompletionStreamHandler streamHandler)
        => throw new NotSupportedEndpointException(Api, RequestType.ChatCompletionStream);

        internal virtual UniTask StreamSpeechAsync(GENSpeechTask task, StreamingAudioPlayer streamAudioPlayer)
        => throw new NotSupportedEndpointException(Api, RequestType.SpeechStream);

        internal virtual UniTask StreamTranscriptAsync(GENTranscriptTask task, TranscriptStreamHandler streamHandler)
        => throw new NotSupportedEndpointException(Api, RequestType.TranscriptStream);

        // OpenAI Legacy
        //internal virtual UniTask<GeneratedImage> GenerateImageVariationAsync(GENImageVariationTask task) 
        //=> throw new InvalidEndpointException(Api, EndpointType.ImageVariation);
        internal virtual UniTask<Transcript> GenerateTranslationAsync(GENTranslationTask task)
        => throw new NotSupportedEndpointException(Api, RequestType.TranscriptTranslation);

        // Internal
        internal virtual UniTask<QueryResponse<IModelData>> ListModelsAsync(Query query)
        => throw new NotSupportedEndpointException(Api, RequestType.ListModels);

        internal virtual UniTask<QueryResponse<IVoiceData>> ListVoicesAsync(Query query)
        => throw new NotSupportedEndpointException(Api, RequestType.ListVoices);

        internal virtual UniTask<QueryResponse<IModelData>> ListCustomModelsAsync(Query query)
        => throw new NotSupportedEndpointException(Api, RequestType.ListCustomModels);

        internal virtual UniTask<QueryResponse<IVoiceData>> ListCustomVoicesAsync(Query query)
        => throw new NotSupportedEndpointException(Api, RequestType.ListCustomVoices);

        internal virtual UniTask<IModelData> RetrieveModelAsync(string modelId)
        => throw new NotSupportedEndpointException(Api, RequestType.RetrieveModel);

        internal virtual UniTask<bool> DeleteModelAsync(string modelId)
        => throw new NotSupportedEndpointException(Api, RequestType.DeleteModel);
    }
}