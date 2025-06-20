using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public abstract class EditorEditWindow<TSelf, TValue> : EditorWindow
        where TSelf : EditorEditWindow<TSelf, TValue>
    {
        protected const float kMinHeight = 80f;
        protected const float kMaxHeight = 980f;
        protected const float kWidth = 340f;

        public static void Show(string title, TValue value, Action<TValue> onEdited, Vector2? maxSize = null)
        => Show(title, string.Empty, value, onEdited, maxSize);
        public static void Show(string title, string description, TValue value, Action<TValue> onEdited, Vector2? maxSize = null)
        {
            TSelf window = GetWindow<TSelf>(false, title, true);
            window.Initialize(description, value, onEdited, maxSize);
            window.position = new Rect(100, 100, 800, 600); // 강제 위치 설정
            window.Show();
        }

        private Vector2 _scrollPosition;
        private Action<TValue> _onEdited;
        private string _description = string.Empty;
        private TValue _initialValue;
        private TValue _currentValue;

        protected virtual void Initialize(string description, TValue value, Action<TValue> onEdited, Vector2? maxSize)
        {
            _onEdited = onEdited ?? throw new ArgumentNullException(nameof(onEdited), Messages.Error_NoCallbackArg);

            _description = description ?? string.Empty;
            _initialValue = value;
            _currentValue = value;

            minSize = new Vector2(kWidth, kMinHeight);
            this.maxSize = maxSize ?? new Vector2(kWidth, kMaxHeight);
        }

        private void OnGUI()
        {
            //if escape key is pressed, close the window
            if (Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.Escape)
                CloseINTERNAL();

            EditorGUILayout.BeginVertical();
            try
            {
                if (!string.IsNullOrEmpty(_description))
                {
                    EditorGUILayout.LabelField(_description, EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space(2);
                }

                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                try
                {
                    _currentValue = DrawGUI(_currentValue);
                }
                finally
                {
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }

                // Draw Buttons
                DrawButtons();
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        protected abstract TValue DrawGUI(TValue value);

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(Labels.Cancel, GUILayout.Height(24)))
                {
                    CloseINTERNAL();
                }

                if (GUILayout.Button(Labels.Save, GUILayout.Height(24)))
                {
                    _onEdited?.Invoke(_currentValue);
                    CloseINTERNAL();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CloseINTERNAL()
        {
            Close();
        }

        protected void RevertChanges() => _currentValue = _initialValue;
    }
}