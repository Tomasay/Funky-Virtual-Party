using System;
using Newtonsoft.Json;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Added 2025.04.15 to handle more setting properties for the RESTClient.
    /// This class is used to configure the RESTClient settings.
    /// </summary>
    public class RESTClientSettings
    {
        public JsonSerializerSettings JsonSettings { get; set; } = JsonConfig.DefaultSerializerSettings;
        public SSEParser SSEParser { get; set; } = new();
        public RESTLogger Logger { get; set; } = new("RESTClient", RESTApiV5.Config.kDefaultLogLevel);
        public TimeSpan Timeout { get; set; } = RESTApiV5.Config.kDefaultTimeout;
        public bool AllowBodyWithDELETE { get; set; } = false;
    }
}