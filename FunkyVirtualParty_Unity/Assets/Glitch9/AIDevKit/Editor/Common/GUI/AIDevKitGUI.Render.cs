using System;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal partial class AIDevKitGUI
    {
        private static class Render
        {
            internal static void DrawIconContent(int index, Rect rect, GUIContent content, float iconSize = 18f)
            {
                if (content == null) return;
                float x = rect.x + index * iconSize;
                rect = new Rect(x, rect.y, iconSize, iconSize);
                EditorGUI.LabelField(rect, new GUIContent(content));
            }

            internal static void DrawCopiableLabelField(string label, GUIContent value)
            {
                if (value == null || string.IsNullOrEmpty(value.text)) value = new GUIContent("-");

                GUILayout.BeginHorizontal();
                try
                {
                    EditorGUILayout.PrefixLabel(label, AIDevKitStyles.Label);
                    if (GUILayout.Button(value, AIDevKitStyles.Label))
                    {
                        EditorGUIUtility.systemCopyBuffer = value.text;
                        Debug.Log($"Copied '{value}' to clipboard.");
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
            }

            internal static Model DrawModelPopup(Model selected, ModelFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
            {
                return _modelPopupGUI.Draw(selected, filter, label, style, apiWidth);
            }

            internal static Voice DrawVoicePopup(Voice selected, VoiceFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
            {
                return _voicePopupGUI.Draw(selected, filter, label, style, apiWidth);
            }

            internal static void DrawModelPopup(SerializedProperty property, ModelFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
            {
                if (property == null || property.propertyType != SerializedPropertyType.String)
                {
                    ExGUILayout.ErrorLabel("Only string properties are supported.");
                    return;
                }

                try
                {
                    Model selected = property.stringValue;
                    Model newModel = _modelPopupGUI.Draw(selected, filter, label, style, apiWidth);

                    if (newModel != null && newModel != selected)
                    {
                        property.stringValue = newModel.Id;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
                catch (Exception e)
                {
                    // fix Type is not supported error
                    ExGUILayout.ErrorLabel(e.Message);
                }
            }

            internal static void DrawVoicePopup(SerializedProperty property, VoiceFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
            {
                if (property == null || property.propertyType != SerializedPropertyType.String)
                {
                    ExGUILayout.ErrorLabel("Only string properties are supported.");
                    return;
                }

                try
                {
                    Voice selected = property.stringValue;
                    Voice newVoice = _voicePopupGUI.Draw(selected, filter, label, style, apiWidth);

                    if (newVoice != null && newVoice != selected)
                    {
                        property.stringValue = newVoice.Id;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
                catch (Exception e)
                {
                    ExGUILayout.ErrorLabel(e.Message);
                }
            }

            internal static string DrawModelPopup(string selectedId, ModelFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
            {
                Model selected = selectedId;
                selected = _modelPopupGUI.Draw(selected, filter, label, style, apiWidth);
                return selected.Id;
            }

            internal static string DrawVoicePopup(string selectedId, VoiceFilter filter, GUIContent label = null, PopupGUIStyle style = PopupGUIStyle.Default, float apiWidth = 100f)
            {
                Voice selected = selectedId;
                selected = _voicePopupGUI.Draw(selected, filter, label, style, apiWidth);
                return selected.Id;
            }

            internal static void DrawCurrencyField(string label, Currency value)
            {
                string display;

                if (value == null)
                {
                    display = "-";
                }
                else if (value == -1)
                {
                    display = "Free";
                }
                else
                {
                    display = value.ToString(value.CurrencyCode);
                }

                GUILayout.BeginHorizontal();
                try
                {
                    EditorGUILayout.PrefixLabel(label, AIDevKitStyles.Label);
                    EditorGUILayout.LabelField(display, AIDevKitStyles.Label);

                    value.CurrencyCode = ExGUILayout.EnumPopupEx(value.CurrencyCode, AIDevKitGUIUtility.SelectedCurrencyCodes, null, null, GUILayout.Width(50));
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
            }

            internal static void DrawTokenField(string label, int? value)
            {
                if (value.HasValue && value.Value > 2)
                {
                    EditorGUILayout.LabelField(label, $"{value.Value} tokens", AIDevKitStyles.Label);
                }
                else
                {
                    EditorGUILayout.LabelField(label, "N/A", AIDevKitStyles.Label);
                }
            }
        }
    }
}