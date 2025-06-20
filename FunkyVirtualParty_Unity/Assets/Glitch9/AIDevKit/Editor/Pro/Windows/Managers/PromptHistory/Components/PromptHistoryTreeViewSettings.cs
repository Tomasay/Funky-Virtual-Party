using Glitch9.Editor;

namespace Glitch9.AIDevKit.Editor.Pro
{
    internal static class PromptHistoryTreeViewSettings
    {
        private static readonly EPrefs<bool> kShowOpenAI = new("AIDevKit.PromptHistoryTreeView.ShowOpenAI", true);
        private static readonly EPrefs<bool> kShowGoogle = new("AIDevKit.PromptHistoryTreeView.ShowGoogle", true);
        private static readonly EPrefs<bool> kShowElevenLabs = new("AIDevKit.PromptHistoryTreeView.ShowElevenLabs", true);
        private static readonly EPrefs<bool> kShowOllama = new("AIDevKit.PromptHistoryTreeView.ShowOllama", true);
        private static readonly EPrefs<bool> kShowOpenRouter = new("AIDevKit.PromptHistoryTreeView.ShowOpenRouter", true);


        public static bool ShowOpenAI { get => kShowOpenAI.Value; set => kShowOpenAI.Value = value; }
        public static bool ShowGoogle { get => kShowGoogle.Value; set => kShowGoogle.Value = value; }
        public static bool ShowElevenLabs { get => kShowElevenLabs.Value; set => kShowElevenLabs.Value = value; }
        public static bool ShowOllama { get => kShowOllama.Value; set => kShowOllama.Value = value; }
        public static bool ShowOpenRouter { get => kShowOpenRouter.Value; set => kShowOpenRouter.Value = value; }
    }
}
