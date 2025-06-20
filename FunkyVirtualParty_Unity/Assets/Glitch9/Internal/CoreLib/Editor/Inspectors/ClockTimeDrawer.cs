using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    [CustomPropertyDrawer(typeof(ClockTime))]
    public class ClockTimeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var hour = property.FindPropertyRelative("_hour");
            var minute = property.FindPropertyRelative("_minute");

            position.height = EditorGUIUtility.singleLineHeight;

            // Indent label
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Rect hourRect = new Rect(labelRect.xMax, position.y, (position.width - labelRect.width) * 0.2f, position.height);
            Rect minuteRect = new Rect(hourRect.xMax, position.y, (position.width - labelRect.width) * 0.2f, position.height);

            EditorGUI.LabelField(labelRect, label);
            hour.intValue = Mathf.Clamp(EditorGUI.IntField(hourRect, GUIContent.none, hour.intValue), 0, 23);
            minute.intValue = Mathf.Clamp(EditorGUI.IntField(minuteRect, GUIContent.none, minute.intValue), 0, 59);
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
