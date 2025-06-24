using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Google.Services
{
    public class ModelService : CRUDServiceBase<GenerativeAI>
    {
        private const string kEndpoint = "{ver}/models";
        private const string kEndpointWithId = "{ver}/models/{0}";
        public ModelService(GenerativeAI client) : base(client, IsBeta.Models) { }
        public async UniTask<GoogleModelData> RetrieveAsync(string modelId, RequestOptions options = null)
        => await client.GETRetrieveAsync<GoogleModelData>(kEndpointWithId, this, options, PathParam.ID(modelId.GetLastSegment()));
        public async UniTask<QueryResponse<GoogleModelData>> ListAsync(TokenQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<TokenQuery, GoogleModelData>(kEndpoint, this, query, options);
        public async UniTask<CountTokensResponse> CountTokensAsync(CountTokensRequest req)
        => await client.POSTCreateAsync<CountTokensRequest, CountTokensResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.CountTokens), PathParam.ID(req.GetModelName()));

        // Embedding ---------------------------------------------------------------------------------------------------------------------------
        public async UniTask<EmbedContentResponse> GenerateEmbedContentAsync(EmbedContentRequest req)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultEMB;
            return await client.POSTCreateAsync<EmbedContentRequest, EmbedContentResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.EmbedContent), PathParam.ID(req.GetModelName()));
        }
        public async UniTask<BatchEmbedContentsResponse> GenerateBatchEmbedContentsAsync(BatchEmbedContentsRequest req)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultEMB;
            return await client.POSTCreateAsync<BatchEmbedContentsRequest, BatchEmbedContentsResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.BatchEmbedContents), PathParam.ID(req.GetModelName()));
        }

        // Generate Answer ---------------------------------------------------------------------------------------------------------------------------
        public async UniTask<GenerateAnswerResponse> GenerateAnswerAsync(GenerateAnswerRequest req)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultLLM;
            return await client.POSTCreateAsync<GenerateAnswerRequest, GenerateAnswerResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.GenerateAnswer), PathParam.ID(req.GetModelName()));
        }

        // Generate Content ---------------------------------------------------------------------------------------------------------------------------
        public async UniTask<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest req)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultLLM;
            return await client.POSTCreateAsync<GenerateContentRequest, GenerateContentResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.GenerateContent), PathParam.ID(req.GetModelName()));
        }
        public async UniTask StreamGenerateContentAsync(GenerateContentRequest req, ChatCompletionStreamHandler streamhandler)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultLLM;
            req.StreamHandler = streamhandler.Initialize(client.CreateChunk);
            await client.POSTCreateAsync<GenerateContentRequest, GenerateContentResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.StreamGenerateContent), PathParam.ID(req.GetModelName()));
        }

        // Generate Image (Added 2025.03.30) ---------------------------------------------------------------------------------------------------------------------------
        public async UniTask<PredictionResponse> GenerateImageAsync(PredictionRequest req)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultIMG;
            return await client.POSTCreateAsync<PredictionRequest, PredictionResponse>(kEndpointWithId, this, req, PathParam.Method(Methods.Predict), PathParam.ID(req.GetModelName()));
        }

        // Generate Video (Long Running) (Added 2025.05.01) ---------------------------------------------------------------------------------------------------------------------------
        public async UniTask<GeneratedVideo> GenerateVideoAsync(PredictionRequest req)
        {
            if (req.Model == null) req.Model = GenerativeAISettings.DefaultVID;
            Dictionary<string, object> res = await client.POSTCreateAsync<PredictionRequest, Dictionary<string, object>>(kEndpointWithId,
                  this, req, PathParam.Method(Methods.PredictLongRunning), PathParam.ID(req.GetModelName()));

            if (res == null) return null;

            string operationName = res["name"] as string;
            if (string.IsNullOrEmpty(operationName)) return null;

            string apiKey = GenerativeAISettings.Instance.GetApiKey();

            string pollUrl = $"{GoogleAIConfig.BaseUrl}/{GoogleAIConfig.BetaVersion}/{operationName}?key={apiKey}";

            JObject pollRes = await UniTaskPolling.PollAsync(pollUrl, (elapsedTime) =>
            {
                client.Logger.Info($"Generating video... {elapsedTime} seconds elapsed.");
            }) ?? throw new System.Exception("Polling response is null.");


            List<string> urls = new();

            var response = pollRes["response"] ?? throw new System.Exception($"'response' is null in the poll response: {pollRes}");
            var videoResponse = response["generateVideoResponse"] ?? throw new System.Exception($"'generateVideoResponse' is null in the poll response: {pollRes}");
            var samples = videoResponse["generatedSamples"];
            if (samples == null || samples.Type != JTokenType.Array)
                throw new System.Exception($"'generatedSamples' is null or not an array in the poll response: {pollRes}");

            foreach (var sample in samples)
            {
                string videoUri = sample["video"]?["uri"]?.ToString();
                if (string.IsNullOrEmpty(videoUri)) continue;
                urls.Add($"{videoUri}&key={apiKey}");
            }

            AIDevKitDebug.Mark($"Generated video URLs: {string.Join(", ", urls)}");
            if (urls.Count == 0)
            {
                throw new System.Exception("No video URLs found in the response.");
            }

            return await GeneratedVideoFactory.CreateAsync(urls, req.OutputPath, req.Model, MIMEType.MP4, null);
        }
    }
}
