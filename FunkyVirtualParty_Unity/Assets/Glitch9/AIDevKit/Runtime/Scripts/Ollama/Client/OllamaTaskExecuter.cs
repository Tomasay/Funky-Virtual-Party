using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Ollama
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal class OllamaTaskExecuter : GENTaskExecuter
    {

#if UNITY_EDITOR 
        static OllamaTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.Ollama, new OllamaTaskExecuter());
        }
#else 
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ResisterTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.Ollama, new OllamaTaskExecuter());
        }
#endif

        internal override Api Api => Api.Ollama;

        internal override async UniTask<ChatCompletion> GenerateResponseAsync(GENResponseTask task, Type jsonSchemaType)
        {
            if (task.messages.IsNullOrEmpty())
            {
                CompletionRequest req = task.CreateCompletionRequest(jsonSchemaType, false);
                GenerateResponse result = await req.ExecuteAsync() ?? throw new EmptyResponseException(task.model);
                return ChatCompletionFactory.FromText(result.ToString(), result.FinishReason, UsageFactory.Free());
            }
            else
            {
                ChatCompletionRequest req = task.CreateChatCompletionRequest(jsonSchemaType, false);
                ChatResponse result = await req.ExecuteAsync() ?? throw new EmptyResponseException(task.model);
                return ChatCompletionFactory.FromMessage(result.Message, result.FinishReason, UsageFactory.Free());
            }
        }

        internal override async UniTask StreamResponseAsync(GENResponseTask task, Type jsonSchemaType, IChatCompletionStreamHandler streamHandler)
        {
            CompletionRequest req = task.CreateCompletionRequest(jsonSchemaType, true);
            await req.StreamAsync(streamHandler);
        }

        internal override async UniTask<QueryResponse<IModelData>> ListModelsAsync(Query query)
        {
            ListModelsResponse res = await Ollama.DefaultInstance.Models.ListLocalModelsAsync();
            if (res == null || res.Models.IsNullOrEmpty()) return null;

            IModelData[] models = res.Models
                .Where(m => m is IModelData)
                .Select(m => (IModelData)m)
                .ToArray();

            return new QueryResponse<IModelData>
            {
                Data = models,
            };
        }

        internal override async UniTask<IModelData> RetrieveModelAsync(string modelId)
        {
            // return await Ollama.DefaultInstance.Models.RetrieveAsync(modelId);
            // return UniTask.FromException<IModelData>(
            //     new NotImplementedException("Ollama does not support retrieving model details at the moment."));

            // query the list of models and find the one with the matching ID

            QueryResponse<IModelData> response = await ListModelsAsync(null);
            if (response == null || response.Data.IsNullOrEmpty())
                throw new EmptyResponseException(EndpointType.RetrieveModel);

            IModelData model = response.Data.FirstOrDefault(m => m.Id.Equals(modelId, StringComparison.OrdinalIgnoreCase))
                ?? throw new ModelNotFoundOnApiException(Api.Ollama, modelId);

            return model;
        }
    }
}