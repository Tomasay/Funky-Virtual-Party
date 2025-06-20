using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Client;
using Glitch9.AIDevKit.Ollama.Services;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Glitch9.AIDevKit.Ollama
{
    public class OllamaClientSettingsFactory : AIClientSettingsFactory
    {
        protected override CRUDClientSettings CreateSettings()
        {
            return new CRUDClientSettings
            {
                Name = nameof(Ollama),
                BaseURL = OllamaSettings.GetEndpoint(),
                Version = CRUDParam.Query(OllamaConfig.Version),
                AllowBodyWithDELETE = true,
            };
        }

        protected override AIClientSerializerSettings CreateSerializerSettings()
        {
            return new AIClientSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new CompletionRequestConverter<CompletionRequest>(Api.Ollama),
                    new CompletionRequestConverter<ChatCompletionRequest>(Api.Ollama),
                    new ChatMessageConverter(Api.Ollama),
                    new UsageConverter(Api.Ollama),
                    new ContentConverter(Api.Ollama),
                    new ToolCallConverter(),
                },
            };
        }
    }

    public class Ollama : AIClient<Ollama>
    {
        /// <summary>
        /// The default instance of the Ollama client.
        /// </summary>
        public static Ollama DefaultInstance => _defaultInstance ??= CreateDefault();
        private static Ollama _defaultInstance;

        // Services
        public ChatService Chat { get; private set; }
        public GenerateService Generate { get; private set; }
        public ModelService Models { get; private set; }
        public EmbeddingsService Embeddings { get; private set; }


        private static Ollama CreateDefault()
        {
            return new Ollama
            {
                OnException = DefaultExceptionHandler,
            };

            static void DefaultExceptionHandler(string endpoint, Exception exception)
            {
                LogService.Error($"{endpoint}: {exception} \n{exception.StackTrace}");
            }
        }

        public Ollama() : base(new OllamaClientSettingsFactory())
        {
            // Initialize services
            Chat = new ChatService(this);
            Generate = new GenerateService(this);
            Models = new ModelService(this);
            Embeddings = new EmbeddingsService(this);
        }

        public async UniTask<string> GetVersionAsync()
        {
            const string kVersionEndpointFormat = "http://{0}/api/version";

            string endpoint = OllamaSettings.GetEndpoint();

            if (string.IsNullOrEmpty(endpoint))
                throw new Exception("OllamaSettings endpoint is null or empty. Please ensure OllamaSettings.asset exists in the project.");

            string url = string.Format(kVersionEndpointFormat, endpoint);

            var request = new RESTRequest()
            {
                Endpoint = url,
            };

            var response = await GETAsync<Dictionary<string, string>>(request);

            if (response is RESTResponse<Dictionary<string, string>> restResponse)
            {
                if (restResponse.Body != null &&
                    restResponse.Body.TryGetValue("version", out string version))
                    return version;
                else
                    throw new Exception("Failed to retrieve version from the response.");
            }
            else
            {
                throw new Exception("Failed to retrieve version. Response is null or invalid.");
            }
        }

        internal IEnumerable<ChatCompletionChunk> CreateChunk(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) yield return null;

            using var reader = new JsonTextReader(new StringReader(jsonString));

            reader.SupportMultipleContent = true;
            var serializer = JsonSerializer.Create(JsonSettings);

            while (reader.Read())
            {
                ChatDeltaResponse item = serializer.Deserialize<ChatDeltaResponse>(reader);
                if (item != null)
                {
                    bool isDone = item?.Done ?? false;

                    if (isDone)
                    {
                        yield return ChatCompletionChunk.Done();
                        yield break;
                    }

                    ChatCompletion delta = ChatCompletionFactory.Create(item?.Delta?.Content, item?.Delta?.ToolCalls, UsageFactory.Free());
                    yield return ChatCompletionChunk.Chunk(delta);
                }
            }
        }
    }
}

