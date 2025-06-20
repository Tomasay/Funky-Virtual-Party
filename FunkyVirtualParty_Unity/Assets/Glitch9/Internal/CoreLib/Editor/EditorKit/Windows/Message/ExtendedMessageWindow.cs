
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public class ExtendedMessageWindow : EditorWindow
    {
        public static void Show(string title, string message, string okLabel, Vector2 size)
        {
            ExtendedMessageWindow popup = GetWindow<ExtendedMessageWindow>(true, title, true);
            popup._message = message;
            popup._ok = okLabel;
            popup.minSize = size;
            popup.maxSize = size;
        }

        private string _message;
        private string _ok;
        private Vector2 _scrollPosition;


        private void OnGUI()
        {

            GUILayout.BeginVertical(ExStyles.popupBody);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField(_message, EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_ok))
            {
                Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}