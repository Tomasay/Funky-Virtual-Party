using Glitch9.AIDevKit.Editor.Pro;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class ChatIMGUIToolbar
    {
        private const float kBtnWidth = 32f;
        private readonly EditorChatWindow _window;
        private static readonly GUIContent kNewChat = new(EditorIcons.Plus, "Start a new chat session.");
        internal ChatIMGUIToolbar(EditorChatWindow window) => _window = window;

        internal void DrawToolbar()
        {
            DrawPrimaryToolbar();

            // if (EditorChatSettings.ShowUsageTracking)
            // {
            //     DrawUsageTrackingToolbar();
            // }
            // 위 Usage바는 화면 하단에 UIToolkit을 베이스로한 바로 변경됨.

            if (EditorChatSettings.ShowDebugToolbar && EditorChatSettings.DebugMode)
            {
                DrawDebugToolbar();
            }
        }

        private void DrawPrimaryToolbar()
        {
            if (_window == null) return;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button(kNewChat, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                {
                    //_window.RebuildChatArea();
                    _window.StartNewChat();
                }

                if (EditorChatSettings.ChatType == EditorChatType.OpenAI_AssistantsAPI)
                {
                    Model model = EditorChatSettings.AssistantsAPIModel;
                    Model newModel = AIDevKitGUI.LLMPopup(model, Api.OpenAI, style: PopupGUIStyle.Toolbar);
                    if (newModel != model) EditorChatSettings.AssistantsAPIModel = newModel.Id;
                }
                else if (EditorChatSettings.ChatType == EditorChatType.OpenAI_ResponseAPI)
                {
                    Model model = EditorChatSettings.ResponseAPIModel;
                    Model newModel = AIDevKitGUI.LLMPopup(model, Api.OpenAI, style: PopupGUIStyle.Toolbar);
                    if (newModel != model) EditorChatSettings.ResponseAPIModel = newModel.Id;
                }
                else
                {
                    EditorChatSettings.CurrentModel = AIDevKitGUI.LLMPopup(
                        selected: EditorChatSettings.CurrentModel,
                        style: PopupGUIStyle.Toolbar
                    );
                }

                if (GUILayout.Button(EditorChatConfig.kToolbarSaveAsTextLabel, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                {
                    _window.SaveChatAsText();
                }

                if (GUILayout.Button(EditorChatConfig.kToolbarSettingsLabel, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                {
                    EditorChatSettingsWindow.ShowWindow(_window);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawUsageTrackingToolbar()
        {
            if (_window == null) return;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            try
            {
                Usage totalUsage = _window.TotalUsage;
                if (totalUsage == null)
                {
                    GUILayout.Label("No usage data available.", EditorStyles.toolbarButton);
                }
                else
                {
                    string[] parts = totalUsage.ToInspectorTextParts();
                    foreach (string part in parts)
                    {
                        GUILayout.Label(part, EditorStyles.toolbarButton);
                    }
                    GUILayout.Label($"$ {_window.TotalCostInUSD}", EditorStyles.toolbarButton);

                    // btn to open prompt history
                    if (GUILayout.Button(EditorIcons.History, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                    {
                        PromptHistoryWindow.ShowWindow();
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal void DrawDebugToolbar()
        {
            if (_window == null) return;

            var session = _window.CurrentSession?.Value;

            if (session == null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label("No active session.", EditorStyles.toolbarButton);
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            try
            {
                string sessionId = session.Id ?? "<unknown>";
                int messageCount = _window.MessageCount;
                int displayedMessageCount = _window.DisplayedMessageCount;
                int attCount = _window.AttachedFiles?.Count ?? 0;

                GUILayout.Label($"ID: {sessionId}", EditorStyles.toolbarButton);
                GUILayout.Label($"Messages: {messageCount}({displayedMessageCount})", EditorStyles.toolbarButton);
                GUILayout.Label($"Attachments: {attCount}", EditorStyles.toolbarButton);

                if (GUILayout.Button(EditorIcons.Refresh, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                {
                    _window.RebuildUI();
                }

                // dropdown button for more options
                if (GUILayout.Button(EditorIcons.More, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                {
                    GenericMenu menu = new();

                    menu.AddItem(new GUIContent("Re-calculate Total Usage"), false, session.ReCalcTotalUsage);
                    menu.AddItem(new GUIContent("Re-calculate Total Cost"), false, session.ReCalcTotalCost);
                    menu.AddItem(new GUIContent("Save Chat Session"), false, () => session.SaveFile());

                    menu.ShowAsContext();
                }

                // hide/shot progressbar
                if (GUILayout.Button(EditorIcons.Add, EditorStyles.toolbarButton, GUILayout.Width(kBtnWidth)))
                {
                    if (_window.ProgressBar == null)
                    {
                        Debug.LogWarning("ProgressBarController is not initialized.");
                    }
                    else
                    {
                        _window.ProgressBar.ToggleProgressBar();
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }
    }
}