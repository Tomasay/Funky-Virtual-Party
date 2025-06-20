using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// A popup window that allows the user to select a value from a list of values.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window.</typeparam>
    /// <typeparam name="TValue">The type of the value to be selected.</typeparam>
    public abstract class SelectDialog<TWindow, TValue> : EditorWindow
        where TWindow : SelectDialog<TWindow, TValue>
    {
        protected const float WINDOW_MIN_HEIGHT = 80f;
        protected const float WINDOW_MAX_HEIGHT = 980f;
        protected const float WINDOW_WIDTH = 340f;
        private Vector2 _scrollPosition;
        protected Action<TValue> Callback;

        protected string Description = string.Empty;
        protected TValue Value;
        protected List<TValue> ValueList;
        protected bool ShowTitle = true;

        private void OnGUI()
        {
            // if escape key is pressed, close the window
            if (Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.Escape)
                Close();

            EditorGUILayout.BeginVertical(ExStyles.paddedArea);
            try
            {
                EditorGUILayout.LabelField(Description, EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space(2);

                // Draw Content
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                try
                {
                    Value = DrawContent(Value);
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

        public static void Show(string title, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, null, default, true, onComplete, valueList);
        }

        public static void Show(string title, bool showTitle, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, null, default, showTitle, onComplete, valueList);
        }

        public static void Show(string title, TValue defaultValue, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, null, defaultValue, true, onComplete, valueList);
        }

        public static void Show(string title, TValue defaultValue, bool showTitle, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, null, defaultValue, showTitle, onComplete, valueList);
        }

        public static void Show(string title, string description, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, description, default, true, onComplete, valueList);
        }

        public static void Show(string title, string description, bool showTitle, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, description, default, showTitle, onComplete, valueList);
        }

        public static void Show(string title, string description, TValue defaultValue, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            Show(title, description, defaultValue, true, onComplete, valueList);
        }

        public static void Show(string title, string description, TValue defaultValue, bool showTitle, Action<TValue> onComplete, IEnumerable<TValue> valueList = null)
        {
            if (onComplete == null) throw new ArgumentNullException(nameof(onComplete), Messages.Error_NoCallbackArg);

            try
            {
                TWindow window = GetWindow<TWindow>(false, title, true);
                window.Description = description;
                window.Callback = onComplete;
                window.Value = defaultValue;
                window.ValueList = valueList != null ? new List<TValue>(valueList) : null;
                window.ShowTitle = showTitle;
                window.Initialize();
                window.Show();
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format(Messages.Error_FailedToCreateWindow, typeof(TWindow).Name, e.StackTrace));
            }
        }

        protected virtual void Initialize()
        {
            minSize = new Vector2(WINDOW_WIDTH, WINDOW_MIN_HEIGHT);
            maxSize = new Vector2(WINDOW_WIDTH, WINDOW_MAX_HEIGHT);
        }

        /// <summary>
        /// Abstract method to draw the content of the window. Must be implemented by derived classes to render the selection UI.
        /// </summary>
        /// <param name="value">The current selection value.</param>
        /// <returns>The updated value after user interaction.</returns>
        protected abstract TValue DrawContent(TValue value);

        /// <summary>
        /// Draws OK and Cancel buttons at the bottom of the popup.
        /// </summary>
        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(Labels.Cancel, GUILayout.Height(24)))
                {
                    Close();
                }

                if (GUILayout.Button(Labels.Confirm, GUILayout.Height(24)))
                {
                    Callback?.Invoke(Value);
                    Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Sets the default value for the selector, used when the popup is displayed.
        /// </summary>
        /// <param name="value">The value to set as default.</param>
        protected void SetDefaultValue(TValue value)
        {
            Value = value;
        }
    }
}