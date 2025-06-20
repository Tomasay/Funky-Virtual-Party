using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.Editor;
using Glitch9.ScriptableObjects;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.ElevenLabs
{
    internal class ElevenLabsSettingsProvider : AIClientSettingsProvider<ElevenLabsSettingsProvider, ElevenLabsSettings>
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            ElevenLabsSettingsProvider provider = new(AIDevKitEditor.Providers.ElevenLabs)
            {
                deactivateHandler = DeactivateHandler,
                keywords = Keywords
            };
            return provider;
        }

        private static void DeactivateHandler()
        {
            ElevenLabsSettings.Instance.SaveAsset();
        }

        private SerializedProperty defaultTTS;
        private SerializedProperty defaultVCM;
        private SerializedProperty defaultVoice;

        public ElevenLabsSettingsProvider(string path) : base(Api.ElevenLabs, true, path) { }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            defaultTTS = serializedObject.FindProperty(nameof(defaultTTS));
            defaultVCM = serializedObject.FindProperty(nameof(defaultVCM));
            defaultVoice = serializedObject.FindProperty(nameof(defaultVoice));
        }

        protected override void DrawOptionalSettings()
        {
            if (ExGUILayout.ButtonField("Subscription Details"))
            {
                AIDevKitEditor.ShowElevenLabsSubscriptionWindow();
            }
        }

        protected override void DrawAdditionalSections()
        {
            ExGUILayout.BeginSection(GUIContents.DefaultAudioModelsAndVoiceSectionTitle);
            {
                AIDevKitGUI.TTSPopup(defaultTTS, Api.ElevenLabs, GUIContents.DefaultTTS);
                AIDevKitGUI.VCMPopup(defaultVCM, Api.ElevenLabs, GUIContents.DefaultVCM);
                AIDevKitGUI.VoicePopup(defaultVoice, Api.ElevenLabs, GUIContents.DefaultVoice);
            }
            ExGUILayout.EndSection();
        }

        protected override void DrawUsefulLinks()
        {
            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Get API Key Guide", "https://glitch9.gitbook.io/ai-development-kit/elevenlabs/getting-the-api-key"),
                ("Create ElevenLabs Account", "https://elevenlabs.io/app/sign-in?redirect=%2Fapp%2Fsettings%2Fapi-keys"),
                ("Manage ElevenLabs API Keys", "https://elevenlabs.io/app/settings/api-keys")
            );

            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Pricing", "https://elevenlabs.io/pricing/api"),
                ("Blog", "https://elevenlabs.io/blog"),
                ("Platform (Console)", "https://elevenlabs.io/app/home"),
                ("Subscription", "https://elevenlabs.io/app/subscription")
            );
        }
    }
}