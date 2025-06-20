namespace Glitch9.Internal
{
    internal class EditorConfig
    {
        internal const string kToolsPath = "Tools/";
        internal const string kRootUserPreferencePath = "Preferences/Glitch9/";

        private const int kStartingCreateMenuOrder = 0;
        internal const int kStartingToolsMenuPriority = -5000;

        internal const string GithubSupportUrl = "https://github.com/Glitch9Inc/Glitch9-Support";
        internal const string DiscordUrl = "https://discord.gg/hgajxPpJYf";


        internal const int kAIDevKitPriority = 500;
        internal const int kAIDevKitLocalizationPriority = kAIDevKitPriority + 5;


        internal static class NativeMediaPlayer
        {
            internal const string NAME = "Native Media Player";
            internal const int RELEASE_YEAR = 2022;
            private const string TOOLS_PATH = kToolsPath + NAME + "/";

            internal const string PATH_ANDROID12_PREVIEW = TOOLS_PATH + "Android 12 Preview";
            internal const string PATH_IOS13_PREVIEW = TOOLS_PATH + "iOS 13 Preview";
            internal const string PATH_PREFERENCES = TOOLS_PATH + "Preferences";
            internal const string PATH_DOCUMENTATION = TOOLS_PATH + "Documentation";

            // Priorities
            internal const int PRIORITY_ANDROID12_PREVIEW = kStartingToolsMenuPriority;
            internal const int PRIORITY_IOS13_PREVIEW = PRIORITY_ANDROID12_PREVIEW + 1;

            internal const int PRIORITY_PREFERENCES = PRIORITY_IOS13_PREVIEW + 15;
            internal const int PRIORITY_DOCUMENTATION = PRIORITY_PREFERENCES + 1;

            internal const string URL_DOCUMENTATION = "https://glitch9.gitbook.io/native-media-player";
            internal const string PROVIDER_SETTINGS = kRootUserPreferencePath + NAME;
        }

        internal static class CommitGen // Commit-Gen (Only has 1 tool)
        {
            internal const int PRIORITY = kStartingToolsMenuPriority - 15;
            internal const string TOOL_PATH = kToolsPath + "Commit Message Generator";
        }


        #region Not Available to Internal yet

        internal static class Game
        {
            private const string NAME = "Game";
            private const string TOOLS_PATH = kToolsPath + NAME + "/";


            internal const int ORDER_CREATE_GAME_SETTINGS = kStartingCreateMenuOrder;
            internal const int ORDER_CREATE_ITEM_SETTINGS = ORDER_CREATE_GAME_SETTINGS + 1;
            internal const int ORDER_CREATE_SEASON_PASS_SETTINGS = ORDER_CREATE_ITEM_SETTINGS + 1;
        }

        internal static class GoogleSheets
        {
            private const string NAME = "Google Sheets";
            private const string TOOL_PATH = kToolsPath + NAME + "/";

            internal const string NAME_GOOGLE_SHEETS_MANAGER = "Google Sheets Manager";
            internal const string PATH_GOOGLE_SHEETS_MANAGER = TOOL_PATH + NAME_GOOGLE_SHEETS_MANAGER;
            internal const int PRIORITY_GOOGLE_SHEETS_MANAGER = kStartingToolsMenuPriority;
        }

        internal static class AIDialogGenerator
        {
            private const string NAME = "AI Dialog Generator";
            private const string TOOL_PATH = kToolsPath + NAME + "/";

            internal const string NAME_AI_DIALOG_GENERATOR = "AI Dialog Generator";
            internal const string PATH_AI_DIALOG_GENERATOR = TOOL_PATH + NAME_AI_DIALOG_GENERATOR;
            internal const int PRIORITY_AI_DIALOG_GENERATOR = kStartingToolsMenuPriority;
        }


        #endregion
    }
}
