using Glitch9.AIDevKit.Google;
using Glitch9.Editor;
using Glitch9.ScriptableObjects;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor
{
    internal class GenerativeAISettingsProvider : AIClientSettingsProvider<GenerativeAISettingsProvider, GenerativeAISettings>
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            GenerativeAISettingsProvider provider = new(AIDevKitEditor.Providers.Google)
            {
                deactivateHandler = DeactivateHandler,
                keywords = Keywords
            };
            return provider;
        }

        private static void DeactivateHandler()
        {
            GenerativeAISettings.Instance.SaveAsset();
        }

        private SerializedProperty projectId;
        private SerializedProperty defaultLLM;
        private SerializedProperty defaultEMB;
        private SerializedProperty defaultIMG;
        private SerializedProperty defaultVID;
        private SerializedProperty defaultTTS;
        private SerializedProperty defaultVoice;

        public GenerativeAISettingsProvider(string path) : base(Api.Google, false, path) { }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            projectId = serializedObject.FindProperty(nameof(projectId));

            defaultLLM = serializedObject.FindProperty(nameof(defaultLLM));
            defaultEMB = serializedObject.FindProperty(nameof(defaultEMB));
            defaultIMG = serializedObject.FindProperty(nameof(defaultIMG));
            defaultVID = serializedObject.FindProperty(nameof(defaultVID));

            defaultTTS = serializedObject.FindProperty(nameof(defaultTTS));
            defaultVoice = serializedObject.FindProperty(nameof(defaultVoice));
        }

        protected override void DrawOptionalSettings()
        {
            EditorGUILayout.PropertyField(projectId, GUIContents.ApiProjectIdLabel);
        }

        protected override void DrawAdditionalSections()
        {
            ExGUILayout.BeginSection(GUIContents.DefaultModelsSectionTitle);
            {
                AIDevKitGUI.LLMPopup(defaultLLM, Api.Google, GUIContents.DefaultLLM);
                AIDevKitGUI.EMBPopup(defaultEMB, Api.Google, GUIContents.DefaultEMB);
                AIDevKitGUI.IMGPopup(defaultIMG, Api.Google, GUIContents.DefaultIMG);
                AIDevKitGUI.VIDPopup(defaultVID, Api.Google, GUIContents.DefaultVID);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection(GUIContents.DefaultAudioModelsAndVoiceSectionTitle);
            {
                AIDevKitGUI.TTSPopup(defaultTTS, Api.Google, GUIContents.DefaultTTS);
                AIDevKitGUI.VoicePopup(defaultVoice, Api.Google, GUIContents.DefaultVoice);
            }
            ExGUILayout.EndSection();
        }

        protected override void DrawUsefulLinks()
        {
            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Get API Key Guide", "https://ai.google.dev/gemini-api/docs/api-key"),
                ("Create Google Account", "https://myaccount.google.com/"),
                ("Manage Google API Keys", "https://aistudio.google.com/app/apikey")
            );

            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Google Cloud Console", "https://console.cloud.google.com/"),
                ("Google AI Studio", "https://aistudio.google.com/")
            );
        }
    }
}