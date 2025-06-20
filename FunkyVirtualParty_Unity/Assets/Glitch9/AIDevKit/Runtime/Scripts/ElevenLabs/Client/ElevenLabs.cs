using Glitch9.AIDevKit.Client;
using Glitch9.AIDevKit.ElevenLabs.Services;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class ElevenLabsClientSettingsFactory : AIClientSettingsFactory
    {
        protected override CRUDClientSettings CreateSettings()
        {
            return new CRUDClientSettings
            {
                Name = nameof(ElevenLabs),
                BaseURL = ElevenLabsConfig.BASE_URL,
                ApiKey = CRUDParam.Header(() => ElevenLabsSettings.Instance.GetApiKey(), "xi-api-key", "{0}"),
                Version = CRUDParam.Query(ElevenLabsConfig.VERSION),
                BetaVersion = CRUDParam.Query(ElevenLabsConfig.BETA_VERSION),
            };
        }

        protected override AIClientSerializerSettings CreateSerializerSettings()
        {
            return new AIClientSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new QueryResponseConverter<ElevenLabsModelData>("models"),
                    new QueryResponseConverter<ElevenLabsVoiceData>("voices"),
                    new QueryResponseConverter<ElevenLabsSharedVoiceData>("voices"),

                    new SharedVoiceConverter(),
                },
            };
        }
    }


    public class ElevenLabs : AIClient<ElevenLabs>
    {
        // Services Goes Here ---------------------------------------------
        public TextToSpeechService TextToSpeech { get; }
        public SpeechToTextService SpeechToText { get; }
        public VoiceChangerService VoiceChanger { get; }
        public SoundEffectService SoundEffects { get; }
        public AudioIsolationService AudioIsolation { get; }
        public VoiceService Voices { get; }
        public ModelService Models { get; }
        public VoiceLibraryService VoiceLibrary { get; }
        public UserService User { get; }

        /// <summary>
        /// The default instance of the ElevenLabsClient client.
        /// </summary>
        public static ElevenLabs DefaultInstance => _defaultInstance ??= CreateDefault();
        private static ElevenLabs _defaultInstance;

        private static ElevenLabs CreateDefault()
        {
            return new ElevenLabs
            {
                OnException = DefaultExceptionHandler,
            };

            static void DefaultExceptionHandler(string endpoint, Exception exception)
            {
                LogService.Error($"{endpoint}: {exception}");
            }
        }

        public ElevenLabs() : base(new ElevenLabsClientSettingsFactory())
        {
            // Initialize services ---------------------------------
            TextToSpeech = new TextToSpeechService(this);
            SpeechToText = new SpeechToTextService(this);
            VoiceChanger = new VoiceChangerService(this);
            SoundEffects = new SoundEffectService(this);
            AudioIsolation = new AudioIsolationService(this);
            Voices = new VoiceService(this);
            Models = new ModelService(this);
            VoiceLibrary = new VoiceLibraryService(this);
            User = new UserService(this);
        }
    }
}