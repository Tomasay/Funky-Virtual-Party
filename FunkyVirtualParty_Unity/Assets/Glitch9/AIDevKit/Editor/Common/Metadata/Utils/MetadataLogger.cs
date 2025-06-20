namespace Glitch9.AIDevKit.Editor
{
    internal static class MetadataLogger
    {
        internal static void Missing(MetadataType type, Api api, string id)
        {
            // sample: ($"Model ID '{notFound}' not found in OpenAI model descriptions."));
            AIDevKitDebug.Warning($"Missing {type} for model ID '{id}' in {api} metadata.");
        }
    }
}