using Glitch9.AIDevKit.Client;
using Glitch9.AIDevKit.Mubert.Services;
using Glitch9.IO.Networking.RESTApi;
using System;

namespace Glitch9.AIDevKit.Mubert
{
    public class MubertClientSettingsFactory : AIClientSettingsFactory
    {
        protected override CRUDClientSettings CreateSettings()
        {
            return new CRUDClientSettings
            {
                Name = nameof(Mubert),
                BaseURL = MubertConfig.BASE_URL,
                ApiKey = CRUDParam.Header(() => MubertSettings.Instance.GetApiKey(), "xi-api-key", "{0}"),
                Version = CRUDParam.Query(MubertConfig.VERSION),
                BetaVersion = CRUDParam.Query(MubertConfig.BETA_VERSION),
            };
        }

        protected override AIClientSerializerSettings CreateSerializerSettings()
        {
            return new AIClientSerializerSettings();
        }
    }

    public class Mubert : AIClient<Mubert>
    {
        // Services Goes Here --------------------------------------------- 
        public TrackGenerationService TrackGeneration { get; }

        /// <summary>
        /// The default instance of the MubertClient client.
        /// </summary>
        public static Mubert DefaultInstance => _defaultInstance ??= CreateDefault();
        private static Mubert _defaultInstance;

        private static Mubert CreateDefault()
        {
            return new Mubert
            {
                OnException = DefaultExceptionHandler,
            };

            static void DefaultExceptionHandler(string endpoint, Exception exception)
            {
                LogService.Error($"{endpoint}: {exception}");
            }
        }

        public Mubert() : base(new MubertClientSettingsFactory())
        {
            // Initialize services --------------------------------- 
            TrackGeneration = new TrackGenerationService(this);
        }
    }
}