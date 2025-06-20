using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.GENTasks
{
    /// <summary>
    /// Fluent API for generating text and images using all generative AI services supported by AIDevKit.
    /// </summary>
    internal static partial class GENTaskManager
    {
        private static readonly Dictionary<Api, GENTaskExecuter> _taskExecuters = new();

        #region Global Internal Method [IMPORTANT]

        // Register executers from each assembly by 
        // - Editor: using static constructor 
        // - Runtime: using [UnityEngine.RuntimeInitializeOnLoadMethod]
        internal static void RegisterTaskExecuter(Api provider, GENTaskExecuter executer)
        {
            if (_taskExecuters.ContainsKey(provider))
            {
                Debug.LogWarning($"Task executer for {provider} is already registered.");
                return;
            }

            _taskExecuters.Add(provider, executer);
        }

        #endregion

        #region Utility Method
        private static GENTaskExecuter GetTaskExecuter(Api provider)
        {
            if (_taskExecuters.TryGetValue(provider, out var executer)) return executer;

            throw new NotImplementedException(
                $"No task executer found for provider {provider}." +
                $"\n - The module for {provider} may not be implemented yet." +
                $"\n - You don't have the {provider} module installed." +
                $"\n - It's some unexpected error.");
        }

        #endregion

        internal static async UniTask<ChatCompletion> GenerateContentAsync(GENResponseTask task, Type jsonSchemaType)
        {
            await task.LoadAllAttachedFilesAsync();
            await GENTaskUtil.ThrowIfBlockedAsync(task);
            Api api = GENTaskUtil.ResolveLLMApi(task);
            GENTaskExecuter executer = GetTaskExecuter(api);
            ChatCompletion result = await executer.GenerateResponseAsync(task, jsonSchemaType);
            if (task.textProcessor != null) result.ProcessText(task.textProcessor);
            //if (GENTaskUtil.IsCreatingHistory(task)) PromptRecordFactory.Create(task, result);
            return result;
        }

        internal static async UniTask StreamContentAsync(GENResponseTask task, Type jsonSchemaType, IChatCompletionStreamHandler streamHandler)
        {
            await task.LoadAllAttachedFilesAsync();
            await GENTaskUtil.ThrowIfBlockedAsync(task);
            Api api = GENTaskUtil.ResolveLLMApi(task);
            GENTaskExecuter executer = GetTaskExecuter(api);
            await executer.StreamResponseAsync(task, jsonSchemaType, streamHandler);
        }

        internal static async UniTask<GeneratedImage> GenerateImageAsync(GENImageTask task)
        {
            Api api = GENTaskUtil.ResolveIMGApi(task);
            task.ResolveOutputPath();
            GENTaskExecuter executer = GetTaskExecuter(api);
            GeneratedImage result = await executer.GenerateImageAsync(task);
            //if (GENTaskUtil.IsCreatingHistory(task)) PromptRecordFactory.Create(task, result);
            return result;
        }

        internal static async UniTask<GeneratedImage> GenerateImageEditAsync(GENInpaintTask task)
        {
            Api api = GENTaskUtil.ResolveIMGApi(task);
            task.ResolveOutputPath();
            GENTaskExecuter executer = GetTaskExecuter(api);
            GeneratedImage result = await executer.GenerateInpaintAsync(task);
            //if (GENTaskUtil.IsCreatingHistory(task)) PromptRecordFactory.Create(task, result);
            return result;
        }

        // internal static async UniTask<GeneratedImage> GenerateImageVariationAsync(GENImageVariationTask task)
        // {
        //     Api api = GENTaskUtil.ResolveIMGApi(task);
        //     task.ResolveOutputPath();
        //     GENTaskExecuter executer = GetTaskExecuter(api);
        //     GeneratedImage result = await executer.GenerateImageVariationAsync(task);
        //     //if (GENTaskUtil.IsCreatingHistory(task)) PromptRecordFactory.Create(task, result);
        //     return result;
        // }

        internal static async UniTask<GeneratedAudio> GenerateSpeechAsync(GENSpeechTask task)
        {
            Api api = task.api ?? GENTaskUtil.ResolveTTSApi(task);
            task.ResolveOutputPath();
            GENTaskExecuter executer = GetTaskExecuter(api);
            GeneratedAudio result = await executer.GenerateSpeechAsync(task);
            //if (GENTaskUtil.IsCreatingHistory(task)) PromptRecordFactory.Create(task, result);
            return result;
        }

        internal static async UniTask StreamSpeechAsync(GENSpeechTask task, RealtimeAudioPlayer streamAudioPlayer)
        {
            Api api = task.api ?? GENTaskUtil.ResolveTTSApi(task);
            GENTaskExecuter executer = GetTaskExecuter(api);
            await executer.StreamSpeechAsync(task, streamAudioPlayer);
        }

        internal static async UniTask<Transcript> GenerateTranscriptAsync(GENTranscriptTask task)
        {
            Api api = GENTaskUtil.ResolveSTTApi(task);
            GENTaskExecuter executer = GetTaskExecuter(api);
            Transcript result = await executer.GenerateTranscriptAsync(task);
            return result;
        }

        internal static async UniTask<GeneratedText> GenerateTranslationAsync(GENTranslationTask task)
        {
            Api api = GENTaskUtil.ResolveSTTApi(task);
            GENTaskExecuter executer = GetTaskExecuter(api);
            Transcript result = await executer.GenerateTranslationAsync(task);
            return new GeneratedText(result.Text, result.Usage);
        }

        internal static async UniTask<GeneratedAudio> GenerateSoundEffectAsync(GENSoundEffectTask task)
        {
            task.ResolveOutputPath(Api.ElevenLabs, "sfx");
            GENTaskExecuter executer = GetTaskExecuter(Api.ElevenLabs);
            return await executer.GenerateSoundEffectAsync(task);
        }

        internal static async UniTask<GeneratedAudio> GenerateVoiceChangeAsync(GENVoiceChangeTask task)
        {
            if (task.prompt == null) throw new ArgumentNullException(nameof(task.prompt), "Input audio clip is null.");
            task.ResolveOutputPath(Api.ElevenLabs, "voice_change");
            GENTaskExecuter executer = GetTaskExecuter(Api.ElevenLabs);
            return await executer.GenerateVoiceChangeAsync(task);
        }

        internal static async UniTask<GeneratedAudio> GenerateAudioIsolationAsync(GENAudioIsolationTask task)
        {
            if (task.prompt == null) throw new ArgumentNullException(nameof(task.prompt), "Input audio clip is null.");
            task.ResolveOutputPath(Api.ElevenLabs, "isolation");
            GENTaskExecuter executer = GetTaskExecuter(Api.ElevenLabs);
            return await executer.GenerateAudioIsolationAsync(task);
        }

        // Added new on 2025.05.14
        internal static async UniTask<Moderation> GenerateModerationAsync(GENModerationTask task)
        {
            Api api = GENTaskUtil.ResolveMODApi(task);
            GENTaskExecuter executer = GetTaskExecuter(api);
            SafetyRating[] ratings = await executer.GenerateModerationAsync(task);
            return new Moderation(ratings);
        }

        // Added new on 2025.05.05
        internal static async UniTask<GeneratedVideo> GenerateVideoAsync(GENVideoTask task)
        {
            GENTaskExecuter executer = GetTaskExecuter(Api.Google);
            GeneratedVideo video = await executer.GenerateVideoAsync(task);
            //if (GENTaskUtil.IsCreatingHistory(task)) PromptRecordFactory.Create(task, video);
            return video;
        }

        // Added new on 2025.05.11
        internal static async UniTask<QueryResponse<IModelData>> ListModelsAsync(Api api, Query query = null)
        {
            GENTaskExecuter executer = GetTaskExecuter(api);
            return await executer.ListModelsAsync(query);
        }

        internal static async UniTask<QueryResponse<IModelData>> ListCustomModelsAsync(Api api, Query query = null)
        {
            GENTaskExecuter executer = GetTaskExecuter(api);
            return await executer.ListCustomModelsAsync(query);
        }

        internal static async UniTask<QueryResponse<IVoiceData>> ListVoicesAsync(Api api, Query query = null)
        {
            GENTaskExecuter executer = GetTaskExecuter(api);
            return await executer.ListVoicesAsync(query);
        }

        internal static async UniTask<QueryResponse<IVoiceData>> ListCustomVoicesAsync(Api api, Query query = null)
        {
            GENTaskExecuter executer = GetTaskExecuter(api);
            return await executer.ListCustomVoicesAsync(query);
        }

        // Added new on 2025.05.29
        internal static async UniTask<IModelData> RetrieveModelAsync(Api api, string modelId)
        {
            if (string.IsNullOrWhiteSpace(modelId)) throw new ArgumentException("Model ID cannot be null or empty.", nameof(modelId));
            GENTaskExecuter executer = GetTaskExecuter(api);
            return await executer.RetrieveModelAsync(modelId);
        }

        internal static async UniTask<bool> DeleteModelAsync(Api api, string modelId)
        {
            if (string.IsNullOrWhiteSpace(modelId)) throw new ArgumentException("Model ID cannot be null or empty.", nameof(modelId));
            GENTaskExecuter executer = GetTaskExecuter(api);
            return await executer.DeleteModelAsync(modelId);
        }
    }
}