using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal class GenerativeAITaskExecuter : GENTaskExecuter
    {
#if UNITY_EDITOR 
        static GenerativeAITaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.Google, new GenerativeAITaskExecuter());
        }
#else 
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ResisterTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.Google, new GenerativeAITaskExecuter());
        }
#endif

        internal override Api Api => Api.Google;

        internal override async UniTask<ChatCompletion> GenerateResponseAsync(GENResponseTask task, Type jsonSchemaType)
        {
            GenerateContentRequest req = CreateTextRequest(task, jsonSchemaType);
            GenerateContentResponse result = await req.ExecuteAsync();
            return ChatCompletionFactory.Create(result.GetChatChoices(), result.Usage);
        }

        internal override async UniTask StreamResponseAsync(GENResponseTask task, Type jsonSchemaType, IChatCompletionStreamHandler streamHandler)
        {
            GenerateContentRequest req = CreateTextRequest(task, jsonSchemaType);
            await req.StreamAsync(streamHandler);
        }

        internal override async UniTask<GeneratedImage> GenerateImageAsync(GENImageTask task)
        {
            if (task.model.Family == ModelFamily.Imagen)
            {
                PredictionRequest.Builder builder = new PredictionRequest.Builder()
                    .SetSender(task.sender)
                    .SetIgnoreLogs(task._ignoreLogs)
                    .SetModel(task.model.Id)
                    .SetPrompt(task.prompt)
                    .SetNumberOfImages(task.n)
                    .SetOutputPath(task._outputPath)
                    .SetCancellationToken(task.token);

                AspectRatio? aspectRatio = task.GetAspectRatio();
                PersonGeneration? personGeneration = task.GetPersonGeneration();

                if (aspectRatio != null) builder.SetAspectRatio(aspectRatio.Value);
                if (personGeneration != null) builder.SetPersonGeneration(personGeneration.Value);

                PredictionResponse result = await builder.Build().GenerateImageAsync();

                if (result == null || result.GeneratedImages.IsNullOrEmpty()) throw new EmptyResponseException(task.model);

                return await result.ToGeneratedImageAsync(task._outputPath);
            }
            else
            {
                GenerateContentRequest.Builder builder = new GenerateContentRequest.Builder()
                    .SetSender(task.sender)
                    .SetIgnoreLogs(task._ignoreLogs)
                    .SetModel(task.model.Id)
                    .SetPrompt(task.prompt)
                    .SetResponseModalities(Modality.Text, Modality.Image)
                    .SetResponseCount(task.n)
                    .SetOutputPath(task._outputPath)
                    .SetCancellationToken(task.token);

                GenerateContentResponse result = await builder.Build().ExecuteAsync();

                if (result == null || result.Candidates.IsNullOrEmpty()) throw new EmptyResponseException(task.model);

                return await result.ToGeneratedImageAsync(task._outputPath);
            }
        }

        internal override async UniTask<GeneratedImage> GenerateInpaintAsync(GENInpaintTask task)
        {
            if (task.model.Family == ModelFamily.Imagen)
            {
                throw new NotImplementedException($"Model {task.model.Id} does not support image editing.");
            }
            else
            {
                GenerateContentRequest.Builder builder = new GenerateContentRequest.Builder()
                    .SetSender(task.sender)
                    .SetIgnoreLogs(task._ignoreLogs)
                    .SetPrompt(task.prompt)//task.promptText, task.prompt)
                    .SetModel(task.model.Id)
                    .SetResponseModalities(Modality.Text, Modality.Image)
                    .SetResponseCount(task.n)
                    .SetOutputPath(task._outputPath)
                    .SetCancellationToken(task.token);

                GenerateContentResponse result = await builder.Build().ExecuteAsync();

                if (result == null || result.Candidates.IsNullOrEmpty()) throw new EmptyResponseException(task.model);

                return await result.ToGeneratedImageAsync(task._outputPath);
            }
        }

        // AspectRadio: Supported values are "16:9" and "9:16". The default is "16:9". 
        internal override async UniTask<GeneratedVideo> GenerateVideoAsync(GENVideoTask task)
        {
            PredictionRequest.Builder builder = new PredictionRequest.Builder()
                   .SetSender(task.sender)
                   .SetIgnoreLogs(task._ignoreLogs)
                   .SetModel(task.model.Id)
                   .SetPrompt(task.prompt)
                   .SetNumberOfImages(task.n)
                   .SetOutputPath(task._outputPath)
                   .SetCancellationToken(task.token);

            AspectRatio? aspectRatio = task.GetAspectRatio();
            PersonGeneration? personGeneration = task.GetPersonGeneration();

            if (aspectRatio != null)
            {
                if (aspectRatio.Value != AspectRatio.Vertical && aspectRatio.Value != AspectRatio.Horizontal)
                {
                    //throw new ArgumentOutOfRangeException(nameof(aspectRatio), "Aspect ratio must be either 16:9 (Horizontal) or 9:16 (Vertical).");
                    builder.SetAspectRatio(AspectRatio.Horizontal); // 기본값으로 설정
                }
                else
                {
                    builder.SetAspectRatio(aspectRatio.Value);
                }
            }
            if (personGeneration != null) builder.SetPersonGeneration(personGeneration.Value);

            GeneratedVideo result = await builder.Build().GenerateVideoAsync();


            return result; // 리턴 형태를 알수없음으로 테스팅 필요 
        }

        private GenerateContentRequest CreateTextRequest(GENResponseTask task, Type jsonSchemaType)
        {
            var builder = new GenerateContentRequest.Builder()
                .SetSender(task.sender)
                .SetIgnoreLogs(task._ignoreLogs)
                .SetModel(task.model.Id)
                .SetModelOptions(task.modelSettings)
                .SetInstruction(task.instruction)
                .SetPrompt(task.prompt, task.attachedFiles)
                .SetJsonSchema(jsonSchemaType)
                .SetCancellationToken(task.token);

            return builder.Build();
        }

        internal override async UniTask<QueryResponse<IModelData>> ListModelsAsync(Query query = null)
        {
            QueryResponse<GoogleModelData> res = await GenerativeAI.DefaultInstance.Models.ListAsync(query as TokenQuery);
            return res.ToSoftRef<GoogleModelData, IModelData>();
        }

        internal override async UniTask<IModelData> RetrieveModelAsync(string modelId)
        {
            return await GenerativeAI.DefaultInstance.Models.RetrieveAsync(modelId);
        }

        internal override async UniTask<bool> DeleteModelAsync(string modelId)
        {
            return await GenerativeAI.DefaultInstance.TunedModels.DeleteAsync(modelId);
        }

        // Generate Speech (Currently in Preview / Added 2025.05.29) --------------------------------------------------------------------------------------------------------------------------- 
        internal override async UniTask<GeneratedAudio> GenerateSpeechAsync(GENSpeechTask task)
        {
            if (task.model == null) task.model = GenerativeAISettings.DefaultTTS;
            if (string.IsNullOrWhiteSpace(task.voiceId)) task.voiceId = GenerativeAISettings.DefaultVoice;

            var builder = new GenerateContentRequest.Builder()
               .SetSender(task.sender)
               .SetIgnoreLogs(task._ignoreLogs)
               .SetModel(task.model.Id)
               .SetPrompt(task.prompt)
               .SetVoice(task.voiceId)
               .SetCancellationToken(task.token);

            GenerateContentResponse res = await builder.Build().ExecuteAsync() ?? throw new EmptyResponseException(task.model);
            List<string> inlineDataList = res.ToInlineDataList();
            if (inlineDataList.IsNullOrEmpty()) throw new EmptyResponseException(task.model);

            /*
            }' | jq -r '.candidates[0].content.parts[0].inlineData.data' | \
                base64 --decode > out.pcm
            # You may need to install ffmpeg.
            ffmpeg -f s16le -ar 24000 -ac 1 -i out.pcm out.wav
            */

            // format: signed 16-bit little endian PCM
            // sample rate: 24000 Hz
            // channels: 1 (mono)
            // encoding: pcm

            List<File<AudioClip>> audioClips = new();
            AudioFormat decodingFormat = new()
            {
                Encoding = AudioEncoding.PCM,
                SampleRate = SampleRate.Hz24000,
                BitDepth = BitDepth.Bit16,
                Channels = 1 // Mono 
            };

            foreach (string inlineData in inlineDataList)
            {
                if (string.IsNullOrEmpty(inlineData))
                {
                    Debug.LogWarning("Inline data is empty or null.");
                    continue;
                }

                File<AudioClip> audioClip = await AudioClipDecoder.DecodeAsync(inlineData, decodingFormat, task.outputPath, MIMEType.PCM);

                if (audioClip != null)
                {
                    await AudioFileWriter.WriteFileAsync(audioClip.Asset, audioClip.FullPath, AudioType.WAV);
                    audioClips.Add(audioClip);
                }
                else
                {
                    Debug.LogWarning("Failed to decode audio clip from inline data.");
                }
            }

            List<AudioClip> audioClipsList = audioClips.ConvertAll(file => file.Asset);
            List<string> paths = audioClips.ConvertAll(file => file.FullPath);

            return new GeneratedAudio(audioClipsList.ToArray(), paths.ToArray(), res.Usage);
        }
    }
}