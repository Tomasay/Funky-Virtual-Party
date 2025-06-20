using System.Collections.Generic;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Chatbots
{
    public class ChatbotMessagesWindow : PaddedEditorWindow
    {
        private static readonly Color SeparatorColor = new(0.8f, 0.8f, 0.8f, 0.25f);
        private List<ChatMessage> _messages;
        private GUIStyle _messageStyle;
        private GUIStyle MessageStyle
        {
            get
            {
                _messageStyle ??= new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    richText = true,
                    fontSize = 12,
                    margin = new RectOffset(10, 10, 5, 5)
                };
                return _messageStyle;
            }
        }
        private GUIStyle _labelStyle;
        private GUIStyle LabelStyle
        {
            get
            {
                _labelStyle ??= new GUIStyle(GUI.skin.label)
                {
                    wordWrap = true,
                    richText = true,
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    margin = new RectOffset(10, 10, 5, 10)
                };
                return _messageStyle;
            }
        }
        private Vector2 _scrollPosition;

        public static void ShowWindow(List<ChatMessage> messages)
        {
            ChatbotMessagesWindow window = GetWindow<ChatbotMessagesWindow>("Chatbot Messages");
            window._messages = messages;
            window.Show();
            ChatSessionManager.LoadSessions();
        }

        protected override void DrawGUI()
        {
            if (_messages == null || _messages.Count == 0)
            {
                EditorGUILayout.LabelField("No messages to display.");
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (ChatMessage message in _messages)
            {
                if (message == null)
                {
                    EditorGUILayout.LabelField("Null message encountered.", MessageStyle);
                    continue;
                }

                try
                {
                    EditorGUILayout.LabelField(FormatLabel(message), LabelStyle);
                    EditorGUILayout.LabelField(message.Content, MessageStyle);
                }
                catch (System.Exception ex)
                {
                    EditorGUILayout.LabelField($"Exception: {ex.Message}", MessageStyle);
                }

                GUILayout.Space(5);
                DrawSeparator();
                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();
        }

        private string FormatLabel(ChatMessage message)
        {
            return $"<b>{FormatRole(message.Role)}</b> <size=14>({FormatTimestamp(message.Timestamp)})</size>";
        }

        private string FormatTimestamp(UnixTime timestamp)
        {
            return timestamp.ToDateTime().ToString("yyyy-MM-dd HH:mm tt");
        }

        private string FormatRole(ChatRole role)
        {
            return role switch
            {
                ChatRole.User => "User",
                ChatRole.Assistant => "Chatbot",
                ChatRole.System => "System",
                ChatRole.Tool => "Tool",
                _ => "Unknown"
            };
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, SeparatorColor);
        }
    }
}