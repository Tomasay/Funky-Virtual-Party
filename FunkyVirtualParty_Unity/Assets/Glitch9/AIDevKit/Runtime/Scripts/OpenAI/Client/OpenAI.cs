using Glitch9.AIDevKit.Client;
using Glitch9.AIDevKit.OpenAI.Services;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenAI
{
    public class OpenAIClientSettingsFactory : AIClientSettingsFactory
    {
        protected override CRUDClientSettings CreateSettings()
        {
            List<RESTHeader> additionalHeaders = new();
            string organization = OpenAISettings.Organization;
            string project = OpenAISettings.ProjectId;

            if (!string.IsNullOrEmpty(organization)) additionalHeaders.Add(new(OpenAIConfig.OrganizationHeaderName, organization));
            if (!string.IsNullOrEmpty(project)) additionalHeaders.Add(new(OpenAIConfig.ProjectHeaderName, project));

            return new CRUDClientSettings
            {
                Name = nameof(OpenAI),
                BaseURL = OpenAIConfig.BaseUrl,
                ApiKey = CRUDParam.Header(() => OpenAISettings.Instance.GetApiKey()),
                Version = CRUDParam.Query(OpenAIConfig.Version),
                BetaVersion = CRUDParam.Header(OpenAIConfig.BetaVersion, OpenAIConfig.BetaHeaderName, OpenAIConfig.BetaVersion),
                AdditionalHeaders = additionalHeaders.ToArray(),
            };
        }

        protected override AIClientSerializerSettings CreateSerializerSettings()
        {
            return new AIClientSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new CompletionRequestConverter<ChatCompletionRequest>(Api.OpenAI),
                    //new ChatMessageConverter(Api.OpenAI),
                    new OpenAIChatMessageConverter(),
                    new UsageConverter(Api.OpenAI),
                    new ChatRoleConverter(Api.OpenAI),
                    new ContentConverter(Api.OpenAI),
                    new ToolCallConverter(),

                    new QueryResponseConverter<OpenAIModelData>(),
                    new QueryResponseConverter<OpenAIFile>(),
                    new QueryResponseConverter<Batch>(),
                    new QueryResponseConverter<Image>(),
                    new QueryResponseConverter<FineTuningJob>(),
                    new QueryResponseConverter<FineTuningEvent>(),

                    new QueryResponseConverter<Assistant>(),
                    new QueryResponseConverter<Thread>(),
                    new QueryResponseConverter<ThreadMessage>(),
                    new QueryResponseConverter<Run>(),
                    new QueryResponseConverter<RunStep>(),

                    new QueryResponseConverter<VectorStore>(),
                    new QueryResponseConverter<VectorStoreFile>(),
                    new QueryResponseConverter<VectorStoreFilesBatch>(),

                    // new ThreadMessageRequestConverter(),
                    // new ToolChoiceConverter(),
                    // new CodeInterpreterOutputConverter(), 
                },
            };
        }
    }

    // File History
    //
    // 2024-06-02 Changes by Munchkin:
    // - Changed the class name from OpenAIClient to OpenAI.
    //   (This is due to eliminate the confusion between this asset and the examples on the official OpenAI documentation.)

    /// <summary>
    /// Represents the OpenAI service with methods to interact with the API.
    /// </summary>
    public class OpenAI : AIClient<OpenAI>
    {
        /// <summary>
        /// The default instance of the OpenAI client.
        /// </summary>
        public static OpenAI DefaultInstance => _defaultInstance ??= new();
        private static OpenAI _defaultInstance;

        public static RESTHeader AssistantsApiHeader = new(OpenAIConfig.BetaHeaderName, OpenAIConfig.BetaHeader_Assistants);
        public static RESTHeader RealtimeApiHeader = new(OpenAIConfig.BetaHeaderName, OpenAIConfig.BetaHeader_Realtime);

        // Services 
        /// <summary>
        /// Learn how to turn audio into text or text into audio.
        /// </summary>
        public AudioService Audio { get; }

        /// <summary>
        /// Given a list of messages comprising a conversation, the model will return a response.
        /// </summary>
        public ChatService Chat { get; }

        /// <summary>
        /// Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.
        /// </summary>
        public EmbeddingService Embeddings { get; }

        /// <summary>
        /// Manage fine-tuning jobs to tailor a model to your specific training data.
        /// </summary>
        public FineTuningService FineTuning { get; }

        /// <summary>
        /// Create large batches of API requests for asynchronous processing.
        /// The Batch API returns completions within 24 hours for a 50% discount.
        /// </summary>
        public BatchService Batch { get; }

        /// <summary>
        /// Files are used to upload documents that can be used with features like Assistants, Fine-tuning, and Batch API.
        /// </summary>
        public FileService Files { get; }

        /// <summary>
        /// Given a prompt and/or an input image, the model will generate a new image.
        /// </summary>
        public ImageService Images { get; }

        /// <summary>
        /// List and describe the various models available in the API.
        /// You can refer to the Models documentation to understand what models are available and the differences between them.
        /// </summary>
        public ModelService Models { get; }

        /// <summary>
        /// Given some input text, outputs if the model classifies it as potentially harmful across several categories.
        /// </summary>
        public ModerationService Moderations { get; }

        /// <summary>
        /// Beta features that are not yet available in the main API.
        /// </summary>
        public BetaService Beta { get; }

        public OpenAI() : base(Api.OpenAI, new OpenAIClientSettingsFactory())
        {
            // Initialize services
            Audio = new AudioService(this);
            Chat = new ChatService(this);
            Embeddings = new EmbeddingService(this);
            FineTuning = new FineTuningService(this);
            Batch = new BatchService(this);
            Files = new FileService(this);
            Images = new ImageService(this);
            Models = new ModelService(this);
            Moderations = new ModerationService(this);
            Beta = new BetaService(this);
        }

        protected override bool IsDeletedPredicate(RESTResponse res)
        {
            if (res is RESTResponse<DeletionStatus> deletionStatus)
            {
                return deletionStatus.Body.Deleted;
            }
            return res.HasBody;
        }

        private class DeletionStatus : AIResponse
        {
            [JsonProperty("deleted")] public bool Deleted { get; set; }
        }
    }
}

