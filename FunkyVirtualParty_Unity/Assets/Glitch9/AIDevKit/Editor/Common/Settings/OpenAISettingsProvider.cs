using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor;
using Glitch9.ScriptableObjects;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.OpenAI
{
    internal class OpenAISettingsProvider : AIClientSettingsProvider<OpenAISettingsProvider, OpenAISettings>
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            OpenAISettingsProvider provider = new(AIDevKitEditor.Providers.OpenAI)
            {
                deactivateHandler = DeactivateHandler,//OpenAISettings.Instance.Save,
                keywords = Keywords
            };
            return provider;
        }

        private static void DeactivateHandler()
        {
            OpenAISettings.Instance.SaveAsset();
        }

        private SerializedProperty organization;
        private SerializedProperty projectId;
        private SerializedProperty adminKey;

        private SerializedProperty defaultLLM;
        private SerializedProperty defaultIMG;
        private SerializedProperty defaultTTS;
        private SerializedProperty defaultSTT;
        private SerializedProperty defaultEMB;
        private SerializedProperty defaultMOD;
        private SerializedProperty defaultASS;
        private SerializedProperty defaultRTM;
        private SerializedProperty defaultVoice;

        public OpenAISettingsProvider(string path) : base(Api.OpenAI, false, path) { }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            organization = serializedObject.FindProperty(nameof(organization));
            projectId = serializedObject.FindProperty(nameof(projectId));
            adminKey = serializedObject.FindProperty(nameof(adminKey));

            defaultLLM = serializedObject.FindProperty(nameof(defaultLLM));
            defaultIMG = serializedObject.FindProperty(nameof(defaultIMG));
            defaultTTS = serializedObject.FindProperty(nameof(defaultTTS));
            defaultSTT = serializedObject.FindProperty(nameof(defaultSTT));
            defaultEMB = serializedObject.FindProperty(nameof(defaultEMB));
            defaultMOD = serializedObject.FindProperty(nameof(defaultMOD));
            defaultASS = serializedObject.FindProperty(nameof(defaultASS));
            defaultRTM = serializedObject.FindProperty(nameof(defaultRTM));

            defaultVoice = serializedObject.FindProperty(nameof(defaultVoice));
        }

        protected override void DrawOptionalSettings()
        {
            EditorGUILayout.PropertyField(adminKey, GUIContents.AdminApiKeyLabel);
            EditorGUILayout.PropertyField(organization, GUIContents.ApiOrganizationLabel);
            EditorGUILayout.PropertyField(projectId, GUIContents.ApiProjectIdLabel);
        }

        protected override void DrawAdditionalSections()
        {
            Api api = Api.OpenAI;

            ExGUILayout.BeginSection(GUIContents.DefaultModelsSectionTitle);
            {
                AIDevKitGUI.LLMPopup(defaultLLM, api, GUIContents.DefaultLLM);
                AIDevKitGUI.IMGPopup(defaultIMG, api, GUIContents.DefaultIMG);
                AIDevKitGUI.EMBPopup(defaultEMB, api, GUIContents.DefaultEMB);
                AIDevKitGUI.MODPopup(defaultMOD, api, GUIContents.DefaultMOD);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection(GUIContents.DefaultAudioModelsAndVoiceSectionTitle);
            {
                AIDevKitGUI.STTPopup(defaultSTT, api, GUIContents.DefaultSTT);
                AIDevKitGUI.TTSPopup(defaultTTS, api, GUIContents.DefaultTTS);
                AIDevKitGUI.VoicePopup(defaultVoice, api, GUIContents.DefaultVoice);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection(GUIContents.DefaultModelsForAdvancedAPISectionTitle);
            {
                AIDevKitGUI.LLMPopup(defaultASS, api, GUIContents.DefaultASS);
                AIDevKitGUI.RTMPopup(defaultRTM, api, GUIContents.DefaultRTM);
            }
            ExGUILayout.EndSection();
        }

        protected override void DrawUsefulLinks()
        {
            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Get API Key Guide", "https://glitch9.gitbook.io/ai-development-kit/introduction/getting-the-api-key"),
                ("Create OpenAI Account", "https://platform.openai.com/signup"),
                ("Manage OpenAI API Keys", "https://platform.openai.com/api-keys")
            );

            AIDevKitGUI.Presets.DrawUrlButtons(
                ("OpenAI Billing", "https://platform.openai.com/settings/organization/billing/overview"),
                ("OpenAI API Status", "https://status.openai.com/"),
                ("OpenAI API Pricing", "https://openai.com/pricing")
            );
        }
    }
}