using System.Collections.Generic;
using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal class ModelPopupGUI : AssetPopupGUI<Model, ModelFilter>
    {
        protected override Dictionary<Api, List<Model>> GetFilteredAssets(ModelFilter filter) => ModelLibrary.GetFilteredRefs(filter);
        protected override Model GetDefaultAssetId(ModelFilter filter) => GetDefaultModelId(filter);

        internal static string GetDefaultModelId(ModelFilter filter)
        {
            if (filter.Feature != null)
            {
                ModelFeature cap = filter.Feature.Value;
                Api api = ResolveDefaultApi(cap);
                return ResolveDefaultModelId(api, cap);
            }

            return AIDevKitConfig.kDefault_OpenAI_LLM;
        }

        private static Api ResolveDefaultApi(ModelFeature cap)
        {
            return cap switch
            {
                ModelFeature.TextGeneration => GetApi(AIDevKitSettings.DefaultLLM),
                ModelFeature.ImageGeneration => GetApi(AIDevKitSettings.DefaultIMG),
                ModelFeature.SpeechGeneration => GetApi(AIDevKitSettings.DefaultTTS),
                ModelFeature.SpeechRecognition => GetApi(AIDevKitSettings.DefaultSTT),
                ModelFeature.TextEmbedding => GetApi(AIDevKitSettings.DefaultEMB),
                ModelFeature.Moderation => GetApi(AIDevKitSettings.DefaultMOD),
                ModelFeature.SoundFXGeneration => Api.ElevenLabs,
                ModelFeature.VoiceChanger => Api.ElevenLabs,
                ModelFeature.Realtime => Api.OpenAI,
                _ => Api.OpenAI,
            };
        }

        private static Api GetApi(string modelId)
        {
            if (string.IsNullOrEmpty(modelId)) return Api.OpenAI;
            Model model = modelId;
            if (model == null) return Api.OpenAI;
            return model.Api;
        }


        private static string ResolveDefaultModelId(Api api, ModelFeature cap)
        {
            return api switch
            {
                Api.OpenAI => cap switch
                {
                    ModelFeature.TextGeneration => AIDevKitConfig.kDefault_OpenAI_LLM,
                    ModelFeature.ImageGeneration => AIDevKitConfig.kDefault_OpenAI_IMG,
                    ModelFeature.SpeechGeneration => AIDevKitConfig.kDefault_OpenAI_TTS,
                    ModelFeature.SpeechRecognition => AIDevKitConfig.kDefault_OpenAI_STT,
                    ModelFeature.TextEmbedding => AIDevKitConfig.kDefault_OpenAI_EMB,
                    ModelFeature.Moderation => AIDevKitConfig.kDefault_OpenAI_MOD,
                    _ => AIDevKitConfig.kDefault_OpenAI_LLM,
                },
                Api.Google => cap switch
                {
                    ModelFeature.TextGeneration => AIDevKitConfig.kDefault_Google_LLM,
                    ModelFeature.ImageGeneration => AIDevKitConfig.kDefault_Google_IMG,
                    ModelFeature.TextEmbedding => AIDevKitConfig.kDefault_Google_EMB,
                    _ => AIDevKitConfig.kDefault_Google_LLM,
                },
                Api.ElevenLabs => cap switch
                {
                    ModelFeature.SpeechGeneration => AIDevKitConfig.kDefault_ElevenLabs_TTS,
                    ModelFeature.VoiceChanger => AIDevKitConfig.kDefault_ElevenLabs_VCM,
                    _ => AIDevKitConfig.kDefault_ElevenLabs_TTS,
                },
                _ => null,
            };
        }

        protected override void DrawLibraryButton(Model model, GUIStyle style, float width)
        {
            // if (GUILayout.Button(AIDevKitIcons.ModelLibrary, style, GUILayout.Width(width)))
            // {
            //     ModelCatalogueWindow.ShowWindow();
            // }

            if (GUILayout.Button(EditorIcons.Plus, style, GUILayout.Width(width)))
            {
                ModelCatalogueWindow.ShowWindow();
            }
        }
    }
}