using Glitch9.Editor;

namespace Glitch9.AIDevKit.Editor
{
    internal class AIDevKitEditorSettings
    {
        private static readonly EPrefs<bool> kAutoUpdateModels = new("AIDevKit.AutoUpdateModels", true);
        private static readonly EPrefs<bool> kAutoDeleteDeprecatedModels = new("AIDevKit.AutoDeleteDeprecatedModels", true);

        internal static bool AutoUpdateModels { get => kAutoUpdateModels.Value; set => kAutoUpdateModels.Value = value; }
        internal static bool AutoDeleteDeprecatedModels { get => kAutoDeleteDeprecatedModels.Value; set => kAutoDeleteDeprecatedModels.Value = value; }
    }
}