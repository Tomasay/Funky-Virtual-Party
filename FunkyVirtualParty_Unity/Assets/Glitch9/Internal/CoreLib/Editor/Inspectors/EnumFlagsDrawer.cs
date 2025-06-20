using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public class EnumFlagsDrawer<T> : PropertyDrawer where T : Enum
    {
        private GUIStyle ResolveStyle(bool isFirst, bool isLast)
        {
            if (isFirst) return EditorStyles.miniButtonLeft;
            if (isLast) return EditorStyles.miniButtonRight;
            return EditorStyles.miniButtonMid;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Get the enum value as an int
            int intValue = property.intValue;

            // Get all possible values of the enum
            T enumValue = (T)Enum.ToObject(typeof(T), intValue);

            // Get the names of the enum values
            string[] enumNames = property.enumDisplayNames;

            // Calculate the width of each button
            float buttonWidth = position.width / enumNames.Length;

            // Create a rect for the buttons
            Rect buttonRect = new(position.x, position.y, buttonWidth, position.height);

            // Iterate through each enum value and create a toggle button
            for (int i = 0; i < enumNames.Length; i++)
            {
                T enumFlag = (T)Enum.ToObject(typeof(T), 1 << i);

                bool isFirst = i == 0;
                bool isLast = i == enumNames.Length - 1;

                GUIStyle style = ResolveStyle(isFirst, isLast);

                bool isSelected;

                if (i == 0)
                {
                    isSelected = intValue == 0;
                }
                else
                {
                    isSelected = enumValue.HasFlag(enumFlag);
                }

                bool toggled = GUI.Toggle(buttonRect, isSelected, enumNames[i], style);

                if (toggled != isSelected)
                {
                    if (i == 0) // Selecting "None (O)", need to deselect all but 'None'
                    {
                        property.intValue = 0;
                    }
                    else
                    {
                        if (isSelected)
                        {
                            intValue &= ~(1 << i);
                        }
                        else
                        {
                            intValue |= 1 << i;
                        }

                        property.intValue = intValue;
                    }
                }


                buttonRect.x += buttonWidth;
            }

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}