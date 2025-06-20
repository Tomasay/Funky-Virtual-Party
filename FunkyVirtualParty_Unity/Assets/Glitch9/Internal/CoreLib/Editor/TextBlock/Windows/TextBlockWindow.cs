using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class TextBlockWindow : PaddedEditorWindow
    {
        internal static void ShowWindow(string windowName, string text)
        {
            TextBlockWindow window = GetWindow<TextBlockWindow>(true, windowName);
            window._text = text;
        }

        private string _text;
        private Vector2 _scrollPosition;

        protected override void DrawGUI()
        {
            GUILayout.BeginVertical(ExStyles.helpBox);
            {
                if (string.IsNullOrEmpty(_text))
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("No text to display.", ExStyles.centeredGreyMiniLabel);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                    {
                        TextBlock.DrawIMGUI(_text, position.width);
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            GUILayout.EndVertical();
        }
    }
}