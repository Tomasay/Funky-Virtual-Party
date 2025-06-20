using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal enum EditorChatType
    {
        [InspectorName("Default Chat")] Default,
        [InspectorName("OpenAI AssistantsAPI")] OpenAI_AssistantsAPI,
        [InspectorName("OpenAI ResponseAPI")] OpenAI_ResponseAPI,
    }

    internal static class EditorChatSettings
    {
        private static readonly EPrefs<bool> kRequirementsMet = new("EditorChat.RequirementsMet", false);

        // Save Data 
        private static readonly EPrefs<string> kCurrentSessionId = new("EditorChat.CurrentSessionId", string.Empty);
        private static readonly EPrefs<Model> kCurrentModel = new("EditorChat.CurrentModel", OpenAIModel.GPT4o);

        // UI States 
        private static readonly EPrefs<bool> kShowTimestampUser = new("EditorChat.ShowTimestampUser", false);
        private static readonly EPrefs<bool> kShowTimestampAI = new("EditorChat.ShowTimestampAI", true);
        private static readonly EPrefs<bool> kShowUsage = new("EditorChat.ShowUsage", false);
        private static readonly EPrefs<bool> kShowUsageTracking = new("EditorChat.ShowUsageTracking", false);
        private static readonly EPrefs<bool> kShowTitleBar = new("EditorChat.ShowTitleBar", true);
        private static readonly EPrefs<bool> kShowToolbar = new("EditorChat.ShowToolbar", true);

        // Global Settings (applied to all chats)
        private static readonly EPrefs<bool> kSaveHistory = new("EditorChat.SaveHistory", true);
        private static readonly EPrefs<bool> kDebugMode = new("EditorChat.DebugMode", false);

        // Debug Settings (shown when kDebugMode(above) is enabled)
        private static readonly EPrefs<bool> kShowDebugToolbar = new("EditorChat.ShowDebugToolbar", false);
        private static readonly EPrefs<bool> kStreamDisabled = new("EditorChat.StreamDisabled", false);
        private static readonly EPrefs<int> kMaxStackTraceLength = new("EditorChat.MaxStackTraceLength", 1000);

        // Text-to-Speech Settings
        private static readonly EPrefs<bool> kTextToSpeechEnabled = new("EditorChat.TextToSpeechEnabled", false);
        private static readonly EPrefs<string> kTextToSpeechModel = new("EditorChat.TextToSpeechModel", AIDevKitConfig.kDefault_OpenAI_TTS);
        private static readonly EPrefs<string> kTextToSpeechVoice = new("EditorChat.TextToSpeechVoice", AIDevKitConfig.kDefault_OpenAI_Voice);

        // Not Used
        private static readonly EPrefs<EditorChatType> kChatType = new("EditorChat.ChatType", EditorChatType.Default);
        private static readonly EPrefs<Model> kAssistantsAPIModel = new("EditorChat.AssistantsAPIModelV2", OpenAIModel.GPT4o);
        private static readonly EPrefs<Model> kResponseAPIModel = new("EditorChat.ResponseAPIModel", OpenAIModel.GPT4o);

        internal static bool RequirementsMet { get => kRequirementsMet.Value; set => kRequirementsMet.Value = value; }
        internal static string CurrentSessionId { get => kCurrentSessionId.Value; set => kCurrentSessionId.Value = value; }
        internal static Model CurrentModel { get => kCurrentModel.Value; set => kCurrentModel.Value = value; }
        internal static Model AssistantsAPIModel { get => kAssistantsAPIModel.Value; set => kAssistantsAPIModel.Value = value; }
        internal static Model ResponseAPIModel { get => kResponseAPIModel.Value; set => kResponseAPIModel.Value = value; }
        internal static EditorChatType ChatType { get => kChatType.Value; set => kChatType.Value = value; }
        internal static bool DebugMode { get => kDebugMode.Value; set => kDebugMode.Value = value; }
        internal static bool SaveHistory { get => kSaveHistory.Value; set => kSaveHistory.Value = value; }
        internal static string CurrentModelName => CurrentModel.SafeGetName();
        internal static bool ShowTimestampUser { get => kShowTimestampUser.Value; set => kShowTimestampUser.Value = value; }
        internal static bool ShowTimestampAI { get => kShowTimestampAI.Value; set => kShowTimestampAI.Value = value; }
        internal static bool ShowUsage { get => kShowUsage.Value; set => kShowUsage.Value = value; }
        internal static bool ShowUsageTracking { get => kShowUsageTracking.Value; set => kShowUsageTracking.Value = value; }

        // Debug Settings
        internal static bool ShowDebugToolbar { get => kShowDebugToolbar.Value; set => kShowDebugToolbar.Value = value; }
        internal static bool DisableStream { get => kStreamDisabled.Value; set => kStreamDisabled.Value = value; }
        internal static int MaxStackTraceLength { get => kMaxStackTraceLength.Value; set => kMaxStackTraceLength.Value = value; }

        // Text-to-Speech Settings
        internal static bool TextToSpeechEnabled { get => kTextToSpeechEnabled.Value; set => kTextToSpeechEnabled.Value = value; }
        internal static string TextToSpeechModel { get => kTextToSpeechModel.Value; set => kTextToSpeechModel.Value = value; }
        internal static string TextToSpeechVoice { get => kTextToSpeechVoice.Value; set => kTextToSpeechVoice.Value = value; }

        internal static bool ShowTitleBar { get => kShowTitleBar.Value; set => kShowTitleBar.Value = value; }
        internal static bool ShowToolbar { get => kShowToolbar.Value; set => kShowToolbar.Value = value; }
    }
}