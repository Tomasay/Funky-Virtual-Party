// FunctionReferenceDrawer.cs (Full fix: parameter appears on target/method change)
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using Glitch9.AIDevKit.Components;
using Glitch9.IO.Json.Schema;

namespace Glitch9.AIDevKit.Editor
{
    [CustomPropertyDrawer(typeof(FunctionReference))]
    public class FunctionReferenceDrawer : PropertyDrawer
    {
        private static readonly GUIContent kTargetLabel = new("Script", "The target MonoBehaviour that contains the method to use as a function.");
        private static readonly GUIContent kMethodLabel = new("Method", "The method to use as a function.");
        private static readonly GUIContent kDescriptionLabel = new("Description", "A description of the function.");
        private static readonly GUIContent kParametersLabel = new("Parameters", "The parameters of the function.");

        private const int kMethodDescriptionHeight = 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("target");
            var methodProp = property.FindPropertyRelative("methodName");
            var descProp = property.FindPropertyRelative("description");
            var paramsProp = property.FindPropertyRelative("parameters");

            float lineHeight = EditorGUIUtility.singleLineHeight + 2f;
            Rect currentRect = new(position.x, position.y, position.width, lineHeight);

            // Always draw target
            EditorGUI.PropertyField(currentRect, targetProp, kTargetLabel);
            currentRect.y += lineHeight;

            MonoBehaviour targetObj = targetProp.objectReferenceValue as MonoBehaviour;

            if (targetObj == null)
            {
                EditorGUI.EndProperty();
                return;
            }

            if (string.IsNullOrEmpty(methodProp.stringValue))
            {
                var defaultMethod = targetObj.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .FirstOrDefault(m => m.ReturnType == typeof(void));

                if (defaultMethod != null)
                {
                    methodProp.stringValue = defaultMethod.Name;
                    property.serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                }
            }

            MethodInfo[] methods = targetObj.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.ReturnType == typeof(void))
                .ToArray();

            GUIContent[] methodNames = methods.Select(m => new GUIContent(m.Name)).ToArray();
            int currentIndex = Array.FindIndex(methods, m => m.Name == methodProp.stringValue);
            if (currentIndex < 0) currentIndex = 0;

            int selectedIndex = EditorGUI.Popup(currentRect, kMethodLabel, currentIndex, methodNames);
            currentRect.y += lineHeight;

            MethodInfo selectedMethod = (selectedIndex >= 0 && selectedIndex < methods.Length) ? methods[selectedIndex] : null;

            if (selectedIndex != currentIndex && selectedMethod != null)
            {
                methodProp.stringValue = selectedMethod.Name;
                property.serializedObject.ApplyModifiedProperties();
                GUI.changed = true;
            }

            ParameterInfo[] parameters = selectedMethod?.GetParameters() ?? Array.Empty<ParameterInfo>();

            currentRect.height *= kMethodDescriptionHeight;
            EditorGUI.PropertyField(currentRect, descProp, kDescriptionLabel);
            currentRect.y += lineHeight * kMethodDescriptionHeight;

            if (paramsProp.arraySize != parameters.Length)
            {
                paramsProp.serializedObject.Update();

                while (paramsProp.arraySize < parameters.Length)
                    paramsProp.InsertArrayElementAtIndex(paramsProp.arraySize);

                for (int i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    var entryProp = paramsProp.GetArrayElementAtIndex(i);

                    if (entryProp.managedReferenceValue == null)
                    {
                        string paramName = parameters[i].Name;
                        JsonSchemaType schemaType = JsonSchemaTypes.ConvertType(paramType);
                        FunctionParameter parameter;

                        if (schemaType == JsonSchemaType.Enum)
                        {
                            parameter = new FunctionParameter(paramName, paramType);
                        }
                        else if (schemaType == JsonSchemaType.Array)
                        {
                            Type elementType = paramType.IsArray
                                ? paramType.GetElementType()
                                : paramType.GetGenericArguments().FirstOrDefault();

                            parameter = new FunctionParameter(
                                schemaType,
                                paramName,
                                JsonSchemaTypes.ConvertType(elementType)
                            );
                        }
                        else
                        {
                            parameter = new FunctionParameter(schemaType, paramName);
                        }


                        entryProp.managedReferenceValue = parameter;
                        paramsProp.serializedObject.ApplyModifiedProperties();
                    }
                }

                while (paramsProp.arraySize > parameters.Length)
                    paramsProp.DeleteArrayElementAtIndex(paramsProp.arraySize - 1);

                paramsProp.serializedObject.ApplyModifiedProperties();
            }

            bool noParameters = parameters.Length == 0;

            if (!noParameters)
            {
                // just show the parameters with PropertyField
                //EditorGUI.PropertyField(currentRect, paramsProp, kParametersLabel, true); 
                // Set up label alignment
                float labelWidth = EditorGUIUtility.labelWidth;
                lineHeight = EditorGUIUtility.singleLineHeight + 2f;

                Rect foldoutRect = new(currentRect.x, currentRect.y, currentRect.width, lineHeight);
                paramsProp.isExpanded = EditorGUI.Foldout(foldoutRect, paramsProp.isExpanded, kParametersLabel, true);
                currentRect.y += lineHeight;

                if (paramsProp.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < paramsProp.arraySize; i++)
                    {
                        var entryProp = paramsProp.GetArrayElementAtIndex(i);
                        float entryHeight = EditorGUI.GetPropertyHeight(entryProp, true);

                        EditorGUI.PropertyField(
                            new Rect(currentRect.x, currentRect.y, currentRect.width, entryHeight),
                            entryProp,
                            GUIContent.none,
                            true
                        );

                        currentRect.y += entryHeight + 2;
                    }
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2f;

            height += lineHeight; // target
            height += lineHeight; // method
            height += lineHeight * kMethodDescriptionHeight; // description

            var targetProp = property.FindPropertyRelative("target");
            var methodProp = property.FindPropertyRelative("methodName");
            var paramsProp = property.FindPropertyRelative("parameters");

            MonoBehaviour targetObj = targetProp.objectReferenceValue as MonoBehaviour;
            if (targetObj != null && !string.IsNullOrEmpty(methodProp.stringValue))
            {
                var method = targetObj.GetType().GetMethod(methodProp.stringValue);
                if (method != null)
                {
                    var parameters = method.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (i >= paramsProp.arraySize) continue;
                        var entryProp = paramsProp.GetArrayElementAtIndex(i);
                        var descPropInner = entryProp.FindPropertyRelative("description");

                        if (descPropInner == null) continue;

                        height += lineHeight;
                    }
                }
            }

            // ✅ apply expanded state height
            if (paramsProp.isExpanded)
            {
                for (int i = 0; i < paramsProp.arraySize; i++)
                {
                    var entryProp = paramsProp.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(entryProp, true) + 2;
                }
            }

            return height;
        }

    }
}