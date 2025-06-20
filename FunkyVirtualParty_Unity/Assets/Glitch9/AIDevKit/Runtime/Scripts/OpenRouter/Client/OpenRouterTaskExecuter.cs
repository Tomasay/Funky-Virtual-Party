using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenRouter
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal class OpenRouterTaskExecuter : GENTaskExecuter
    {

#if UNITY_EDITOR 
        static OpenRouterTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.OpenRouter, new OpenRouterTaskExecuter());
        }
#else 
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ResisterTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.OpenRouter, new OpenRouterTaskExecuter());
        }
#endif

        internal override Api Api => Api.OpenRouter;

        internal override async UniTask<ChatCompletion> GenerateResponseAsync(GENResponseTask task, Type jsonSchemaType)
        {
            var req = task.CreateChatCompletionRequest(jsonSchemaType, false);
            return await req.ExecuteAsync();
        }

        internal override async UniTask StreamResponseAsync(GENResponseTask task, Type jsonSchemaType, IChatCompletionStreamHandler streamHandler)
        {
            var req = task.CreateChatCompletionRequest(jsonSchemaType, true);
            await req.StreamAsync(streamHandler);
        }

        internal override async UniTask<QueryResponse<IModelData>> ListModelsAsync(Query query)
        {
            QueryResponse<OpenRouterModelData> res = await OpenRouter.DefaultInstance.Models.ListAsync();
            return res.ToSoftRef<OpenRouterModelData, IModelData>();
        }


        internal override async UniTask<IModelData> RetrieveModelAsync(string modelId)
        {
            QueryResponse<IModelData> response = await ListModelsAsync(null);

            if (response == null || response.Data.IsNullOrEmpty())
                throw new EmptyResponseException(EndpointType.RetrieveModel);

            IModelData model = response.Data.FirstOrDefault(m => m.Id.Equals(modelId, StringComparison.OrdinalIgnoreCase))
                ?? throw new ModelNotFoundOnApiException(Api.OpenRouter, modelId);

            return model;
        }
    }
}