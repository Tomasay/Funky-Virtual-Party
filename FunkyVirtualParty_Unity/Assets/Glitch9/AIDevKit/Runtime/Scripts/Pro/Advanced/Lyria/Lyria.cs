namespace Glitch9.AIDevKit.Advanced.Lyria
{
    internal static class Lyria
    {
        internal const string UrlFormat = "wss://generativelanguage.googleapis.com/{apiVersion}/{modelId}:stream?key={apiKey}";
        internal const string ApiVersion = "v1alpha";
        internal const string DefaultModelId = "models/lyria-realtime-exp";
    }
}