using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal static class ExGUIRenderer
    {
        private const float kSwitchOffsetX = 0.5f;
        private const float kPrefixToggleSize = 14f;
        private static Dictionary<string, AnimatedToggle> _animatedToggles;
        internal static AnimatedToggle GetAnimatedToggle(string key, bool defaultValue = false)
        {
            _animatedToggles ??= new Dictionary<string, AnimatedToggle>();

            if (!_animatedToggles.TryGetValue(key, out var toggle))
            {
                toggle = new AnimatedToggle(defaultValue);
                _animatedToggles[key] = toggle;
            }
            return toggle;
        }

        internal static T DrawEnumPopup<T>(GUIContent label, T value, params GUILayoutOption[] options) where T : Enum
        {
            Type enumType = typeof(T);

            string[] displayedNames = EnumUtil.GetInspectorNames(enumType);
            Array enumValues = Enum.GetValues(enumType);

            // 값이 Enum 정의에 없는 경우 처리
            int enumIndex = Array.IndexOf(enumValues, value);
            if (enumIndex < 0) enumIndex = 0;

            // Index 유효성 체크
            enumIndex = Mathf.Clamp(enumIndex, 0, displayedNames.Length - 1);

            // Popup 표시
            int newIndex = EditorGUILayout.Popup(label, enumIndex, displayedNames, options);
            newIndex = Mathf.Clamp(newIndex, 0, enumValues.Length - 1);

            if (newIndex != enumIndex) value = (T)enumValues.GetValue(newIndex);

            return value;
        }

        internal static T DrawResettableEnumPopup<T>(GUIContent label, T selected, T defaultValue) where T : Enum
        {
            GUILayout.BeginHorizontal();
            try
            {
                selected = DrawEnumPopup(label, selected);
                if (ExGUI.ResetButton()) selected = defaultValue;
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
            return selected;
        }

        internal static T DrawExtendedEnumPopup<T>(string label, T selected, IEnumerable<T> displayedOptions, string[] displayedNames, GUIStyle style = null, params GUILayoutOption[] options) where T : Enum
        {
            T[] enumValues = displayedOptions.ToArray();

            // Display only a subset of names if ignoring default 
            string[] displayedOptionsAsString = displayedOptions.Select(v => v.ToString()).ToArray();
            displayedNames ??= displayedOptions.Select(v => v.GetInspectorName()).ToArray();

            int selectedIndex = Array.IndexOf(enumValues, selected);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, enumValues.Length - 1);

            style ??= EditorStyles.popup;
            int newIndex;

            if (label == null)
            {
                newIndex = EditorGUILayout.Popup(selectedIndex: selectedIndex, displayedOptions: displayedNames, style: style, options);
            }
            else
            {
                newIndex = EditorGUILayout.Popup(label, selectedIndex: selectedIndex, displayedOptions: displayedNames, style: style, options);
            }

            // Convert back to original enum index
            return (T)Enum.Parse(typeof(T), displayedOptionsAsString[newIndex]);
        }

        internal static void DrawPathField(GUIContent label, SerializedProperty property, string rootPath, string defaultPath)
        {
            GUILayout.BeginHorizontal();

            string value = property.stringValue;

            if (string.IsNullOrEmpty(value))
            {
                value = defaultPath.FixSlashes();
            }

            string displayValue = value.Replace(rootPath, "");
            EditorGUILayout.LabelField(label, new GUIContent(displayValue), EditorStyles.textField, GUILayout.MinWidth(20));

            if (ExGUI.ResetButton(EditorStyles.miniButtonMid))
            {
                value = defaultPath.FixSlashes();
            }

            if (ExGUI.FolderPanelButton(EditorStyles.miniButtonMid))
            {
                value = ExGUIUtility.OpenFolderPanel(value, rootPath);
            }

            if (ExGUI.OpenFolderButton(EditorStyles.miniButtonRight))
            {
                ExGUIUtility.OpenFolder(value, defaultPath);
            }

            GUILayout.EndHorizontal();

            if (value != property.stringValue)
            {
                property.stringValue = value.FixSlashes();
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        internal static T DrawPopupT<T>(GUIContent label, T value, IList<T> list, params GUILayoutOption[] options)
        {
            if (list.IsNullOrEmpty())
            {
                EditorGUILayout.HelpBox("No list found.", MessageType.None);
                return default;
            }

            value ??= list.First();

            try
            {
                int index = list.IndexOf(value);
                List<string> stringArray = list.Select(enumValue => enumValue.ToString()).ToList();
                index = EditorGUILayout.Popup(label, index, stringArray.ToArray(), options);
                if (index < 0) index = 0;
                return list[index];
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox(e.Message, MessageType.Error);
                return default;
            }
        }

        internal static T DrawEnumSwitch<T>(GUIContent label, T enumValue, int startIndex = 0, params GUILayoutOption[] options) where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            List<string> names = new();
            foreach (var value in values)
            {
                T e = (T)value;
                string name = e.GetInspectorName();
                names.Add(name);
            }

            int currentIndex = Array.IndexOf(values, enumValue);
            int selectedIndex = currentIndex;

            EditorGUILayout.BeginHorizontal(options);
            try
            {
                //float viewWidth = EditorGUIUtility.currentViewWidth - 21f;

                if (label != null)
                {
                    EditorGUILayout.PrefixLabel(label);
                    //viewWidth -= EditorGUIUtility.labelWidth;
                }

                //float btnWidth = viewWidth / (values.Length - startIndex);

                for (int i = startIndex; i < values.Length; i++)
                {
                    bool isSelected = i == selectedIndex;

                    bool clicked;
                    if (i == startIndex)
                    {
                        clicked = ExGUILayout.ToggleLeft(names[i], isSelected, options: GUILayout.ExpandWidth(true));
                    }
                    else if (i == values.Length - 1)
                    {
                        clicked = ExGUILayout.ToggleRight(names[i], isSelected, options: GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        clicked = ExGUILayout.ToggleMid(names[i], isSelected, options: GUILayout.ExpandWidth(true));
                    }

                    if (clicked)
                    {
                        selectedIndex = i;
                    }
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }

            return (T)values.GetValue(selectedIndex);
        }

        internal static T DrawEnumFlagSwitch<T>(T enumValue, int startIndex = 0, params GUILayoutOption[] options) where T : Enum
        {
            // Get all values and names of the ApiEnumDE
            Array values = Enum.GetValues(typeof(T));
            string[] names = Enum.GetNames(typeof(T));

            // Convert the current selected value to int
            int currentIntValue = Convert.ToInt32(enumValue);
            int newIntValue = currentIntValue;

            EditorGUILayout.BeginHorizontal();

            if (values.Length > startIndex)
            {
                for (int i = startIndex; i < values.Length; i++)
                {
                    // Determine if the current flag is set
                    bool flagSet = (currentIntValue & (int)values.GetValue(i)) != 0;

                    if (i == startIndex)
                    {
                        flagSet = ExGUILayout.ToggleLeft(names[i], flagSet, options: options);
                    }
                    else if (i == values.Length - 1)
                    {
                        flagSet = ExGUILayout.ToggleRight(names[i], flagSet, options: options);
                    }
                    else
                    {
                        flagSet = ExGUILayout.ToggleMid(names[i], flagSet, options: options);
                    }

                    // Update the int value based on toggle button state
                    if (flagSet)
                    {
                        newIntValue |= (int)values.GetValue(i);
                    }
                    else
                    {
                        newIntValue &= ~(int)values.GetValue(i);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            // Convert the int value back to the enum type and return
            return (T)Enum.ToObject(typeof(T), newIntValue);
        }

        internal static string DrawPathField(GUIContent label, string value, string rootPath, string defaultPath)
        {
            GUILayout.BeginHorizontal();
            try
            {
                string displayValue = value.Replace(rootPath, "");
                EditorGUILayout.LabelField(label, new GUIContent(displayValue), EditorStyles.textField, GUILayout.MinWidth(20));

                if (ExGUI.ResetButton(EditorStyles.miniButtonMid))
                {
                    value = defaultPath.FixSlashes();
                }

                if (ExGUI.FolderPanelButton(EditorStyles.miniButtonRight))
                {
                    value = ExGUIUtility.OpenFolderPanel(value, rootPath);
                }

                // if (ExGUI.OpenFolderButton(EditorStyles.miniButtonRight))
                // {
                //     ExGUIUtility.OpenFolder(value, defaultPath);
                // }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            if (string.IsNullOrEmpty(value))
            {
                value = defaultPath.FixSlashes();
            }

            return value;
        }

        internal static void DrawIconLabelWithSpace(GUIContent label, Texture icon, int space, GUIStyle style = null)
        {
            style ??= ExStyles.label;

            if (icon != null)
            {
                GUILayout.BeginHorizontal();
                try
                {
                    GUILayout.Label(icon, style, GUILayout.Width(16f), GUILayout.Height(16f));
                    if (space > 0) GUILayout.Space(space);
                    GUILayout.Label(label, style, GUILayout.Height(18f));
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField(label, style, GUILayout.Height(18f));
            }
        }

        internal static Color DrawColorPicker(GUIContent label, Color selected, IList<Color> displayedOptions)
        {
            Color defaultColor = GUI.backgroundColor;
            GUIStyle style = ExStyles.colorPickerButton;

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));

                foreach (Color color in displayedOptions)
                {
                    // Change the background texture for the selected color entry
                    style.normal.background = color == selected ? EditorTextures.ToolBarButtonOn : EditorTextures.ToolBarButtonOff;
                    GUI.backgroundColor = color;

                    if (GUILayout.Button("", style))
                    {
                        selected = color;
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            GUI.backgroundColor = defaultColor;
            return selected;
        }

        internal static void DrawSwitch(string label, Action<bool> action, string yes, string no)
        {
            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.PrefixLabel(label);

                float buttonWidth = (EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth) / 2;
                buttonWidth -= kSwitchOffsetX * 2;

                if (GUILayout.Button(yes, GUILayout.Width(buttonWidth)))
                {
                    action(true);
                }
                if (GUILayout.Button(no, GUILayout.Width(buttonWidth)))
                {
                    action(false);
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal static void DrawSwitchFromButtonEntries(string label, params ButtonEntry[] buttonEntries)
        {
            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.PrefixLabel(label);

                float buttonWidth = (EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth) / buttonEntries.Length;
                buttonWidth -= kSwitchOffsetX * buttonEntries.Length;

                foreach (ButtonEntry entry in buttonEntries)
                {
                    if (GUILayout.Button(entry.Label, GUILayout.Width(buttonWidth)))
                    {
                        entry.Callback?.Invoke();
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal static void DrawHelpBoxEx(string message, MessageTypeEx status, GUIStyle textStyle, params GUILayoutOption[] options)
        {
            Texture stateIcon = status switch
            {
                MessageTypeEx.Success => EditorIcons.StatusCheck,
                MessageTypeEx.Error => EditorIcons.StatusError,
                MessageTypeEx.Warning => EditorIcons.StatusWarning,
                _ => null,
            };

            GUILayout.BeginHorizontal(ExStyles.IndentedHelpBox(EditorGUI.indentLevel), options);
            {
                if (stateIcon != null)
                {
                    GUILayout.Label(stateIcon, GUILayout.Height(30), GUILayout.Width(30), GUILayout.ExpandHeight(true));
                }

                GUILayout.Label(message, textStyle, GUILayout.ExpandHeight(true));
            }
            GUILayout.EndHorizontal();
        }

        internal static T? DrawNullableField<T>(GUIContent label, T? value, T defaultValue, Func<T, T> drawer) where T : struct
        {
            float padding = 4f;
            float prevLabelWidth = EditorGUIUtility.labelWidth;
            T newValue;

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUIUtility.labelWidth = Mathf.Max(0, prevLabelWidth - kPrefixToggleSize - padding - (EditorGUI.indentLevel * 13f));

                bool enabled = value != null;
                bool newEnabled = EditorGUILayout.Toggle(GUIContent.none, enabled, GUILayout.Width(kPrefixToggleSize));
                if (newEnabled != enabled) value = newEnabled ? defaultValue : null;

                EditorGUILayout.PrefixLabel(label);

                EditorGUI.BeginDisabledGroup(!enabled);
                newValue = drawer.Invoke(value ?? defaultValue);
                EditorGUI.EndDisabledGroup();
            }
            finally
            {
                EditorGUIUtility.labelWidth = prevLabelWidth;
                GUILayout.EndHorizontal();
            }

            if (value == null) return null;
            value = newValue;
            return value;
        }

        internal static T? DrawNullableFieldWithoutLabel<T>(T? value, T defaultValue, Func<T, T> drawer) where T : struct
        {
            float padding = 4f;
            float prevLabelWidth = EditorGUIUtility.labelWidth;
            T newValue;

            GUILayout.BeginHorizontal();
            try
            {
                bool enabled = value != null;
                bool newEnabled = EditorGUILayout.Toggle(GUIContent.none, enabled, GUILayout.Width(kPrefixToggleSize));
                if (newEnabled != enabled) value = newEnabled ? defaultValue : null;
                EditorGUIUtility.labelWidth = Mathf.Max(0, prevLabelWidth - kPrefixToggleSize - padding);

                EditorGUI.BeginDisabledGroup(!enabled);
                newValue = drawer.Invoke(value ?? defaultValue);
                EditorGUI.EndDisabledGroup();

            }
            finally
            {
                EditorGUIUtility.labelWidth = prevLabelWidth;
                GUILayout.EndHorizontal();
            }

            if (value == null) return null;
            value = newValue;
            return value;
        }

        internal static T DrawResettableField<T>(GUIContent label, T value, T defaultValue, Func<T, T> drawer) where T : struct
        {
            int savedIndentLevel = EditorGUI.indentLevel;
            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.PrefixLabel(label);
                EditorGUI.indentLevel = 0;
                value = drawer.Invoke(value);
                if (ExGUI.ResetButton()) value = defaultValue;
            }
            finally
            {
                EditorGUI.indentLevel = savedIndentLevel;
                GUILayout.EndHorizontal();
            }
            return value;
        }

        internal static void DrawTextureField(Texture texture, Vector2? size = null, float offsetY = 0)
        {
            try
            {
                size ??= new Vector2(texture.width, texture.height);
                Rect rect = GUILayoutUtility.GetRect(size.Value.x, size.Value.y + offsetY);
                GUI.DrawTexture(rect, texture != null ? texture : EditorIcons.NoImageHighRes, ScaleMode.ScaleToFit);
            }
            catch
            {
                GUILayout.Label(EditorIcons.NoImageHighRes, GUILayout.Width(size.Value.x), GUILayout.Height(size.Value.y));
            }
        }

        internal static void DrawExpandableTextField(GUIContent label, string value, Action<string> onEdited, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            try
            {
                string newValue = EditorGUILayout.TextField(label, value, style, options);
                if (newValue != value)
                {
                    value = newValue;
                    onEdited?.Invoke(value);
                }
                if (GUILayout.Button(EditorIcons.Pick, ExStyles.miniButton, options))
                {
                    EditTextWindow.Show(label.text, value, onEdited, new Vector2(1200, 1200));
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal static bool DrawButtonField(GUIContent label)
        {
            bool pressed = false;

            GUILayout.BeginHorizontal();
            try
            {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                GUILayout.Space(EditorGUI.indentLevel * 2f);
                pressed = GUILayout.Button(label, EditorStyles.miniButton);
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            return pressed;
        }

        internal static bool DrawEnumToolbar<T>(T enumValue, out T newEnum, GUIStyle toolbarStyle, params GUILayoutOption[] options) where T : Enum
        {
            string[] texts = EnumUtil.GetInspectorNames(typeof(T));
            int selected = Convert.ToInt32(enumValue);

            toolbarStyle ??= new(EditorStyles.toolbarButton);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            int newIndex = GUILayout.Toolbar(selected, texts, toolbarStyle, options);

            GUILayout.EndHorizontal();

            if (newIndex != selected)
            {
                newEnum = (T)Enum.ToObject(typeof(T), newIndex);
                return true;
            }

            newEnum = enumValue;
            return false;
        }

        internal static void DrawErrorLabel(string label, string message, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            try
            {
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.LabelField(new GUIContent(message, EditorIcons.StatusError), ExStyles.Box(TextAnchor.MiddleCenter), GUILayout.Height(18f));
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal static bool DrawAddComponentButton(GUIContent label)
        {
            bool clicked = false;

            GUILayout.BeginHorizontal();
            try
            {
                GUILayout.FlexibleSpace();
                clicked = GUILayout.Button(label, ExStyles.addComponentButton);
                GUILayout.FlexibleSpace();
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            return clicked;
        }

        internal static double? DrawNullableDoubleSlider(GUIContent label, double? value, float defaultValue, float min, float max)
        {
            float? durAsFloat = value.HasValue ? (float)value.Value : null;
            float? newValue = ExGUILayout.NullableField(
                durAsFloat,
                defaultValue,
                v => EditorGUILayout.Slider(label, v, min, max)
            );

            if (newValue == null && value.HasValue)
            {
                return null;
            }
            else if (newValue != durAsFloat)
            {
                return (double)newValue;
            }
            else
            {
                return value;
            }
        }

        internal static bool DrawAnimatedToggle(GUIContent content, bool isOn, GUIStyle labelStyle, string uniqueKey, params GUILayoutOption[] options)
        {
            string key = uniqueKey;
            if (string.IsNullOrEmpty(key)) key = content.text;
            if (string.IsNullOrEmpty(key)) key = new Guid().ToString();
            AnimatedToggle toggle = GetAnimatedToggle(key, isOn);
            return toggle.DrawGUI(content, labelStyle, options);
        }

        internal static bool DrawAnimatedToggleLeft(GUIContent content, bool isOn, GUIStyle labelStyle, string uniqueKey, params GUILayoutOption[] options)
        {
            string key = uniqueKey;
            if (string.IsNullOrEmpty(key)) key = content.text;
            if (string.IsNullOrEmpty(key)) key = new Guid().ToString();
            AnimatedToggle toggle = GetAnimatedToggle(key, isOn);
            return toggle.DrawGUILeft(content, labelStyle, options);
        }

        internal static bool[] DrawToggleGroup(GUIContent label, string[] btnLabels, bool[] values, params GUILayoutOption[] options)
        {
            if (values == null || values.Length == 0)
            {
                EditorGUILayout.HelpBox("No toggle values provided.", MessageType.Warning);
                return Array.Empty<bool>();
            }

            GUILayout.BeginHorizontal(options);
            try
            {
                EditorGUILayout.PrefixLabel(label);

                for (int i = 0; i < values.Length; i++)
                {
                    bool first = i == 0;
                    bool last = i == values.Length - 1;
                    bool newValue;

                    string labelText = btnLabels != null && i < btnLabels.Length ? btnLabels[i] : $"Toggle {i + 1}";
                    bool value = values[i];

                    // if (first) newValue = ExGUILayout.ToggleLeft(labelText, value, GUILayout.Width(btnWidth));
                    // else if (last) newValue = ExGUILayout.ToggleRight(labelText, value, GUILayout.Width(btnWidth));
                    // else newValue = ExGUILayout.ToggleMid(labelText, value, GUILayout.Width(btnWidth));
                    if (first) newValue = ExGUILayout.ToggleLeft(labelText, value, options: GUILayout.ExpandWidth(true));
                    else if (last) newValue = ExGUILayout.ToggleRight(labelText, value, options: GUILayout.ExpandWidth(true));
                    else newValue = ExGUILayout.ToggleMid(labelText, value, options: GUILayout.ExpandWidth(true));
                    if (newValue != values[i]) values[i] = newValue;
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            return values;
        }
    }
}
