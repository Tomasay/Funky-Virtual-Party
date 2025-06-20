using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.Google
{
    public static class RequestExtensions
    {
        internal static string GetModelName(this GenerativeAIRequest request)
        {
            string modelName = request.Model.Id;
            if (modelName.Contains('/')) modelName = modelName.Split('/')[1];
            return modelName;
        }

        public static Dataset AddTrainingData(this Dataset dataset, params TuningExample[] trainingData)
        {
            dataset ??= new Dataset();
            dataset.AddRange(trainingData);
            return dataset;
        }

        public static async UniTask<GenerateContentResponse> ExecuteAsync(this GenerateContentRequest request)
        => await GenerativeAI.DefaultInstance.Models.GenerateContentAsync(request);
        public static async UniTask StreamAsync(this GenerateContentRequest request, IChatCompletionStreamHandler streamHandler)
        => await GenerativeAI.DefaultInstance.Models.StreamGenerateContentAsync(request, streamHandler);
        public static async UniTask<PredictionResponse> GenerateImageAsync(this PredictionRequest request)
        => await GenerativeAI.DefaultInstance.Models.GenerateImageAsync(request);
        public static async UniTask<GeneratedVideo> GenerateVideoAsync(this PredictionRequest request)
        => await GenerativeAI.DefaultInstance.Models.GenerateVideoAsync(request);
    }
}