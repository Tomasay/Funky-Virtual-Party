using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.AIDevKit.Editor.Chatbots;
using Glitch9.AIDevKit.Editor.Pro;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using Glitch9.Internal;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class EditorChatSettingsWindow : EditorWindow
    {
        private static class Labels
        {
            internal static readonly GUIContent ShowSessionDetails = new("Show Session Details", "Open the details of the current chat session.");
            internal static readonly GUIContent StartNewSession = new("Start New Session", "Create and start a new chat session.");
            internal static readonly GUIContent DeleteCurrentSession = new("Delete Current Session", "Permanently delete the current chat session.");
            internal static readonly GUIContent RefreshSessionList = new("Refresh Session List", "Reload the list of available chat sessions.");
        }

        // internal static EditorChatSettingsWindow Instance { get; private set; }


        internal static void ShowWindow(EditorChatWindow window)
        {
            var w = GetWindow<EditorChatSettingsWindow>(EditorChatConfig.SettingsWindowTitle);
            w._window = window;
        }

        private EditorChatWindow _window;
        private Vector2 _scrollPos;
        private void OnGUI()
        {
            GUILayout.BeginVertical(AIDevKitStyles.PopupWindow);
            {
                EditorGUIUtility.labelWidth = EditorChatConfig.kSettingsLabelWidth;

                try
                {
                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                    try
                    {
                        DrawChatSessionSettings();
                        DrawEditorChatSettings();
                        DrawVoiceSettings();
                        DrawDebugSettings();
                    }
                    finally
                    {
                        EditorGUILayout.EndScrollView();
                    }

                    EditorGUILayout.Space(10);

                    if (EditorGUILayout.LinkButton("Join Discord", GUILayout.ExpandWidth(true)))
                    {
                        Application.OpenURL(EditorConfig.DiscordUrl);
                    }
                }
                finally
                {
                    EditorGUIUtility.labelWidth = 0;
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawChatSessionSettings()
        {
            GUILayout.Label("Current Session", EditorStyles.boldLabel);
            GUILayout.BeginVertical(ExStyles.helpBox);
            try
            {
                string selectedId = EditorChatSettings.CurrentSessionId;
                ChatSession session = null;

                GUILayout.BeginHorizontal();
                try
                {
                    //GUILayout.Label("Chat Session", GUILayout.Width(100f));
                    session = AIDevKitProGUI.ChatSessionPopup(selectedId, ChatSessionManager.EditorSessions);

                    if (session != null && session.Id != selectedId)
                    {
                        session.OnTitleChanged += _ => ChatSessionManager.ReloadSessions();
                        EditorChatSettings.CurrentSessionId = session.Id;
                        _window.SetChatSession(session);
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(5);

                if (GUILayout.Button(Labels.ShowSessionDetails))
                {
                    if (session == null)
                    {
                        Debug.LogWarning("No chat session selected.");
                    }
                    else
                    {
                        var window = CreateWindow<ChatbotManagerWindow.ChatbotManagerTreeViewDetailsWindow>();
                        window.SetData(session);
                    }
                }


                if (GUILayout.Button(Labels.RefreshSessionList))
                {
                    ChatSessionManager.ReloadSessions();
                    Repaint();
                }

                if (GUILayout.Button(Labels.StartNewSession))
                {
                    _window.StartNewChat();
                }

                if (GUILayout.Button(Labels.DeleteCurrentSession))
                {
                    if (session == null)
                    {
                        Debug.LogWarning("No chat session selected.");
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Remove Chat Session", $"Are you sure you want to remove the chat session '{session.Id}'?", "Yes", "No"))
                        {
                            ChatSessionManager.DeleteSession(session);
                            var lastSession = ChatSessionManager.GetLastEditorSession();

                            if (lastSession == null)
                            {
                                _window.StartNewChat();
                            }
                            else
                            {
                                _window.SetChatSession(lastSession);
                            }
                        }
                    }
                }
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }

        private void DrawEditorChatSettings()
        {
            GUILayout.Label(EditorChatConfig.SettingsWindowTitle, EditorStyles.boldLabel);
            GUILayout.BeginVertical(ExStyles.helpBox);
            try
            {
                // EditorChatSettings.ChatType = (EditorChatType)EditorGUILayout.EnumPopup(
                //                label: new GUIContent("Chat Mode"),
                //                selected: EditorChatSettings.ChatType,
                //                checkEnabled: ChatTypeCheckEnabled,
                //                includeObsolete: false);

                // if (_initialChatType != EditorChatSettings.ChatType)
                // {
                //     ExGUILayout.StatusBox("Close the window and reopen it to apply changes.", MessageStatus.Warning);
                // }

                bool newShowToolbar = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Toolbar", "Display the toolbar at the top of the chat window."),
                    EditorChatSettings.ShowToolbar);

                bool newShowTitleBar = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Title Bar", "Display the title bar at the top of the chat window."),
                    EditorChatSettings.ShowTitleBar);

                bool newShowTimestampUser = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Timestamp on User Messages", "Display timestamps for each message in the chat."),
                    EditorChatSettings.ShowTimestampUser);

                bool newShowTimestampAI = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Timestamp on AI Messages", "Display timestamps for each message in the chat."),
                    EditorChatSettings.ShowTimestampAI);

                bool newShowUsage = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Usage on AI Messages", "Display token usage information for each message in the chat."),
                    EditorChatSettings.ShowUsage);

                bool newShowUsageTracking = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Usage Tracking Toolbar", "Display usage tracking information for the chat session."),
                    EditorChatSettings.ShowUsageTracking);

                if (newShowToolbar != EditorChatSettings.ShowToolbar)
                {
                    EditorChatSettings.ShowToolbar = newShowToolbar;
                    _window.RebuildUI();
                }

                if (newShowTitleBar != EditorChatSettings.ShowTitleBar)
                {
                    EditorChatSettings.ShowTitleBar = newShowTitleBar;
                    _window.RebuildUI();
                }

                if (newShowTimestampUser != EditorChatSettings.ShowTimestampUser)
                {
                    EditorChatSettings.ShowTimestampUser = newShowTimestampUser;
                    _window.RebuildChatScrollView();
                }

                if (newShowTimestampAI != EditorChatSettings.ShowTimestampAI)
                {
                    EditorChatSettings.ShowTimestampAI = newShowTimestampAI;
                    _window.RebuildChatScrollView();
                }

                if (newShowUsage != EditorChatSettings.ShowUsage)
                {
                    EditorChatSettings.ShowUsage = newShowUsage;
                    _window.RebuildChatScrollView();
                }

                if (newShowUsageTracking != EditorChatSettings.ShowUsageTracking)
                {
                    EditorChatSettings.ShowUsageTracking = newShowUsageTracking;
                    _window.RebuildChatScrollView();
                }

                EditorChatSettings.SaveHistory = ExGUILayout.AnimatedToggleLeft(GUIContents.SaveHistory, EditorChatSettings.SaveHistory);

                GUILayout.Space(5);

                if (GUILayout.Button("Open Save Folder"))
                {
                    if (!AIDevKitGenerateMenu.OpenChatSaveFolder())
                    {
                        ExGUILayout.HelpBoxExBig("Chat save folder not found.", MessageTypeEx.Warning);
                    }
                }

                if (GUILayout.Button("Open Preferences"))
                {
                    SettingsService.OpenUserPreferences(AIDevKitEditor.Providers.BasePath);
                }
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }

        private void DrawVoiceSettings()
        {
            //GUILayout.Label("Voice Assistant Settings", EditorStyles.boldLabel);
            bool newTTSEnabled = TreeViewGUI.BeginSectionWithToggle("Voice Assistant", EditorChatSettings.TextToSpeechEnabled);
            //GUILayout.BeginVertical(ExStyles.helpBox);
            try
            {
                EditorGUI.BeginDisabledGroup(!EditorChatSettings.TextToSpeechEnabled);
                EditorChatSettings.TextToSpeechModel = AIDevKitGUI.TTSPopup(
                    label: new GUIContent("TTS Model", "The model used for text-to-speech conversion."),
                    selected: EditorChatSettings.TextToSpeechModel);

                Model selected = EditorChatSettings.TextToSpeechModel;
                Api voiceApi = Api.All;
                if (selected != null) voiceApi = selected.Api;

                EditorChatSettings.TextToSpeechVoice = AIDevKitGUI.VoicePopup(
                    label: new GUIContent("TTS Voice", "The voice used for text-to-speech conversion."),
                    api: voiceApi,
                    selected: EditorChatSettings.TextToSpeechVoice);
            }
            finally
            {
                EditorGUI.EndDisabledGroup();
                //GUILayout.EndVertical();
                TreeViewGUI.EndSection();

                if (newTTSEnabled != EditorChatSettings.TextToSpeechEnabled)
                {
                    EditorChatSettings.TextToSpeechEnabled = newTTSEnabled;
                    _window.Repaint();
                }
            }
        }

        private void DrawDebugSettings()
        {
            bool newDebugMode = TreeViewGUI.BeginSectionWithToggle("Debug Mode", EditorChatSettings.DebugMode);
            //GUILayout.BeginVertical(ExStyles.helpBox);
            try
            {
                EditorGUI.BeginDisabledGroup(!EditorChatSettings.DebugMode);
                EditorChatSettings.ShowDebugToolbar = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Show Debug Toolbar", "Display the debug toolbar for additional debugging options."),
                    EditorChatSettings.ShowDebugToolbar);

                EditorChatSettings.DisableStream = ExGUILayout.AnimatedToggleLeft(
                    new GUIContent("Disable Stream"),
                    EditorChatSettings.DisableStream);

                EditorChatSettings.MaxStackTraceLength = EditorGUILayout.IntSlider(
                    new GUIContent("Max Stack Trace Length", "Maximum length of the stack trace for debugging."),
                    EditorChatSettings.MaxStackTraceLength, 1000, 50000);
            }
            finally
            {
                EditorGUI.EndDisabledGroup();
                TreeViewGUI.EndSection();
                if (newDebugMode != EditorChatSettings.DebugMode)
                {
                    EditorChatSettings.DebugMode = newDebugMode;
                    if (!newDebugMode) EditorChatSettings.DisableStream = false;
                    _window.Repaint();
                }
                //GUILayout.EndVertical();
            }
        }
    }
}