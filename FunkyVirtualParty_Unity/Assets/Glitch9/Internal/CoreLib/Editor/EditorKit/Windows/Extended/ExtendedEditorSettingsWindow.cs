using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class ExtendedEditorSettingsWindow : PaddedEditorWindow
    {
        private IExtendedEditorWindow _window;

        internal static ExtendedEditorSettingsWindow Initialize(IExtendedEditorWindow window)
        {
            ExtendedEditorSettingsWindow settingsWindow = (ExtendedEditorSettingsWindow)GetWindow(typeof(ExtendedEditorSettingsWindow), true, "Extended Editor Window Settings");

            settingsWindow.Show();
            settingsWindow._window = window;
            settingsWindow.minSize = new Vector2(456, 250);
            settingsWindow.maxSize = new Vector2(456, 250);

            return settingsWindow;
        }

        protected override void DrawGUI()
        {
            if (_window == null)
            {
                EditorGUILayout.HelpBox("Base window is null", MessageType.Error);
                return;
            }

            GUILayout.BeginVertical(ExGUI.Box(10, 10));
            {
                EditorGUILayout.LabelField("Extended Editor Window Settings", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                {
                    _window.WindowName.Value = EditorGUILayout.TextField("Window Name", _window.WindowName.Value);
                    if (GUILayout.Button("Update", GUILayout.Width(100)))
                    {
                        _window.WindowName.Value = GetType().Name;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    _window.MinWindowSize.Value = EditorGUILayout.Vector2Field("Minimum Window Size", _window.MinWindowSize.Value);
                    _window.MaxWindowSize.Value = EditorGUILayout.Vector2Field("Maximum Window Size", _window.MaxWindowSize.Value);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.LabelField("UI Buttons", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                {
                    _window.ButtonSize.Value = EditorGUILayout.Vector2Field("Button Size", _window.ButtonSize.Value);
                    _window.MiniButtonSize.Value = EditorGUILayout.Vector2Field("Inner Button Size", _window.MiniButtonSize.Value);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.LabelField("UI Images (Textures/Icons)", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                {
                    _window.ImageSize.Value = EditorGUILayout.Vector2Field("Image Size", _window.ImageSize.Value);
                    _window.IconSize.Value = EditorGUILayout.Vector2Field("Icon Size", _window.IconSize.Value);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
