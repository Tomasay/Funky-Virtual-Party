using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.GENTasks
{
    internal static class GENTaskUtil
    {
        internal static Api ResolveApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task, string modelId)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult
        {
            if (task.model == null)
            {
                Model defaultModel = modelId;
                return defaultModel.Api;
            }
            return task.model.Api;
        }

        internal static Api ResolveApi(Model model)
        {
            if (model == null) return Api.None; // default to OpenAI if model is null.
            return model.Api;
        }

        internal static string ResolveKeyword(Model model)
        {
            if (model == null) return "unknown"; // default to unknown if model is null.
            return model.Id;
        }

        internal static Api ResolveLLMApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult => ResolveApi(task, AIDevKitSettings.DefaultLLM);

        internal static Api ResolveIMGApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult => ResolveApi(task, AIDevKitSettings.DefaultIMG);

        internal static Api ResolveTTSApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult => ResolveApi(task, AIDevKitSettings.DefaultTTS);

        internal static Api ResolveSTTApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult => ResolveApi(task, AIDevKitSettings.DefaultSTT);

        internal static Api ResolveEMBApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult => ResolveApi(task, AIDevKitSettings.DefaultEMB);

        internal static Api ResolveMODApi<TSelf, TPrompt, TResult>(GENTask<TSelf, TPrompt, TResult> task)
        where TSelf : GENTask<TSelf, TPrompt, TResult> where TResult : IGeneratedResult => ResolveApi(task, AIDevKitSettings.DefaultMOD);


        internal static bool TryResolveOutputPath(bool saveOutput, string outputPath, MIMEType outputMimeType, Api api, string keyword, out string resolvedOutputPath)
        {
            resolvedOutputPath = outputPath;

            if (!saveOutput) return false;

            if (string.IsNullOrWhiteSpace(resolvedOutputPath))
            {
                resolvedOutputPath = AIDevKitSettings.OutputPath;
            }

            bool hasExtension = Path.HasExtension(resolvedOutputPath);

            if (hasExtension)
            {
                // 사용자가 직접 파일명을 입력했음: 그대로 사용
                return true;
            }

            // 파일 확장자가 없으면, 디렉토리로 간주하고 파일명 생성
            string fileName = OutputPathResolver.ResolveOutputFileName(api, keyword, outputMimeType);
            if (string.IsNullOrEmpty(fileName)) return false;

            resolvedOutputPath = Path.Combine(resolvedOutputPath, fileName);

            AIDevKitDebug.Blue($"Output path: {resolvedOutputPath}");

            return !string.IsNullOrEmpty(resolvedOutputPath);
        }

        internal static bool IsCreatingHistory(IGENTask task)
        {
            if (task == null) return false;
            if (task.enableHistory) return true;
            if (!Application.isPlaying) return true;
            return AIDevKitSettings.PromptHistoryOnRuntime;
        }

        internal static async UniTask ThrowIfBlockedAsync(GENResponseTask task)
        {
            ModerationOptions modOptions = task.moderationOptions;
            if (modOptions != null && modOptions.IsValid)
            {
                if (task.prompt == null) throw new ArgumentNullException(nameof(task.prompt));

                Moderation moderation = await task.prompt
                    .GENModeration(modOptions.SafetySettings)
                    .Attach(task.attachedFiles.ToArray())
                    .ExecuteAsync();

                if (moderation == null || moderation.IsEmpty)
                {
                    throw new InvalidOperationException("Moderation result is empty.");
                }

                if (ModerationUtil.IsBlocked(modOptions.SafetySettings, moderation.Values))
                {
                    throw new BlockedPromptException(PromptFeedback.Safety(moderation.Values));
                }
            }
        }
    }
}