using System.Linq;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.Ollama;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenRouter;

namespace Glitch9.AIDevKit
{
    internal class ModelPolicy
    {
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

        internal static int GetMaxN(Model model)
        {
            const int defaultMaxN = 1;

            if (model == null) return defaultMaxN;
            if (string.IsNullOrEmpty(model.Id)) return defaultMaxN;
            if (model.IsDallE2()) return 10; // DALL-E 2 has a max N of 10
            if (model.IsDallE3()) return 1; // DALL-E 3 has a max N of 1
            if (model.IsGptImage1()) return 1; // GPT Image 1 has a max N of 1
            if (model.IsGemini()) return 1; // Gemini models typically have a max N of 1
            if (model.IsImagen()) return 4; // Imagen models typically have a max N of 4
            if (model.IsLLM()) return 20;

            return defaultMaxN;
        }

        internal static ImageSize GetDefaultImageSize(string modelId)
        {
            if (AIDevKitConfig.SupportedImageSizes.TryGetValue(modelId, out ImageSize[] sizes))
            {
                return sizes[0]; // Return the first size as default
            }
            return ImageSize._1024x1024; // Fallback default size
        }

        internal static ImageQuality GetDefaultImageQuality(string modelId)
        {
            if (AIDevKitConfig.SupportedImageQualities.TryGetValue(modelId, out ImageQuality[] qualities))
            {
                return qualities[0]; // Return the first quality as default
            }
            return ImageQuality.Standard; // Fallback default quality
        }
    }
}