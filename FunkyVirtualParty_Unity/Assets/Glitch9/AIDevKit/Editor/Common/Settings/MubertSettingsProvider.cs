using Glitch9.AIDevKit.Mubert;
using Glitch9.ScriptableObjects;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.Mubert
{
    internal class MubertSettingsProvider : AIClientSettingsProvider<MubertSettingsProvider, MubertSettings>
    {
        //[SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            MubertSettingsProvider provider = new(AIDevKitEditor.Providers.Mubert)
            {
                deactivateHandler = DeactivateHandler,
                keywords = Keywords
            };
            return provider;
        }

        private static void DeactivateHandler()
        {
            MubertSettings.Instance.SaveAsset();
        }

        public MubertSettingsProvider(string path) : base(Api.Mubert, true, path)
        {
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();
        }

        protected override void DrawOptionalSettings()
        {

        }
        protected override void DrawAdditionalSections()
        {

        }
        protected override void DrawUsefulLinks()
        {
            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Get API Key Guide", ""),
                ("Create Mubert Account", "https://mubertapp.typeform.com/to/p6CzphzX?utm_source=Website&typeform-source=landing.mubert.com#page=Website"),
                ("Manage Mubert API Keys", "https://landing.mubert.com/")
            );

            AIDevKitGUI.Presets.DrawUrlButtons(
                ("Pricing", "https://landing.mubert.com/#Pricing"),
                ("Platform", "https://mubert.com/render?utm_source=redirect&utm_medium=typeform&utm_campaign=api_form")
            );
        }


    }
}