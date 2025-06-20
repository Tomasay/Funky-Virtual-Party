using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public static class EditorExtensions
    {
        public static Type GetEnumType(this SerializedProperty property)
        {
            // Get the object that the property belongs to
            object targetObject = property.serializedObject.targetObject;

            // Use reflection to get the FieldInfo of the property
            Type targetType = targetObject.GetType();
            FieldInfo fieldInfo = targetType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null && fieldInfo.FieldType.IsEnum)
            {
                // Return the enum type
                return fieldInfo.FieldType;
            }

            return null;
        }

        public static T SafeGetEnumValue<T>(this SerializedProperty property) where T : Enum
        {
            // Get the enum type
            string[] names = property.enumNames;
            int index = property.enumValueIndex;
            if (index >= 0 && index < names.Length)
            {
                // Return the enum value
                return (T)Enum.Parse(typeof(T), names[index]);
            }

            return names.Length > 0 ? (T)Enum.Parse(typeof(T), names[0]) : default;
        }

        public static void SafeSetEnumValue<T>(this SerializedProperty property, T value) where T : Enum
        {
            // Get the enum type
            string[] names = property.enumNames;
            int newIndex = Array.IndexOf(names, value.ToString());
            if (newIndex >= 0)
            {
                property.enumValueIndex = newIndex;
            }
            else
            {
                Debug.LogWarning($"Enum value '{value}' not found in enum names.");
            }
        }

        public static void AddItem(this GenericMenu menu, string label, GenericMenu.MenuFunction action, bool isChecked = false)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu), "GenericMenu cannot be null.");
            }

            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException("Label cannot be null or empty.", nameof(label));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Action cannot be null.");
            }

            menu.AddItem(new GUIContent(label), isChecked, action);
        }

        public static void AddUserPreferences(this GenericMenu menu, string label, string preferencesPath, bool isChecked = false)
        => AddItem(menu, label, () => SettingsService.OpenUserPreferences(preferencesPath), isChecked);
    }
}