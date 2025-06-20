using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Ollama;
using Glitch9.Editor;
using Glitch9.Internal;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class AIDevKitEditor
    {
        internal const string PackageName = "AI Dev Kit";
        internal const string OnlineDocUrl = "https://glitch9.gitbook.io/ai-dev-kit/";
        private static readonly EPrefs<CurrencyCode> kCurrencyCode = new("AIDevKit.Editor.CurrencyCode", CurrencyCode.USD);
        internal static CurrencyCode DisplayedCurrencyCode { get => kCurrencyCode.Value; set => kCurrencyCode.Value = value; }

        internal static class Providers
        {
            internal const string BasePath = "Preferences/" + PackageName;
            internal const string OpenAI = BasePath + "/OpenAI";
            internal const string Google = BasePath + "/Google Gemini";
            internal const string ElevenLabs = BasePath + "/Eleven Labs";
            internal const string Mubert = BasePath + "/Mubert";
            internal const string Ollama = BasePath + "/Ollama";
            internal const string OpenRouter = BasePath + "/OpenRouter";
        }

        internal static class Orders
        {
            private const int Start = 0;

            internal const int CommonSettings = Start;
            internal const int OpenAISettings = CommonSettings + 1;
            internal const int GeminiSettings = OpenAISettings + 1;
            internal const int LogsRepo = GeminiSettings + 1;
            internal const int ModelsRepo = LogsRepo + 1;
            internal const int FilesRepo = ModelsRepo + 1;
            internal const int ModelMetadata = FilesRepo + 1;
        }

        internal static class Labels
        {
            // --- Editor Tools ---
            internal const string EditorChat = "AI Dev Kit Assistant";
            internal const string GeneratorHub = "AI Dev Kit Generator";

            // internal const string EditorVision = "Editor Vision";
            // internal const string EditorSpeech = "Editor Speech";

            // --- Shared Managing Tools ---
            internal const string ModelCatalogue = "Model Library";
            internal const string VoiceCatalogue = "Voice Library";

            // --- OpenAI Tools ---
            internal const string PromptHistory = "Prompt History";
            internal const string ChatSessionLibrary = "Chat Sessions";
            internal const string OpenAIAssistants = "Assistants (OpenAI)";
            internal const string FileManager = "Uploaded Files";

            // --- Supports ---
            internal const string OnlineDoc = "Online Documentation";
            internal const string JoinDiscord = "Join Discord Server";

            // --- Preferences ---  
            internal const string Preferences = "Preferences";
        }

        internal static class Paths
        {
            private const string Tools = "Tools/" + PackageName + "/";

            // --- Editor Tools --- 
            internal const string EditorChat = Tools + Labels.EditorChat + " %#e";
            // internal const string EditorVision = Tools + Labels.EditorVision;
            // internal const string EditorSpeech = Tools + Labels.EditorSpeech;
            internal const string GeneratorHub = Tools + Labels.GeneratorHub + " %#q";

            // --- Shared Managing Tools --- 
            internal const string ModelCatalogue = Tools + Labels.ModelCatalogue;
            internal const string VoiceCatalogue = Tools + Labels.VoiceCatalogue;

            // --- OpenAI Tools ---
            internal const string PromptHistory = Tools + Labels.PromptHistory;
            internal const string ChatSessionLibrary = Tools + Labels.ChatSessionLibrary;
            internal const string OpenAIAssistantManager = Tools + Labels.OpenAIAssistants;
            internal const string OpenAIFileManager = Tools + Labels.FileManager;

            // --- Supports ---
            internal const string OnlineDoc = Tools + Labels.OnlineDoc;
            internal const string JoinDiscord = Tools + Labels.JoinDiscord;

            // --- Preferences --- 
            internal const string Preferences = Tools + Labels.Preferences;
        }

        internal static class Priorities
        {
            internal const int kAIDevKitPriority = 500;

            // --- Editor Tools ---
            internal const int EditorChat = kAIDevKitPriority;
            // internal const int EditorVision = EditorChat + 1;
            // internal const int EditorSpeech = EditorVision + 1;
            internal const int GeneratorHub = EditorChat + 1;

            // --- Shared Managing Tools ---
            internal const int ModelCatalogue = GeneratorHub + 15;
            internal const int VoiceCatalogue = ModelCatalogue + 1;

            // --- OpenAI Tools ---
            internal const int PromptHistory = VoiceCatalogue + 15;
            internal const int ChatSessionLibrary = PromptHistory + 1;
            internal const int OpenAIAssistantManager = ChatSessionLibrary + 1;
            internal const int FileManager = OpenAIAssistantManager + 1;

            // --- Supports ---
            internal const int OnlineDoc = FileManager + 15;
            internal const int JoinDiscord = OnlineDoc + 1;

            // --- Preferences --- 
            internal const int Preferences = JoinDiscord + 15;
        }


        [MenuItem(Paths.EditorChat, priority = Priorities.EditorChat)]
        public static void OpenEditorChat() => ShowProWindow(onShowEditorChatWindow);

        // [MenuItem(Paths.EditorVision, priority = Priorities.EditorVision)]
        // public static void OpenEditorVision() => ShowProWindow(onShowEditorVisionWindow);

        // [MenuItem(Paths.EditorSpeech, priority = Priorities.EditorSpeech)]
        // public static void OpenEditorSpeech() => ShowProWindow(onShowEditorSpeechWindow);

        // Generator Hub
        [MenuItem(Paths.GeneratorHub, priority = Priorities.GeneratorHub)]
        public static void OpenGeneratorHub() => ShowProWindow(onShowGeneratorHubWindow);

        [MenuItem(Paths.OpenAIAssistantManager, priority = Priorities.OpenAIAssistantManager)]
        public static void OpenOpenAIAssistantManager() => ShowProWindow(onShowOpenAIAssistantManagerWindow);

        [MenuItem(Paths.OpenAIFileManager, priority = Priorities.FileManager)]
        public static void OpenOpenAIFileManager() => ShowProWindow(onShowFileManagerWindow);

        [MenuItem(Paths.OnlineDoc, priority = Priorities.OnlineDoc)]
        public static void OpenDocumentURL() => Application.OpenURL(OnlineDocUrl);

        [MenuItem(Paths.JoinDiscord, priority = Priorities.JoinDiscord)]
        public static void OpenDiscordURL() => Application.OpenURL(EditorConfig.DiscordUrl);

        [MenuItem(Paths.PromptHistory, priority = Priorities.PromptHistory)]
        public static void OpenPromptHistory() => ShowProWindow(onShowPromptHistoryWindow);

        [MenuItem(Paths.Preferences, priority = Priorities.Preferences)]
        public static void ShowPreferencesWindow() => SettingsService.OpenUserPreferences(Providers.BasePath);

        [MenuItem(Paths.ChatSessionLibrary, priority = Priorities.ChatSessionLibrary)]
        public static void OpenChatbotLibrary() => ShowProWindow(onShowChatbotLibraryWindow);



        #region Pro Version Delegates (delegates for using Pro version assembly)

#pragma warning disable IDE1006
        internal static event Action onShowEditorChatWindow;
        // internal static event Action onShowEditorVisionWindow;
        // internal static event Action onShowEditorSpeechWindow;
        internal static event Action onShowGeneratorHubWindow;
        internal static event Action onShowPromptHistoryWindow;
        internal static event Action onShowFileManagerWindow;
        internal static event Action onShowChatbotLibraryWindow;
        internal static event Action onShowOpenAIAssistantManagerWindow;
        internal static event Action onShowElevenLabsSubscriptionWindow;
        internal static event Func<UniTask<bool>> isElevenLabsFreeTierPredicateAsync;

#pragma warning restore IDE1006

        #endregion Pro Version Delegates  
        private static bool? _isElevenLabsFreeTier;

        internal static async UniTask<bool> IsElevenLabsFreeTierAsync()
        {
            if (AIDevKitConfig.IsPro)
            {
                if (_isElevenLabsFreeTier.HasValue) return _isElevenLabsFreeTier.Value;

                if (isElevenLabsFreeTierPredicateAsync == null) throw new NullReferenceException("isElevenLabsFreeTierPredicateAsync is null");

                _isElevenLabsFreeTier = await isElevenLabsFreeTierPredicateAsync.Invoke();
                if (_isElevenLabsFreeTier == null) throw new NullReferenceException("isElevenLabsFreeTierPredicateAsync returned null");

                return _isElevenLabsFreeTier.Value;
            }
            else
            {
                LogProVersionRequired();
                return false;
            }
        }

        internal static void SetIsElevenLabsFreeTier(bool isFreeTier) => _isElevenLabsFreeTier = isFreeTier;

        internal static void ShowProWindow(Action delegateAction)
        {
            if (AIDevKitConfig.IsPro)
            {
                if (delegateAction == null)
                {
                    ShowDialog.Error("There was an error while trying to open a this window. Please try re-compiling the project or restarting Unity.");
                    return;
                }
                delegateAction();
            }
            else
            {
                ShowNoProVersionDialog();
            }
        }

        internal static void ShowElevenLabsSubscriptionWindow()
        {
            if (AIDevKitConfig.IsPro)
            {
                onShowElevenLabsSubscriptionWindow?.Invoke();
            }
            else
            {
                ShowNoProVersionDialog();
            }
        }

        internal static void ShowNoProVersionDialog()
        {
            if (EditorUtility.DisplayDialog("Pro Version Required", "This feature is only available in the Pro version of the AI Dev Kit.", "Get Pro", "Cancel"))
            {
                OpenProURL();
            }
        }

        internal static void OpenProURL()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/ai-ml-integration/ai-development-kit-gpt4o-assistants-api-v2-281225");
        }

        private static void LogProVersionRequired()
        {
            Debug.LogWarning("This feature is only available in the Pro version of the AI DevKit.");
        }

        internal static async void CheckOllamaServerStatus()
        {
            try
            {
                if (await OllamaSettings.CheckConnectionAsync())
                {
                    ShowDialog.Info("Ollama server is running.");
                }
                else
                {
                    ShowDialog.Info("Ollama server is not running. Please start the server.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CheckOllamaServerStatus: {e.Message}\n{e.StackTrace}");
                ShowDialog.Info($"Failed to check Ollama server status: {e.Message}");
            }
        }
    }
}