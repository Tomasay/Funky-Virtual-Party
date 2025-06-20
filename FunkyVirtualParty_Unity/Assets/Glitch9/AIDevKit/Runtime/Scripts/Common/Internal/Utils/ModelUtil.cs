using System.Linq;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.Ollama;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenRouter;

namespace Glitch9.AIDevKit
{
    internal class ModelUtil
    {
        internal static string ReturnDefaultIfEmpty(string value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        internal static bool IsDefaultModel(string id, Api api, ModelFeature feature)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false; // If the model ID is null or empty, it's not a default model
            }
            else
            {
                bool isDefault = AIDevKitConfig.SystemDefaultModels.Contains(id);
                if (isDefault) return true;

                return api switch
                {
                    Api.OpenRouter => OpenRouterSettings.IsDefaultModel(id, feature),
                    Api.OpenAI => OpenAISettings.IsDefaultModel(id, feature),
                    Api.Google => GenerativeAISettings.IsDefaultModel(id, feature),
                    Api.ElevenLabs => ElevenLabsSettings.IsDefaultModel(id, feature),
                    Api.Ollama => OllamaSettings.IsDefaultModel(id, feature),
                    _ => false,
                };
            }
        }
    }
}