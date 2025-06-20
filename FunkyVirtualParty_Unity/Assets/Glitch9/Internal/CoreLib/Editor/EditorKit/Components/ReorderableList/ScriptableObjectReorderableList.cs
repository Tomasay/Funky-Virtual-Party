using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Glitch9.Editor
{
    public class ScriptableObjectReorderableList<T>
        where T : ScriptableObject, IReorderableListItem
    {
        private readonly SerializedObject serializedObject;
        private readonly SerializedProperty listProperty;
        private readonly ReorderableList list;
        private readonly string assetFolder;
        private readonly string label;
        private readonly UnityEngine.Object targetObject;
        private readonly Dictionary<int, float> elementHeightCache = new();

        public ScriptableObjectReorderableList(
            UnityEngine.Object target,
            SerializedObject so,
            string propertyName,
            string assetFolderPath,
            Func<Rect, int, SerializedProperty, float> customElementDrawer = null,
            string label = null
            )
        {
            this.targetObject = target;
            this.serializedObject = so;
            this.listProperty = so.FindProperty(propertyName);
            this.assetFolder = assetFolderPath;
            this.label = label ?? ObjectNames.NicifyVariableName(propertyName);

            if (!AssetDatabase.IsValidFolder(assetFolder))
            {
                Directory.CreateDirectory(assetFolder);
                AssetDatabase.Refresh();
            }

            list = new ReorderableList(so, listProperty, true, true, true, true);

            list.elementHeightCallback = index =>
            {
                if (customElementDrawer != null && elementHeightCache.TryGetValue(index, out float cachedHeight))
                {
                    return cachedHeight;
                }

                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            };

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, this.label);
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = listProperty.GetArrayElementAtIndex(index);

                if (customElementDrawer != null)
                {
                    float height = customElementDrawer(rect, index, element);
                    if (height > 0) elementHeightCache[index] = height;
                }
                else
                {
                    EditorGUI.PropertyField(rect, element, GUIContent.none);
                }
            };

            list.onAddCallback = l =>
            {
                T asset = ScriptableObject.CreateInstance<T>();
                string path = AssetDatabase.GenerateUniqueAssetPath($"{assetFolder}/{typeof(T).Name}_{Guid.NewGuid().ToString().Substring(0, 8)}.asset");
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();

                var targetList = GetTargetList();
                targetList.Add(asset);
                EditorUtility.SetDirty(targetObject);
                serializedObject.Update();
            };

            list.onRemoveCallback = l =>
            {
                var element = listProperty.GetArrayElementAtIndex(list.index);
                var asset = element.objectReferenceValue;

                if (asset is IReorderableListItem iAsset)
                {
                    if (!iAsset.IsRemovable)
                    {
                        EditorUtility.DisplayDialog("Delete Item", "Item is not removable.", "OK");
                        return;
                    }
                }

                if (EditorUtility.DisplayDialog("Delete Item", "Remove item and delete asset?", "Yes", "Cancel"))
                {
                    if (asset != null)
                    {
                        string path = AssetDatabase.GetAssetPath(asset);
                        AssetDatabase.DeleteAsset(path);
                    }

                    listProperty.DeleteArrayElementAtIndex(list.index);
                    serializedObject.ApplyModifiedProperties();
                }
            };
        }

        public void DoLayout()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private List<T> GetTargetList()
        {
            var field = targetObject.GetType().GetField(
                listProperty.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            return field?.GetValue(targetObject) as List<T>;
        }
    }
}
