using Glitch9.AIDevKit.Client;
using Glitch9.AIDevKit.Google.Services;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glitch9.AIDevKit.Google
{
    public class GenerativeAIClientSettingsFactory : AIClientSettingsFactory
    {
        protected override CRUDClientSettings CreateSettings()
        {
            return new CRUDClientSettings
            {
                Name = nameof(GenerativeAI),
                BaseURL = GoogleAIConfig.BaseUrl,
                ApiKey = CRUDParam.Query(() => GenerativeAISettings.Instance.GetApiKey(), "key"),
                Version = CRUDParam.Query(GoogleAIConfig.Version),
                BetaVersion = CRUDParam.Query(GoogleAIConfig.BetaVersion),
            };
        }

        protected override AIClientSerializerSettings CreateSerializerSettings()
        {
            return new AIClientSerializerSettings
            {
                TextCase = TextCase.CamelCase,
                Converters = new List<JsonConverter>
                {
                    new UsageConverter(Api.Google),
                    new ChatRoleConverter(Api.Google),
                    new ContentConverter(Api.Google),
                    new FunctionCallConverter(),
                    new ModalityConverter(),

                    new QueryResponseConverter<GoogleModelData>("models"),
                    new QueryResponseConverter<GoogleFile>("files"),
                    new QueryResponseConverter<Chunk>("chunks"),

                    new PredictionRequestConverter(),

                },
            };
        }
    }

    public class GenerativeAI : AIClient<GenerativeAI>
    {
        // Services
        public CachedContentService CachedContents { get; }
        public CorporaService Corpora { get; }
        public FileService Files { get; }
        public MediaService Media { get; }
        public ModelService Models { get; }
        public TunedModelService TunedModels { get; }

        /// <summary>
        /// The default instance of the GenerativeAI client.
        /// </summary>
        public static GenerativeAI DefaultInstance => _defaultInstance ??= new();
        private static GenerativeAI _defaultInstance;
        private GoogleJsonArrayStreamBuffer<GenerateContentResponse> _streamBuffer;


        public GenerativeAI() : base(Api.Google, new GenerativeAIClientSettingsFactory())
        {
            // Initialize services
            CachedContents = new CachedContentService(this);
            Corpora = new CorporaService(this);
            Files = new FileService(this);
            Media = new MediaService(this);
            Models = new ModelService(this);
            TunedModels = new TunedModelService(this);

            // Initialize the stream buffer for handling streaming responses
            _streamBuffer = new GoogleJsonArrayStreamBuffer<GenerateContentResponse>(JsonSettings);
        }

        internal IEnumerable<ChatCompletionChunk> CreateChunk(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) yield break;

            const string error = "\"error\":";

            if (raw.Contains(error))
            {
                var errorResponse = JsonConvert.DeserializeObject<List<ErrorResponseWrapper>>(raw, JsonSettings);
                string errorMessage;

                if (errorResponse == null)
                {
                    errorMessage = $"Failed to parse error response: {raw}";
                }
                else
                {
                    errorMessage = errorResponse.FirstOrDefault().Error?.Message;
                }

                yield return ChatCompletionChunk.Error(errorMessage);
                yield break;
            }

            foreach ((GenerateContentResponse response, bool isDone) in _streamBuffer.Append(raw))
            {
                if (response == null)
                {
                    //AIDevKitDebug.LogWarning($"[GenerativeAI] Received null response from stream buffer.");
                    continue;
                }

                ChatCompletion delta = ChatCompletionFactory.CreateDelta(
                    response.FirstContent(),
                    response.GetToolCalls(),
                    response.Usage
                );

                yield return new ChatCompletionChunk
                {
                    isDone = isDone,
                    Value = delta,
                };
            }

            // string trimmedLine = raw.Trim().TrimStart(',').TrimEnd(',');

            // bool isDone = false;

            // if (!trimmedLine.StartsWith('['))
            // {
            //     trimmedLine = $"[{raw}";
            // }

            // if (trimmedLine.EndsWith(']'))
            // {
            //     isDone = true;
            // }
            // else
            // {
            //     trimmedLine = $"{trimmedLine}]";
            // }

            // if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine == "[]") yield break;

            // if (!JsonUtil.IsJsonComplete(trimmedLine))
            // {
            //     Logger.Error($"Incomplete JSON response: {trimmedLine}");
            //     yield break;
            // }

            // var list = JsonConvert.DeserializeObject<List<GenerateContentResponse>>(trimmedLine, JsonSettings);

            // if (list.IsNullOrEmpty())
            // {
            //     Logger.Error($"Failed to parse response: {trimmedLine}");
            //     yield break;
            // }

            // if (list.Count > 1)
            // {
            //     // 1개 이상일때 로그 남겨서 디버깅
            //     Logger.Warning($"Received multiple items in response: {list.Count}. Only the first item will be processed.");

            // }

            // for (int i = 0; i < list.Count; i++)
            // {
            //     var item = list[i];
            //     if (item == null) continue;

            //     bool isLast = i == list.Count - 1;

            //     ChatCompletion delta = ChatCompletionFactory.CreateDelta(
            //         item.FirstContent(),
            //         item.GetToolCalls(),
            //         item.Usage
            //     );

            //     yield return new ChatCompletionChunk
            //     {
            //         isDone = isLast && isDone,
            //         Value = delta,
            //     };
            // }
        }
    }
}