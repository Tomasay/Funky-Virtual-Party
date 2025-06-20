using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public class TreeViewGUI
    {
        private static class Strings
        {
            internal const string UNKNOWN_TIME = "-";
            internal const string UNIX_TIME_FORMAT = "yyyy-MM-dd HH:mm tt";
            internal const string UNIX_DATE_FORMAT = "yyyy-MM-dd";
            internal const string BOOL_TRUE = "Yes";
            internal const string BOOL_FALSE = "No";
        }


        #region Cell GUIs

        public static void HeaderLabel(string text) => EditorGUILayout.LabelField(text, TreeViewStyles.HeaderLabel);
        public static bool HeaderLabelWithToggle(string text, bool value) => ExGUILayout.AnimatedToggle(text, value, TreeViewStyles.HeaderLabel);
        public static void BeginSection(string title)
        {
            HeaderLabel(title);
            GUILayout.BeginVertical(ExStyles.helpBoxedSection);
        }

        public static bool BeginSectionWithToggle(string title, bool value)
        {
            value = ExGUILayout.AnimatedToggle(title, value, TreeViewStyles.HeaderLabel);
            GUILayout.BeginVertical(ExStyles.helpBoxedSection);
            return value;
        }

        public static void BeginSectionWithoutBox(string title)
        {
            HeaderLabel(title);
            GUILayout.BeginVertical();
        }

        public static void EndSection()
        {
            GUILayout.EndVertical();
            GUILayout.Space(5f);
        }

        public static void StringCell(Rect cellRect, string text, GUIStyle style) => StringCell(cellRect, text, null, style);
        public static void StringCell(Rect cellRect, string text, string defaultText = null, GUIStyle style = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (defaultText != null)
                {
                    text = defaultText;
                }
                else return;
            }

            style ??= TreeViewStyles.StringCell;
            GUI.Label(cellRect, text, style);
        }

        public static void ContentCell(Rect cellRect, GUIContent content, GUIStyle style = null)
        {
            // trim text with ellipsis if it's too long
            if (style == null)
            {
                // style = EditorStyles.label;
                // float maxWidth = cellRect.width - style.padding.horizontal;
                // text = TrimTextWithEllipsis(text, style, maxWidth);
                GUI.Label(cellRect, content);
            }
            else
            {
                GUI.Label(cellRect, content, style);
            }
        }

        public static void FlagsCell(Rect cellRect, Enum flags, GUIStyle style = null)
        {
            if (flags == null) return;
            StringCell(cellRect, flags.ToString(), style);
        }

        public static void PriceCell(Rect cellRect, Currency price, CurrencyCode code = CurrencyCode.USD, string defaultText = null, GUIStyle style = null)
        {
            string text;

            if (price == null)
            {
                if (defaultText != null) text = defaultText;
                else return;
            }
            else
            {
                if (price == -1) text = "Free";
                else if (price == 0) text = defaultText;
                else text = price.ToString(code);
            }

            if (style == null) GUI.Label(cellRect, text);
            else GUI.Label(cellRect, text, style);
        }

        public static void UnixTimeCell(Rect cellRect, UnixTime? unixTime, GUIStyle style = null)
        {
            string timeString;
            if (unixTime == null || unixTime.Value < new UnixTime(1980, 1, 1, 1, 1, 1)) timeString = Strings.UNKNOWN_TIME;
            else timeString = unixTime.Value.ToString(Strings.UNIX_TIME_FORMAT);
            StringCell(cellRect, timeString, style);
        }

        public static void UnixDateCell(Rect cellRect, UnixTime? unixTime, GUIStyle style = null)
        {
            string timeString;
            if (unixTime == null || unixTime.Value < new UnixTime(1980, 1, 1, 1, 1, 1)) timeString = Strings.UNKNOWN_TIME;
            else timeString = unixTime.Value.ToString(Strings.UNIX_DATE_FORMAT);
            StringCell(cellRect, timeString, style);
        }

        public static void IconCell(Rect cellRect, Texture2D icon)
        {
            if (icon == null) return;
            GUI.Label(cellRect, icon, EditorStyles.centeredGreyMiniLabel);
        }

        public static void CheckCircleCell(Rect cellRect, bool value)
        {
            GUI.Label(cellRect, value ? EditorIcons.StatusCheck : EditorIcons.StatusObsolete);
        }

        public static void BoolCell(
            Rect cellRect, bool value,
            string trueText, string falseText,
            GUIStyle trueStyle = null, GUIStyle falseStyle = null)
        {
            if (value) GUI.Label(cellRect, trueText, trueStyle);
            else GUI.Label(cellRect, falseText, falseStyle);
        }

        public static bool LabeledCheckboxCell(Rect cellRect, bool value)
        {
            Rect toggleRect = cellRect;
            toggleRect.width = 20f;

            Rect labelRect = cellRect;
            labelRect.x += toggleRect.width;
            labelRect.width -= toggleRect.width;

            value = GUI.Toggle(toggleRect, value, "");
            GUI.Label(labelRect, value ? Strings.BOOL_TRUE : Strings.BOOL_FALSE);

            return value;
        }

        public static void PriceCell(Rect cellRect, float displayValue, string per)
        {
            if (displayValue == 0) return;

            Rect priceRect = cellRect;
            priceRect.width = 50f;

            Rect perRect = cellRect;
            perRect.width = cellRect.width - priceRect.width;
            perRect.x += priceRect.width;

            GUI.Label(priceRect, $"${displayValue}");
            GUI.Label(perRect, $"per {per}");
        }

        public static string DelayedTextCell(Rect cellRect, string value, EditorColor fieldColor)
        {
            return EditorGUI.DelayedTextField(cellRect, value, TreeViewStyles.TextField(12, fieldColor));
        }

        public static int DelayedIntCell(Rect cellRect, int value, int min, int max, EditorColor fieldColor)
        {
            int newValue = EditorGUI.DelayedIntField(cellRect, value, TreeViewStyles.TextField(12, fieldColor));
            return Mathf.Clamp(newValue, min, max);
        }

        public static TEnum EnumPopupCell<TEnum>(Rect cellRect, TEnum color) where TEnum : Enum
        {
            cellRect = ExGUIUtility.AdjustTreeViewRect(cellRect);
            return ExGUI.ResizableEnumPopup(cellRect, color);
        }

        public static int IntPopupMenu(string prefix, int selected, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
        {
            string[] optionNames = new string[optionValues.Length];
            for (int i = 0; i < optionValues.Length; i++)
            {
                optionNames[i] = $"{prefix}{optionValues[i]}";
            }

            return EditorGUILayout.IntPopup(selected, optionNames, optionValues, style ?? TreeViewStyles.ToolbarDropDown, options);
        }


        #endregion

        #region Edit Window GUIs

        public static void SubtitleLeft(string text, int maxCharacters = -1)
        {
            string tooltip = text;
            if (maxCharacters > 0 && text.Length > maxCharacters)
            {
                text = text.Substring(0, maxCharacters) + "...";
            }
            EditorGUILayout.LabelField(new GUIContent(text, tooltip), TreeViewStyles.ChildWindowSubtitleLeft);
        }

        public static void SubtitleRight(string text, int maxCharacters = -1)
        {
            string tooltip = text;
            if (maxCharacters > 0 && text.Length > maxCharacters)
            {
                text = text.Substring(0, maxCharacters) + "...";
            }
            EditorGUILayout.LabelField(new GUIContent(text, tooltip), TreeViewStyles.ChildWindowSubtitleRight);
        }

        public static void Subtitle(string left, string right, int maxCharacters = -1)
        {
            GUILayout.BeginHorizontal();
            {
                SubtitleLeft(left, maxCharacters);
                SubtitleRight(right, maxCharacters);
            }
            GUILayout.EndHorizontal();
        }

        #endregion


        #region Context Buttons

        public static bool SaveButton(string label = null)
        {
            if (string.IsNullOrEmpty(label)) label = "Save";
            return ContextButton(label, EditorIcons.Save, ExGUI.green);
        }

        public static bool DeleteButton(string label = null)
        {
            if (string.IsNullOrEmpty(label)) label = "Delete";
            return ContextButton(label, EditorIcons.Delete, ExGUI.red);
        }

        private readonly static Dictionary<TreeViewContextMenuType, (Color color, Texture icon)> _contextButtonStyles = new()
        {
            { TreeViewContextMenuType.None, (Color.white, EditorIcons.Settings) },
            { TreeViewContextMenuType.Add, (ExGUI.green, EditorIcons.Add) },
            { TreeViewContextMenuType.Save, (ExGUI.green, EditorIcons.Save) },
            { TreeViewContextMenuType.Delete, (ExGUI.red, EditorIcons.Delete) },
            { TreeViewContextMenuType.Revert, (Color.white, EditorIcons.Refresh) },
            { TreeViewContextMenuType.Copy, (Color.white, EditorIcons.Copy) },
            { TreeViewContextMenuType.Paste, (Color.white, EditorIcons.Clipboard) },
            { TreeViewContextMenuType.Custom, (Color.white, EditorIcons.Settings) },
        };

        public static bool ContextButton(string name, Texture tex, TreeViewContextMenuType type)
        {
            if (!_contextButtonStyles.TryGetValue(type, out var style)) return false;
            if (tex == null) tex = style.icon;
            return ContextButton(name, tex, style.color);
        }

        public static bool ContextButton(string label, Texture icon, Color color)
        {
            GUI.color = color;
            bool pressed = GUILayout.Button(new GUIContent(label, icon), TreeViewStyles.ContextButton);
            GUI.color = Color.white;
            return pressed;
        }

        #endregion
    }
}