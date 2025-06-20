using Glitch9.AIDevKit.Client;
using Glitch9.AIDevKit.OpenRouter.Services;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenRouter
{
    public class OpenRouterClientSettingsFactory : AIClientSettingsFactory
    {
        protected override CRUDClientSettings CreateSettings()
        {
            List<RESTHeader> additionalHeaders = new();
            string httpReferer = OpenRouterSettings.HttpReferer;
            string xTitle = OpenRouterSettings.XTitle;

            if (!string.IsNullOrEmpty(httpReferer)) additionalHeaders.Add(new(OpenRouterConfig.kHttpRefererHeaderName, httpReferer));
            if (!string.IsNullOrEmpty(xTitle)) additionalHeaders.Add(new(OpenRouterConfig.kXTitleHeaderName, xTitle));

            return new CRUDClientSettings
            {
                Name = nameof(OpenRouter),
                BaseURL = OpenRouterConfig.BaseUrl,
                ApiKey = CRUDParam.Header(() => OpenRouterSettings.Instance.GetApiKey()),
                Version = CRUDParam.Query(OpenRouterConfig.Version),
                AdditionalHeaders = additionalHeaders.ToArray(),
                AllowBodyWithDELETE = true,
            };
        }

        protected override AIClientSerializerSettings CreateSerializerSettings()
        {
            return new AIClientSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new CompletionRequestConverter<ChatCompletionRequest>(Api.OpenRouter),
                    new ChatMessageConverter(Api.OpenRouter),
                    new UsageConverter(Api.OpenRouter),
                    new ContentConverter(Api.OpenRouter),
                    new ToolCallConverter(),
                    new ModalityStringListConverter(),
                },
            };
        }
    }

    public class OpenRouter : AIClient<OpenRouter>
    {
        /// <summary>
        /// The default instance of the OpenRouter client.
        /// </summary>
        public static OpenRouter DefaultInstance => _defaultInstance ??= CreateDefault();
        private static OpenRouter _defaultInstance;

        // Services
        public ChatCompletionService ChatCompletion { get; private set; }
        public CompletionService Completion { get; private set; }
        public ModelService Models { get; private set; }


        private static OpenRouter CreateDefault()
        {
            return new OpenRouter
            {
                OnException = DefaultExceptionHandler,
            };

            static void DefaultExceptionHandler(string endpoint, Exception exception)
            {
                LogService.Error($"{endpoint}: {exception}");
            }
        }

        public OpenRouter() : base(new OpenRouterClientSettingsFactory())
        {
            // Initialize services
            ChatCompletion = new ChatCompletionService(this);
            Completion = new CompletionService(this);
            Models = new ModelService(this);
        }

        internal IEnumerable<ChatCompletionChunk> CreateChunk(string sseString)
        {
            if (string.IsNullOrEmpty(sseString)) yield break;

            const string error = "\"error\":";

            if (sseString.Contains(error))
            {
                ErrorResponseWrapper errorResponse = JsonConvert.DeserializeObject<ErrorResponseWrapper>(sseString, JsonSettings);
                string errorMessage;

                if (errorResponse == null)
                {
                    errorMessage = $"Failed to parse error response: {sseString}";
                }
                else
                {
                    errorMessage = errorResponse.Error?.Message;
                }

                yield return ChatCompletionChunk.Error(errorMessage);
                yield break;
            }

            var data = SSEParser.Parse(sseString);

            if (data.IsNullOrEmpty()) yield break;


            foreach (var (field, result) in data)
            {
                if (field == SSEField.Error)
                {
                    yield return ChatCompletionChunk.Error(result);
                    yield break;
                }

                if (field != SSEField.Data || string.IsNullOrEmpty(result))
                {
                    yield return null;
                }

                if (SSEParser.IsDone(result))
                {
                    yield return ChatCompletionChunk.Done();
                    yield break;
                }

                ChatCompletion c = JsonConvert.DeserializeObject<ChatCompletion>(result, JsonSettings);
                yield return ChatCompletionChunk.Chunk(c);
            }
        }
    }
}

