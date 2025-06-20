using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    [CustomPropertyDrawer(typeof(HideIfFalseAttribute))]
    public class HideIfFalseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideIfFalseAttribute hideIfFalse = (HideIfFalseAttribute)attribute;
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(hideIfFalse.ConditionalSourceField);

            if (sourcePropertyValue != null && sourcePropertyValue.boolValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideIfFalseAttribute hideIfFalse = (HideIfFalseAttribute)attribute;
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(hideIfFalse.ConditionalSourceField);

            if (sourcePropertyValue != null && sourcePropertyValue.boolValue)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                // property is hidden
                return 0f;
            }
        }
    }
}