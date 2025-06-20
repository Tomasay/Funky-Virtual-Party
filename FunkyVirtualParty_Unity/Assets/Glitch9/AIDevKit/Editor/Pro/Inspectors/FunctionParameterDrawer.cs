using Glitch9.AIDevKit.Components;
using Glitch9.IO.Json.Schema;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomPropertyDrawer(typeof(FunctionParameter))]
    public class FunctionParameterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProp = property.FindPropertyRelative("name");
            var typeProp = property.FindPropertyRelative("type");
            var descProp = property.FindPropertyRelative("description");
            var elementTypeProp = property.FindPropertyRelative("elementType");

            float indent = EditorGUI.indentLevel * 15f + -2f;
            float labelWidth = EditorGUIUtility.labelWidth - indent;
            float fieldWidth = position.width - labelWidth;

            // Label: name (type) or name (array<type>)
            JsonSchemaType type = (JsonSchemaType)typeProp.enumValueIndex;
            string typeLabel = type.ToString().ToLower();

            if (type == JsonSchemaType.Array && elementTypeProp != null)
            {
                JsonSchemaType innerType = (JsonSchemaType)elementTypeProp.enumValueIndex;
                typeLabel = $"array<{innerType.ToString().ToLower()}>";
            }

            GUIContent combinedLabel = new($"{nameProp.stringValue} ({typeLabel})");

            Rect labelRect = new(position.x, position.y, labelWidth, position.height);
            Rect fieldRect = new(position.x + labelWidth, position.y, fieldWidth, position.height);

            EditorGUI.LabelField(labelRect, combinedLabel);
            EditorGUI.PropertyField(fieldRect, descProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
