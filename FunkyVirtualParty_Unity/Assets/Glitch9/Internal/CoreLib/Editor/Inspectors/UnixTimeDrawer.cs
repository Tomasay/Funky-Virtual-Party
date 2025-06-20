
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    [CustomPropertyDrawer(typeof(UnixTime))]
    public class UnixTimeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty unixTimeAsLong = property.FindPropertyRelative("_value");
            UnixTime unixTime = new(unixTimeAsLong.longValue);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            Rect currentPosition = EditorGUI.PrefixLabel(position, label);
            position.height = singleLineHeight;

            // Indent label
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            UnixTime newUnixTime = UnixTimeField(currentPosition, null, unixTime, true, true, true, true, true, true);

            if (newUnixTime != unixTime)
            {
                unixTimeAsLong.longValue = newUnixTime.Value;
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private static UnixTime UnixTimeField(Rect rect, GUIContent label, UnixTime unixTime, bool year, bool month, bool day, bool hour = false, bool minute = false, bool second = false)
        {
            float originalX = rect.x;
            const float SMALL_SPACE = 10f;
            const float LARGE_SPACE = 20f;

            if (label != null)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, rect.height), label);
                rect.x += labelWidth;
            }

            int YY = unixTime.Year;
            int MM = unixTime.Month;
            int DD = unixTime.Day;
            int hh = unixTime.Hour;
            int mm = unixTime.Minute;
            int ss = unixTime.Second;

            if (year)
            {
                rect.width = 50;
                YY = EditorGUI.IntField(rect, YY);
                rect.x += rect.width + SMALL_SPACE;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, SMALL_SPACE, rect.height), "-");
                rect.x += SMALL_SPACE;
            }

            if (month)
            {
                rect.width = 30;
                MM = EditorGUI.IntField(rect, MM);
                rect.x += rect.width + SMALL_SPACE;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, SMALL_SPACE, rect.height), "-");
                rect.x += SMALL_SPACE;
            }

            if (day)
            {
                rect.width = 30;
                DD = EditorGUI.IntField(rect, DD);
                rect.x += rect.width;
            }

            if (hour)
            {
                rect.x += LARGE_SPACE;
                rect.width = 30;
                hh = EditorGUI.IntField(rect, hh);
                rect.x += rect.width + SMALL_SPACE;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, SMALL_SPACE, rect.height), ":");
                rect.x += SMALL_SPACE;
            }

            if (minute)
            {
                rect.width = 30;
                mm = EditorGUI.IntField(rect, mm);
                rect.x += rect.width + SMALL_SPACE;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, SMALL_SPACE, rect.height), ":");
                rect.x += SMALL_SPACE;
            }

            if (second)
            {
                rect.width = 30;
                ss = EditorGUI.IntField(rect, ss);
            }

            return new UnixTime(YY, MM, DD, hh, mm, ss);
        }
    }
}