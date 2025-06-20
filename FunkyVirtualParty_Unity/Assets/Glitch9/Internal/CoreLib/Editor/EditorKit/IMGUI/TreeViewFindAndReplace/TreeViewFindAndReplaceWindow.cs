using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public class TreeViewFindAndReplaceWindow : EditorWindow
    {
        private enum FNRMode
        {
            Find,
            Replace,
        }

        private const string DEFAULT_WINDOW_NAME = "Find and Replace";
        private const float WINDOW_WIDTH = 400;
        private const float WINDOW_HEIGHT_MIN = 400;
        private const float WINDOW_HEIGHT_MAX = 800;
        private const float TALK_BUBBLE_WIDTH = 320;
        private const float BTN_HEIGHT = 24;

        public static TreeViewFindAndReplaceWindow Instance { get; private set; }

        public static void Find(Action<string> onFindNext, Action<string, string> onReplaceNext, Action<string, string> onReplaceAll)
        {
            Find(DEFAULT_WINDOW_NAME, onFindNext, onReplaceNext, onReplaceAll);
        }

        public static void Replace(Action<string> onFindNext, Action<string, string> onReplaceNext, Action<string, string> onReplaceAll)
        {
            Replace(DEFAULT_WINDOW_NAME, onFindNext, onReplaceNext, onReplaceAll);
        }

        public static void Find(string windowName, Action<string> onFindNext, Action<string, string> onReplaceNext, Action<string, string> onReplaceAll)
        {
            if (Instance != null)
            {
                Instance._mode = FNRMode.Find;
                Instance.Focus();
                return;
            }
            Instance = GetWindow<TreeViewFindAndReplaceWindow>(windowName);
            Instance.Initialize(FNRMode.Find, onFindNext, onReplaceNext, onReplaceAll);
        }

        public static void Replace(string windowName, Action<string> onFindNext, Action<string, string> onReplaceNext, Action<string, string> onReplaceAll)
        {
            if (Instance != null)
            {
                Instance._mode = FNRMode.Replace;
                Instance.Focus();
                return;
            }
            Instance = GetWindow<TreeViewFindAndReplaceWindow>(windowName);
            Instance.Initialize(FNRMode.Replace, onFindNext, onReplaceNext, onReplaceAll);
        }

        private void Initialize(FNRMode mode, Action<string> onFindNext, Action<string, string> onReplaceNext, Action<string, string> onReplaceAll)
        {
            minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT_MIN);
            maxSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT_MAX);
            _mode = mode;
            _onFindNext = onFindNext;
            _onReplace = onReplaceNext;
            _onReplaceAll = onReplaceAll;
        }

        private static class Strings
        {
            internal const string WINDOW_TITLE_FIND = "Find Text";
            internal const string WINDOW_TITLE_REPLACE = "Replace Text";
            internal const string INSTRUCTION_FIND = "Which text would you like to find?";
            internal const string INSTRUCTION_REPLACE = "Enter the text you want to replace the found text with.";
            internal const string INSTRUCTION_PRESS_ENTER_FIND = "Press Enter to find the next occurrence.";
            internal const string INSTRUCTION_PRESS_ENTER_REPLACE = "Press Enter to replace the found text with the entered text.";

            internal const string LABEL_FIND_NEXT = "  Find Next";
            internal const string LABEL_REPLACE_NEXT = "  Replace Next";
            internal const string LABEL_REPLACE_ALL = "  Replace All";
            internal const string LABEL_COMPARISON_TYPE = "Comparison Type";

            internal const string CONFIRMATION_REPLACE_TEXT = "Are you sure you want to replace all instances of the text?";
        }

        private static class GUIContents
        {
            internal static readonly GUIContent FindNextLabel = new(Strings.LABEL_FIND_NEXT, EditorIcons.Search);
            internal static readonly GUIContent ReplaceNextLabel = new(Strings.LABEL_REPLACE_NEXT, EditorIcons.Replace);
            internal static readonly GUIContent ReplaceAllLabel = new(Strings.LABEL_REPLACE_ALL, EditorIcons.Replace);
        }

        private FNRMode _mode;
        private string _textInputFind;
        private string _textInputReplace;

        private string _windowTitle;
        private string _instructionPressEnter;

        private Action<string> _onFindNext;
        private Action<string, string> _onReplace;
        private Action<string, string> _onReplaceAll;

        public StringComparison Comparison
        {
            get
            {
                _comparison ??= new EPrefs<StringComparison>("TreeViewFindAndReplace.Comparison", StringComparison.OrdinalIgnoreCase);
                return _comparison.Value;
            }

            set
            {
                _comparison ??= new EPrefs<StringComparison>("TreeViewFindAndReplace.Comparison", StringComparison.OrdinalIgnoreCase);
                _comparison.Value = value;
            }
        }

        private EPrefs<StringComparison> _comparison;

        private void OnEnable()
        {
            UpdateModeUI();
        }

        private void UpdateModeUI()
        {
            if (_mode == FNRMode.Find)
            {
                _windowTitle = Strings.WINDOW_TITLE_FIND;
                _instructionPressEnter = Strings.INSTRUCTION_PRESS_ENTER_FIND;
            }
            else
            {
                _windowTitle = Strings.WINDOW_TITLE_REPLACE;
                _instructionPressEnter = Strings.INSTRUCTION_PRESS_ENTER_REPLACE;
            }
        }

        public void OnGUI()
        {
            CaptureKeyboardInput(); // Expensive operation

            if (ExGUILayout.EnumToolbar(_mode, out FNRMode newMode))
            {
                _mode = newMode;
                UpdateModeUI();
            }
            ExGUIUtility.DrawHorizontalBorder(20);

            float textFieldHeight = position.height - 214;
            if (_mode == FNRMode.Replace)
            {
                textFieldHeight *= 0.5f; // half
                textFieldHeight -= BTN_HEIGHT * 0.5f; // button / 2
                textFieldHeight -= 5; // offset
            }

            GUILayout.BeginVertical(TreeViewStyles.FindAndReplaceLayout);
            {
                ExGUIPreset.TitleField(_windowTitle);

                GUILayout.Space(5);

                GUILayout.Label(Strings.INSTRUCTION_FIND, EditorStyles.wordWrappedLabel);
                _textInputFind = GUILayout.TextArea(_textInputFind, GUI.skin.textArea, GUILayout.ExpandWidth(true), GUILayout.Height(textFieldHeight));

                if (_mode == FNRMode.Replace)
                {
                    GUILayout.Space(5);

                    GUILayout.Label(Strings.INSTRUCTION_REPLACE, EditorStyles.wordWrappedLabel);
                    _textInputReplace = GUILayout.TextArea(_textInputReplace, GUI.skin.textArea, GUILayout.ExpandWidth(true), GUILayout.Height(textFieldHeight));
                }

                GUILayout.Space(5);

                Comparison = (StringComparison)EditorGUILayout.EnumPopup(Strings.LABEL_COMPARISON_TYPE, Comparison);

                GUILayout.Space(10);

                GUILayout.BeginHorizontal(GUILayout.Height(36));
                {
                    ExGUILayout.TextureField(EditorIcons.Routina.Aimi, new Vector2(36, 36));
                    GUILayout.Label(_instructionPressEnter, TreeViewStyles.TalkBubbleStyle, GUILayout.MaxWidth(TALK_BUBBLE_WIDTH));
                    GUILayout.Space(10);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            if (_mode == FNRMode.Find)
            {
                if (GUILayout.Button(GUIContents.FindNextLabel, GUILayout.Height(BTN_HEIGHT * 2)))
                {
                    if (string.IsNullOrEmpty(_textInputFind)) return;
                    _onFindNext?.Invoke(_textInputFind);
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(GUIContents.FindNextLabel, GUILayout.Height(BTN_HEIGHT)))
                    {
                        if (string.IsNullOrEmpty(_textInputFind)) return;
                        _onFindNext?.Invoke(_textInputFind);
                    }

                    if (GUILayout.Button(GUIContents.ReplaceNextLabel, GUILayout.Height(BTN_HEIGHT)))
                    {
                        if (string.IsNullOrEmpty(_textInputFind) || string.IsNullOrEmpty(_textInputReplace)) return;
                        _onReplace?.Invoke(_textInputFind, _textInputReplace);
                    }
                }
                GUILayout.EndHorizontal();

                if (_mode == FNRMode.Replace)
                {
                    if (GUILayout.Button(GUIContents.ReplaceAllLabel, GUILayout.Height(BTN_HEIGHT)))
                    {
                        if (string.IsNullOrEmpty(_textInputFind) || string.IsNullOrEmpty(_textInputReplace)) return;
                        if (ShowDialog.Confirm(Strings.CONFIRMATION_REPLACE_TEXT))
                        {
                            _onReplaceAll?.Invoke(_textInputFind, _textInputReplace);
                        }
                    }
                }
            }
        }

        private void CaptureKeyboardInput()
        {
            Event e = Event.current;
            if (Event.current.type != EventType.KeyDown) return;

            if (e.keyCode == KeyCode.Return)
            {
                //Debug.Log("Enter key pressed");
                e.Use();
            }
        }
    }
}
