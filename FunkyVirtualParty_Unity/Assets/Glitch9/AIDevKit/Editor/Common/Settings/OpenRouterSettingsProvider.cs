using Glitch9.AIDevKit.OpenRouter;
using Glitch9.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.OpenRouter
{
    internal class OpenRouterSettingsProvider : AIClientSettingsProvider<OpenRouterSettingsProvider, OpenRouterSettings>
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            OpenRouterSettingsProvider provider = new(AIDevKitEditor.Providers.OpenRouter)
            {
                deactivateHandler = DeactivateHandler,//OpenRouterSettings.Instance.Save,
                keywords = Keywords
            };
            return provider;
        }

        private static void DeactivateHandler()
        {
            OpenRouterSettings.Instance.SaveAsset();
        }


        private readonly static GUIContent kHttpRefererLabel = new("HTTP-Referer (Optional)", "Site URL for rankings on openrouter.ai.");
        private readonly static GUIContent kXTitleLabel = new("X-Title (Optional)", "Site title for rankings on openrouter.ai.");

        private SerializedProperty httpReferer;
        private SerializedProperty xTitle;
        private SerializedProperty defaultLLM;

        public OpenRouterSettingsProvider(string path) : base(Api.OpenRouter, true, path) { }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            httpReferer = serializedObject.FindProperty(nameof(httpReferer));
            xTitle = serializedObject.FindProperty(nameof(xTitle));
            defaultLLM = serializedObject.FindProperty(nameof(defaultLLM));
        }


        protected override void DrawOptionalSettings()
        {
            EditorGUILayout.PropertyField(httpReferer, kHttpRefererLabel);
            EditorGUILayout.PropertyField(xTitle, kXTitleLabel);
            AIDevKitGUI.LLMPopup(defaultLLM, Api.OpenRouter, GUIContents.ApiDefaultModel);
        }

        protected override void DrawUsefulLinks()
        {
            AIDevKitGUI.Presets.DrawUrlButtons(
                ("OpenRouter Website", "https://openrouter.ai/"),
                ("OpenRouter API Key", "https://openrouter.ai/settings/keys"),
                ("OpenRouter Billing", "https://openrouter.ai/settings/credits")
            );

            AIDevKitGUI.Presets.DrawUrlButtons(
                ("OpenRouter Models", "https://openrouter.ai/models"),
                ("OpenRouter LLM Rankings", "https://openrouter.ai/rankings"),
                ("OpenRouter Documents", "https://openrouter.ai/docs/quickstart")
            );
        }
    }
}