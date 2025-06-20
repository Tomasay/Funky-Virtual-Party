using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Extends the EditorWindow class with additional functionality
    /// </summary>
    /// <typeparam name="TWindow">The type of the window that is being extended</typeparam>
    [Obsolete("This class is deprecated. Use regular EditorWindow instead.")]
    public abstract class ExtendedEditorWindow<TWindow> : EditorWindow, IExtendedEditorWindow
        where TWindow : EditorWindow
    {
        private const float DEFAULT_IMAGE_HEIGHT = 74f;
        private const float DEFAULT_IMAGE_WIDTH = 74f;
        private const float DEFAULT_ICON_HEIGHT = 32f;
        private const float DEFAULT_ICON_WIDTH = 32f;
        private const float DEFAULT_MIN_WINDOW_HEIGHT = 400f;
        private const float DEFAULT_MIN_WINDOW_WIDTH = 420f;
        private const float DEFAULT_MAX_WINDOW_HEIGHT = 1200f;
        private const float DEFAULT_MAX_WINDOW_WIDTH = 1800f;
        private const float DEFAULT_BUTTON_HEIGHT = 30f;
        private const float DEFAULT_BUTTON_WIDTH = 120f;
        private const float DEFAULT_MINI_BUTTON_HEIGHT = 20f;
        private const float DEFAULT_MINI_BUTTON_WIDTH = 80f;

        public GUILayoutOption[] ButtonOptions;
        public GUILayoutOption[] MiniButtonOptions;

        public EPrefs<string> WindowName { get; set; }
        public EPrefs<Vector2> ImageSize { get; set; }
        public EPrefs<Vector2> IconSize { get; set; }
        public EPrefs<Vector2> MaxWindowSize { get; set; }
        public EPrefs<Vector2> MinWindowSize { get; set; }
        public EPrefs<Vector2> ButtonSize { get; set; }
        public EPrefs<Vector2> MiniButtonSize { get; set; }

        private ExtendedEditorSettingsWindow _settingsWindow;


        protected virtual void OnEnable()
        {
            WindowName = new EPrefs<string>(typeof(TWindow).Name + ".WindowName", GetType().Name);
            ImageSize = new EPrefs<Vector2>(typeof(TWindow).Name + ".ImageSize", new Vector2(DEFAULT_IMAGE_WIDTH, DEFAULT_IMAGE_HEIGHT));
            IconSize = new EPrefs<Vector2>(typeof(TWindow).Name + ".IconSize", new Vector2(DEFAULT_ICON_WIDTH, DEFAULT_ICON_HEIGHT));
            MaxWindowSize = new EPrefs<Vector2>(typeof(TWindow).Name + ".MaxWindowSize", new Vector2(DEFAULT_MAX_WINDOW_WIDTH, DEFAULT_MAX_WINDOW_HEIGHT));
            MinWindowSize = new EPrefs<Vector2>(typeof(TWindow).Name + ".MinWindowSize", new Vector2(DEFAULT_MIN_WINDOW_WIDTH, DEFAULT_MIN_WINDOW_HEIGHT));
            ButtonSize = new EPrefs<Vector2>(typeof(TWindow).Name + ".ButtonSize", new Vector2(DEFAULT_BUTTON_WIDTH, DEFAULT_BUTTON_HEIGHT));
            MiniButtonSize = new EPrefs<Vector2>(typeof(TWindow).Name + ".MiniButtonSize", new Vector2(DEFAULT_MINI_BUTTON_WIDTH, DEFAULT_MINI_BUTTON_HEIGHT));

            // check if _minWindowSize and _maxWindowSize aren't too small
            if (MinWindowSize.Value.x < 100 || MinWindowSize.Value.y < 100)
            {
                MinWindowSize.Value = new Vector2(DEFAULT_MIN_WINDOW_WIDTH, DEFAULT_MIN_WINDOW_HEIGHT);
            }

            if (MaxWindowSize.Value.x < 100 || MaxWindowSize.Value.y < 100)
            {
                MaxWindowSize.Value = new Vector2(DEFAULT_MAX_WINDOW_WIDTH, DEFAULT_MAX_WINDOW_HEIGHT);
            }

            minSize = MinWindowSize.Value;
            maxSize = MaxWindowSize.Value;

            ButtonOptions = new GUILayoutOption[]
            {
                GUILayout.Width(ButtonSize.Value.x), GUILayout.Height(ButtonSize.Value.y), GUILayout.ExpandWidth(true)
            };
            MiniButtonOptions = new GUILayoutOption[]
            {
                GUILayout.Width(MiniButtonSize.Value.x), GUILayout.Height(MiniButtonSize.Value.y), GUILayout.ExpandWidth(true)
            };
        }

        protected static TWindow Initialize(string name = null)
        {
            name ??= typeof(TWindow).Name;
            TWindow window = (TWindow)GetWindow(typeof(TWindow), false, name);
            window.Show();
            window.autoRepaintOnSceneChange = true;
            return window;
        }

        protected void InternalMenuHeader(string windowTitle, ref bool boolValue)
        {
            /* centered button with width 100 */
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(windowTitle, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close"))
                {
                    boolValue = false;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        protected void OpenSettingsWindow()
        {
            try
            {
                if (_settingsWindow == null)
                {
                    _settingsWindow = ExtendedEditorSettingsWindow.Initialize(this); ;
                }
                else
                {
                    _settingsWindow.Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to open settings window: " + ex.ToString());
            }
        }
    }
}