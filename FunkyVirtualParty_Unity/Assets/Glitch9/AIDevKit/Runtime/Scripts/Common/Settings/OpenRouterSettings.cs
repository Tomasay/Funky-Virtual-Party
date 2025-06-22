using UnityEngine;
using Glitch9.AIDevKit.Client;

namespace Glitch9.AIDevKit.OpenRouter
{
    [AssetPath(AIDevKitConfig.CreatePath)]
    public class OpenRouterSettings : AIClientSettings<OpenRouterSettings>
    {
        // general settings
        [SerializeField] private string httpReferer = string.Empty;
        [SerializeField] private string xTitle = string.Empty;

        // default models
        [SerializeField] private string defaultLLM;

        public static string HttpReferer => Instance.httpReferer;
        public static string XTitle => Instance.xTitle;
        public static string DefaultLLM => Instance.defaultLLM;

        public static bool IsDefaultModel(string id, ModelFeature feature)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;

            if (feature.HasFlag(ModelFeature.TextGeneration) && id == DefaultLLM) return true;

            return false;
        }
    }
}