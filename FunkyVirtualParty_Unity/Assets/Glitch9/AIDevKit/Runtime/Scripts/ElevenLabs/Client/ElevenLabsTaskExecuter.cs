using System.IO;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal class ElevenLabsTaskExecuter : GENTaskExecuter
    {
#if UNITY_EDITOR
        static ElevenLabsTaskExecuter()
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ResisterTaskExecuter()
#endif
        {
            GENTaskManager.RegisterTaskExecuter(Api.ElevenLabs, new ElevenLabsTaskExecuter());
        }

        internal override Api Api => Api.ElevenLabs;

        internal override async UniTask<GeneratedAudio> GenerateSpeechAsync(GENSpeechTask task)
        {
            if (task.model == null) task.model = ElevenLabsSettings.DefaultTTS;
            if (task.voiceId == null) task.voiceId = ElevenLabsSettings.DefaultVoice;

            VoiceSettings settings = null;

            if (task.speed != null)
            {
                settings = new VoiceSettings
                {
                    Speed = task.speed.Value,
                };
            }

            var outputFormat = task.GetOutputFormat();

            SpeechRequest request = new()
            {
                Sender = task.sender,
                Text = task.prompt,
                OutputPath = FixOutputPath(outputFormat, task._outputPath),
                Model = task.model,
                Voice = task.voiceId,
                Seed = task.seed,
                OutputFormat = outputFormat,
                VoiceSettings = settings,
                Token = task.token,
                IgnoreLogs = task._ignoreLogs,
            };

            return await request.ExecuteAsync();
        }

        internal override async UniTask StreamSpeechAsync(GENSpeechTask task, RealtimeAudioPlayer streamAudioPlayer)
        {
            if (task.model == null) task.model = ElevenLabsSettings.DefaultTTS;
            if (task.voiceId == null) task.voiceId = ElevenLabsSettings.DefaultVoice;

            VoiceSettings settings = null;

            if (task.speed != null)
            {
                settings = new VoiceSettings
                {
                    Speed = task.speed.Value,
                };
            }

            SpeechRequest speechRequest = new()
            {
                Sender = task.sender,
                Text = task.prompt,
                Model = task.model,
                Voice = task.voiceId,
                Seed = task.seed,
                OutputFormat = task.GetOutputFormat(),
                VoiceSettings = settings,
                Token = task.token,
                IgnoreLogs = task._ignoreLogs,
            };

            await speechRequest.StreamAsync(streamAudioPlayer);
        }

        internal override async UniTask<Transcript> GenerateTranscriptAsync(GENTranscriptTask task)
        {
            TranscriptRequest request = new()
            {
                Sender = task.sender,
                File = task.prompt.EncodeToPCM16(),
                OutputPath = task._outputPath,
                Model = task.model,
                IgnoreLogs = task._ignoreLogs,
            };

            return await request.ExecuteAsync();
        }


        internal override async UniTask<GeneratedAudio> GenerateSoundEffectAsync(GENSoundEffectTask task)
        {
            // No model can be set for this task.
            var outputFormat = task.GetOutputFormat();

            SoundEffectRequest request = new()
            {
                Sender = task.sender,
                Text = task.prompt,
                OutputPath = FixOutputPath(outputFormat, task._outputPath),
                OutputFormat = outputFormat,
                DurationSeconds = task.durationSeconds,
                PromptInfluence = task.promptInfluence,
                Token = task.token,
                IgnoreLogs = task._ignoreLogs,
            };

            return await request.ExecuteAsync();
        }

        internal override async UniTask<GeneratedAudio> GenerateVoiceChangeAsync(GENVoiceChangeTask task)
        {
            // No model can be set for this task. 
            var outputFormat = task.GetOutputFormat();

            VoiceChangerRequest request = new()
            {
                Sender = task.sender,
                Audio = task.prompt.EncodeToPCM16(),
                OutputPath = FixOutputPath(outputFormat, task._outputPath),
                OutputFormat = outputFormat,
                Seed = task.seed,
                RemoveBackgroundNoise = task.removeBackgroundNoise,
                Token = task.token,
                IgnoreLogs = task._ignoreLogs,
            };

            return await request.ExecuteAsync();
        }

        internal override async UniTask<GeneratedAudio> GenerateAudioIsolationAsync(GENAudioIsolationTask task)
        {
            // No model can be set for this task.
            var outputFormat = task.GetOutputFormat();

            AudioIsolationRequest request = new()
            {
                Sender = task.sender,
                Audio = task.prompt.EncodeToPCM16(),
                OutputPath = FixOutputPath(outputFormat, task._outputPath),
                OutputFormat = outputFormat,
                Token = task.token,
                IgnoreLogs = task._ignoreLogs,
            };

            return await request.ExecuteAsync();
        }

        private static string FixOutputPath(ElevenLabsOutputFormat? outputFormat, string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath)) return null;

            ElevenLabsOutputFormat format = outputFormat ?? ElevenLabsOutputFormat.MP3_44100_32;

            string extension = format.GetFileExtension();
            if (string.IsNullOrEmpty(extension)) return outputPath;

            // replace existing extension with the new one
            string currentExtension = Path.GetExtension(outputPath);

            if (!string.IsNullOrEmpty(currentExtension) && currentExtension != extension)
            {
                outputPath = outputPath.Replace(currentExtension, extension);
            }
            else if (string.IsNullOrEmpty(currentExtension))
            {
                outputPath += extension;
            }

            return outputPath;
        }

        internal override async UniTask<QueryResponse<IModelData>> ListModelsAsync(Query query = null)
        {
            QueryResponse<ElevenLabsModelData> res = await ElevenLabs.DefaultInstance.Models.ListAsync(query as ElevenLabsQuery);
            return res.ToSoftRef<ElevenLabsModelData, IModelData>();
        }

        internal override async UniTask<QueryResponse<IVoiceData>> ListVoicesAsync(Query query = null)
        {
            QueryResponse<ElevenLabsVoiceData> res = await ElevenLabs.DefaultInstance.Voices.ListAsync(query as ElevenLabsQuery);
            return res.ToSoftRef<ElevenLabsVoiceData, IVoiceData>();
        }

        internal override async UniTask<QueryResponse<IVoiceData>> ListCustomVoicesAsync(Query query = null)
        {
            QueryResponse<ElevenLabsSharedVoiceData> res = await ElevenLabs.DefaultInstance.VoiceLibrary.ListAsync(query as ElevenLabsQuery);
            return res.ToSoftRef<ElevenLabsSharedVoiceData, IVoiceData>();
        }
    }
}