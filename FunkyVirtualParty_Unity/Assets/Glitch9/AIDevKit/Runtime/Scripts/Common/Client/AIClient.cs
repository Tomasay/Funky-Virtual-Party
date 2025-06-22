using System;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Glitch9.AIDevKit.Client
{
    public class AIClientSerializerSettings
    {
        public TextCase TextCase { get; set; } = TextCase.SnakeCase;
        public List<JsonConverter> Converters { get; set; }
    }

    public abstract class AIClientSettingsFactory
    {
        protected abstract CRUDClientSettings CreateSettings();
        protected abstract AIClientSerializerSettings CreateSerializerSettings();

        public CRUDClientSettings Create()
        {
            CRUDClientSettings settings = CreateSettings();

            settings.Logger = new CRUDLogger(settings.Name, AIDevKitSettings.LogLevel);
            settings.Timeout = TimeSpan.FromSeconds(AIDevKitSettings.RequestTimeout);

            var serializerSettings = CreateSerializerSettings();
            var namingStrategy = serializerSettings.TextCase switch
            {
                TextCase.CamelCase => (NamingStrategy)new CamelCaseNamingStrategy { ProcessDictionaryKeys = true },
                TextCase.SnakeCase => (NamingStrategy)new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true },
                _ => throw new ArgumentOutOfRangeException(nameof(serializerSettings.TextCase), serializerSettings.TextCase, null)
            };

            List<JsonConverter> converters = serializerSettings.Converters ?? new List<JsonConverter>();
            converters.AddRange(new List<JsonConverter>
            {
                new ApiEnumConverter(),
                new SystemLanguageISOConverter(),
                new StringOrConverter<string>(),
                new SafeEnumConverter<UsageType>(),

                // new ZuluTimeJsonConverter(),
                // new UnixTimeJsonConverter(), 

                // new NullableStopReasonConverter(),
                // new ModelConverter(),
                // new VoiceConverter(),
                // new VoiceTypeConverter(), 

                // // chat completion
                // new ContentPartWrapperConverter(),
                // new ImageContentPartConverter(),
                // new AnnotationConverter(),
                // new TextContentPartConverter(),
                // new HarmCategoryConverter(),

                // new StrictJsonSchemaConverter(),
                // new ResponseFormatConverter(),
            });

            settings.JsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new RESTContractResolver { NamingStrategy = namingStrategy },
                Converters = converters,
            };

            return settings;
        }
    }

    public abstract partial class AIClient<TSelf> : CRUDClient<TSelf> where TSelf : AIClient<TSelf>
    {
        public Api Api { get; }

        protected AIClient(Api Api, AIClientSettingsFactory settingsFactory) : base(clientSettings: settingsFactory.Create())
        {
            this.Api = Api;
            OnException = DefaultExceptionHandler;

            void DefaultExceptionHandler(string endpoint, Exception exception)
            {
                throw new ApiException(this.Api, endpoint, exception);
            }
        }

        protected override string FormatErrorMessage(string errorMessage) => AIDevKitDebug.FormatErrorMessage(errorMessage);
        protected override bool IsDeletedPredicate(RESTResponse res) => res.HasBody;
    }
}