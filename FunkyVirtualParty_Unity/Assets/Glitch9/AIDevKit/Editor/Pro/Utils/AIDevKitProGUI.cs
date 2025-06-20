using System.Collections.Generic;
using System.Linq;
using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.AIDevKit.Editor.Chatbots;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class AIDevKitProGUI
    {
        internal static ChatSession ChatSessionPopup(string selectedId, List<ChatSession> displayedOptions, PopupGUIStyle style = PopupGUIStyle.Default)//GUIStyle dropdownStyle = null)
        {
            GUIStyle popupStyle = AIDevKitStyles.GetPopupDropdownStyle(style); //? EditorStyles.toolbarDropDown : EditorStyles.popup;
            GUIStyle btnStyle = AIDevKitStyles.GetPopupButtonStyle(style); //style == PopupGUIStyle.Toolbar ? EditorStyles.toolbarButton : EditorStyles.miniButtonRight; 
            float btnWidth = style == PopupGUIStyle.Toolbar ? 32f : 20f;

            displayedOptions ??= ChatSessionManager.Sessions;

            if (displayedOptions.Count == 0)
            {
                DrawNoChatbots(btnStyle, btnWidth);
                return null;
            }

            ChatSession selected = displayedOptions.FirstOrDefault(a => a.Id == selectedId);

            // Fallback to first valid model if selected is null
            if (selected == null || string.IsNullOrEmpty(selected.Id))
            {
                selected = displayedOptions.FirstOrDefault();
            }

            // If still null after all fallback attempts
            if (selected == null)
            {
                DrawNoChatbots(btnStyle, btnWidth);
                return null;
            }

            List<string> displayedTexts = displayedOptions.Select(FormatChatSessionListName).ToList();

            int selectedAssetIndex = displayedOptions.IndexOf(selected);
            if (selectedAssetIndex < 0) selectedAssetIndex = 0;
            int newAssetIndex = EditorGUILayout.Popup(selectedAssetIndex, displayedTexts.ToArray(), popupStyle, GUILayout.ExpandWidth(true));

            if (newAssetIndex != selectedAssetIndex) selected = displayedOptions[newAssetIndex];

            if (GUILayout.Button(AIDevKitIcons.Assistant, btnStyle, GUILayout.Width(btnWidth)))
            {
                ChatbotManagerWindow.ShowWindow();
            }

            return selected;
        }

        private static string FormatChatSessionListName(ChatSession session)
        {
            if (session == null) return "Null Session";

            bool defaultTitle = ChatSessionUtil.IsDefaultSessionTitle(session.Name);
            if (defaultTitle) return $"{session.CreatedAt.GetInspectorName()} ({session.Count})";
            return $"{session.Name} ({session.Count})";
        }

        private static void DrawNoChatbots(GUIStyle btnStyle, float btnWidth)
        {
            ExGUILayout.ErrorLabel("No Chatbots found. Please create a new Chatbot using the Chatbot Manager.");
            if (GUILayout.Button(AIDevKitIcons.Assistant, btnStyle, GUILayout.Width(btnWidth)))
            {
                ChatbotManagerWindow.ShowWindow();
            }
        }
    }
}