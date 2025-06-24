using System;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    internal static class PromptRecordFactory
    {
        internal static PromptRecord Create(IGENTask task, IGeneratedResult result)
        {
            if (ShouldIgnoreThisTask(task)) return null;

            var requestType = ResolveRequestType(task);

            var record = new PromptRecord()
                .InitializeCommon(requestType, task.model, task.sender, result.Usage, task.n)
                .SetPromptText(ResolvePromptText(task))
                .SetOutputMimeType(task.outputMimeType)
                .SetRequestOptions(task.options);

            if (TryGetInputFiles(task, out var inputFiles))
                record.SetInputFiles(inputFiles);

            if (TryGetOutputTexts(result, out var outputTexts))
                record.SetOutputTexts(outputTexts);

            if (TryGetOutputFiles(result, out var outputFiles))
                record.SetOutputFiles(outputFiles);

            if (task is GENSpeechTask speechTask)
            {
                if (!string.IsNullOrEmpty(speechTask.voiceId))
                {
                    record.AddRequestOption("voice_id", speechTask.voiceId);
                    Voice voice = speechTask.voiceId;
                    if (voice != null) record.AddRequestOption("voice", voice.Name);
                }
                record.AddRequestOption("speed", speechTask.speed);
                record.AddRequestOption("seed", speechTask.seed);
            }

            return record.SaveToDatabase();
        }

        private static bool ShouldIgnoreThisTask(IGENTask task)
        {
            return task == null || task.isWrapperTask;
        }

        // this is for streaming chat completions
        internal static PromptRecord Create(GENResponseTask task, ChatCompletion result)
        {
            var r = new PromptRecord()
                .InitializeCommon(RequestType.ChatCompletion, task.model, task.sender, result.Usage, task.n)
                .SetPromptText(task.prompt)
                .SetOutputTexts(result.ToStringArray())
                .SetInputFiles(task.attachedFiles.ToArray())
                //.SetOutputFiles  > TODO: Add output files
                .SetRequestOptions(task._options);

            return r.SaveToDatabase();
        }

        private static string ResolveRequestType(IGENTask task)
        {
            return task switch
            {
                GENResponseTask => RequestType.ChatCompletion,
                GENImageTask => RequestType.Image,
                GENInpaintTask => RequestType.ImageInpaint,
                //GENImageVariationTask => EndpointType.ImageVariation,
                GENSpeechTask => RequestType.Speech,
                GENVideoTask => RequestType.Video,
                GENSoundEffectTask => RequestType.SoundEffect,
                GENModerationTask => RequestType.Moderation,
                GENAudioIsolationTask => RequestType.AudioIsolation,
                GENVoiceChangeTask => RequestType.VoiceChange,
                GENCodeTask => RequestType.CodeGeneration,
                _ => throw new NotSupportedException($"PromptRecordFactory does not support {task.GetType().Name} tasks.")
            };
        }

        private static Prompt ResolvePromptText(IGENTask task)
        {
            return task switch
            {
                GENResponseTask r => r.prompt,
                GENCodeTask r => r.prompt,
                GENImageTask r => r.prompt,
                GENInpaintTask r => r.prompt,
                GENSpeechTask r => r.prompt,
                GENVideoTask r => r.prompt,
                GENSoundEffectTask r => r.prompt,
                // GENImageVariationTask r => r.prompt,
                // GENAudioIsolationTask r => r.prompt,
                // GENVoiceChangeTask r => r.prompt,
                GENModerationTask r => r.prompt,
                _ => null
            };
        }

        private static bool TryGetInputFiles(IGENTask task, out IFile[] files)
        {
            files = task switch
            {
                GENInpaintTask i => new IFile[] { new File<Texture2D>(i.prompt.image) },
                GENResponseTask r => r.attachedFiles?.ToArray(),
                _ => null
            };
            return files != null && files.Length > 0;
        }

        private static bool TryGetOutputTexts(IGeneratedResult result, out string[] texts)
        {
            texts = result switch
            {
                GeneratedText r => r,
                ChatCompletion r => r.ToStringArray(),
                Transcript r => new[] { r.ToString() },
                _ => null
            };
            return texts != null && texts.Length > 0;
        }

        private static bool TryGetOutputFiles(IGeneratedResult result, out IFile[] files)
        {
            files = result switch
            {
                GeneratedImage r => r.ToFiles(),
                GeneratedAudio r => r.ToFiles(),
                GeneratedVideo r => r.ToFiles(),
                _ => null
            };
            return files != null && files.Length > 0;
        }
    }
}