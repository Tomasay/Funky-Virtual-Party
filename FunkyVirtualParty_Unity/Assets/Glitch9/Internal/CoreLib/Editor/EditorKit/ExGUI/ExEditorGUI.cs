using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public static class ExEditorGUI
    {
        #region Resettable Fields
        public static void ResettableIntField(string label, SerializedProperty property, int defaultValue)
        => ResettableIntField(new GUIContent(label), property, defaultValue);
        public static void ResettableFloatField(string label, SerializedProperty property, float defaultValue)
        => ResettableFloatField(new GUIContent(label), property, defaultValue);
        public static void ResettableTextField(string label, SerializedProperty property, string defaultValue)
        => ResettableTextField(new GUIContent(label), property, defaultValue);

        public static void ResettableIntField(GUIContent label, SerializedProperty property, int defaultValue)
        {
            if (!ExGUIUtility.SerializedPropertyIsValid(property, SerializedPropertyType.Integer)) return;

            GUILayout.BeginHorizontal();
            try
            {
                int newValue = EditorGUILayout.IntField(label, property.intValue);
                if (ExGUI.ResetButton()) newValue = defaultValue;
                if (newValue != property.intValue) property.intValue = newValue;
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        public static void ResettableFloatField(GUIContent label, SerializedProperty property, float defaultValue)
        {
            if (!ExGUIUtility.SerializedPropertyIsValid(property, SerializedPropertyType.Float)) return;

            GUILayout.BeginHorizontal();
            try
            {
                float newValue = EditorGUILayout.FloatField(label, property.floatValue);
                if (ExGUI.ResetButton()) newValue = defaultValue;
                if (newValue != property.floatValue) property.floatValue = newValue;
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        public static void ResettableTextField(GUIContent label, SerializedProperty property, string defaultValue)
        {
            if (!ExGUIUtility.SerializedPropertyIsValid(property, SerializedPropertyType.String)) return;

            GUILayout.BeginHorizontal();
            try
            {
                string newValue = EditorGUILayout.TextField(label, property.stringValue);
                if (ExGUI.ResetButton()) newValue = defaultValue;
                if (newValue != property.stringValue) property.stringValue = newValue;
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        public static void ResettableEnumPopup<T>(string label, SerializedProperty property, T defaultValue) where T : Enum
        => ResettableEnumPopup(new GUIContent(label), property, defaultValue);
        public static void ResettableEnumPopup<T>(GUIContent label, SerializedProperty property, T defaultValue) where T : Enum
        {
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogError("SerializedProperty must be of enum type.");
                return;
            }

            GUILayout.BeginHorizontal();

            string[] displayNames = property.enumDisplayNames;
            int enumIndex = property.enumValueIndex;
            int newEnumIndex = EditorGUILayout.Popup(label, enumIndex, displayNames);

            if (newEnumIndex != enumIndex)
            {
                property.enumValueIndex = newEnumIndex;
            }

            if (ExGUI.ResetButton()) property.enumValueIndex = Convert.ToInt32(defaultValue);

            GUILayout.EndHorizontal();
        }

        #endregion Resettable Fields 

        #region Recording Trigger Key Popup

        public static void RecordingTriggerKeyPopup(string label, SerializedProperty property)
        => RecordingTriggerKeyPopup(new GUIContent(label), property);

        public static void RecordingTriggerKeyPopup(GUIContent label, SerializedProperty property)
        {
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogError("SerializedProperty must be of enum type.");
                return;
            }

            const float btnWidth = 60f;

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15));

                KeyCode currentKey = (KeyCode)property.enumValueIndex;
                KeyCode newKey = (KeyCode)EditorGUILayout.EnumPopup(currentKey);

                if (newKey != currentKey)
                {
                    property.enumValueIndex = (int)newKey;
                    property.serializedObject.ApplyModifiedProperties();
                }

                if (GUILayout.Button("Tab", EditorStyles.miniButtonMid, GUILayout.Width(btnWidth)))
                {
                    property.enumValueIndex = (int)KeyCode.Tab;
                    property.serializedObject.ApplyModifiedProperties();
                }

                if (GUILayout.Button("Tilde (~)", EditorStyles.miniButtonMid, GUILayout.Width(btnWidth)))
                {
                    property.enumValueIndex = (int)KeyCode.Tilde;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        #endregion Recording Trigger Key Popup

        #region Path Fields
        public static void PathField(string label, SerializedProperty property, string rootPath, string defaultPath)
        => PathField(new GUIContent(label), property, rootPath, defaultPath);
        public static void PathField(GUIContent label, SerializedProperty property, string rootPath, string defaultPath)
        => ExGUIRenderer.DrawPathField(label, property, rootPath, defaultPath);

        #endregion Path Fields 


        #region Toggle Selector (with multiple bool values) 

        public static void ToggleSelector(string label, params SerializedProperty[] properties)
        => ToggleSelector(new GUIContent(label), properties);

        public static void ToggleSelector(GUIContent label, params SerializedProperty[] properties)
        {
            if (properties == null || properties.Length == 0) return;

            int count = properties.Length;
            // float width = ExGUIUtility.DivideViewWidthWithoutLabel(count);

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.PrefixLabel(label);

                int oldSelected = -1;
                int newSelected = -1;

                for (int i = 0; i < count; i++)
                {
                    bool value = properties[i].boolValue;
                    if (value) oldSelected = i;

                    GUIStyle style = ExGUIUtility.GetToggleStyle(i, count);

                    bool newValue = GUILayout.Toggle(value, properties[i].displayName, style, GUILayout.ExpandWidth(true));
                    if (newValue) newSelected = i;

                    if (oldSelected != newSelected)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            properties[j].boolValue = j == newSelected;
                            properties[j].serializedObject.ApplyModifiedProperties();
                        }
                        break;
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }


        #endregion Toggle Selector (with multiple bool values)


        #region ExpandableTextField

        public static void ExpandableTextField(string label, SerializedProperty property, GUIStyle style = null, params GUILayoutOption[] options)
        => ExpandableTextField(new GUIContent(label), property, style, options);
        public static void ExpandableTextField(GUIContent label, SerializedProperty property, GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            try
            {
                string value = property.stringValue;
                value = EditorGUILayout.TextField(label, value, style ?? ExStyles.textField, options);
                if (GUILayout.Button(EditorIcons.Pick, ExStyles.miniButton))
                {
                    EditTextWindow.Show(label.text, value, (edited) =>
                    {
                        property.stringValue = edited;
                        property.serializedObject.ApplyModifiedProperties();
                    }, new Vector2(1200, 1200));
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        #endregion ExpandableTextField

        #region Enum Switch (Group of Toggle Buttons) 

        public static void EnumToggle<T>(string label, SerializedProperty property, T[] displayedValues = null) where T : Enum
        => EnumToggle(new GUIContent(label), property, displayedValues);

        public static void EnumToggle<T>(GUIContent label, SerializedProperty property, T[] displayedValued = null) where T : Enum
        {
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogError("SerializedProperty must be of enum type.");
                return;
            }

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.PrefixLabel(label);

                displayedValued ??= (T[])Enum.GetValues(typeof(T));

                int count = displayedValued.Length;
                //float width = ExGUIUtility.DivideViewWidthWithoutLabel(count);

                for (int i = 0; i < count; i++)
                {
                    bool isSelected = property.enumValueIndex == i;
                    bool newValue = GUILayout.Toggle(isSelected, displayedValued[i].ToString(), ExGUIUtility.GetToggleStyle(i, count), GUILayout.ExpandWidth(true));

                    if (newValue && !isSelected)
                    {
                        property.enumValueIndex = i;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        public static void EnumFlagToggle<T>(string label, SerializedProperty property, T[] displayedValues = null) where T : Enum
        => EnumFlagToggle(new GUIContent(label), property, displayedValues);

        public static void EnumFlagToggle<T>(GUIContent label, SerializedProperty property, T[] displayedValues = null) where T : Enum
        {
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                Debug.LogError("SerializedProperty must be of enum type.");
                return;
            }

            GUILayout.BeginHorizontal();
            try
            {
                EditorGUILayout.PrefixLabel(label);

                displayedValues ??= (T[])Enum.GetValues(typeof(T));

                int count = displayedValues.Length;

                for (int i = 0; i < count; i++)
                {
                    bool isSelected = (Convert.ToInt32(property.enumValueIndex) & (1 << i)) != 0;
                    bool newValue = GUILayout.Toggle(isSelected, displayedValues[i].ToString(), ExGUIUtility.GetToggleStyle(i, count), GUILayout.ExpandWidth(true));

                    if (newValue != isSelected)
                    {
                        if (newValue)
                        {
                            property.enumValueIndex |= (1 << i);
                        }
                        else
                        {
                            property.enumValueIndex &= ~(1 << i);
                        }
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        #endregion Enum Switch (Group of Toggle Buttons)

    }
}