using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Glitch9.Editor
{
    public interface IReorderableListItem
    {
        bool IsRemovable { get; }
    }

    public class CustomReorderableList<T>
        where T : class, IReorderableListItem, new()
    {
        private readonly ReorderableList list;
        private readonly string label;
        private readonly List<T> targetList;
        private readonly Dictionary<int, float> elementHeightCache = new();

        public CustomReorderableList(
            UnityEngine.Object target,
            IList<T> targetList,
            string propertyName,
            Func<Rect, int, T, float> elementDrawer,
            string label = null
            )
        {
            this.targetList = targetList.ToList();
            this.label = label ?? ObjectNames.NicifyVariableName(propertyName);
            list = new ReorderableList(this.targetList, typeof(T), true, true, true, true);

            list.elementHeightCallback = index =>
            {
                if (elementDrawer != null && elementHeightCache.TryGetValue(index, out float cachedHeight))
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
                var element = targetList[index];
                float height = elementDrawer(rect, index, element);
                if (height > 0) elementHeightCache[index] = height;
            };

            list.onAddCallback = l =>
            {
                T asset = new();
                targetList.Add(asset);
                EditorUtility.SetDirty(target);
            };

            list.onRemoveCallback = l =>
            {
                var element = targetList[list.index];

                if (element is IReorderableListItem iAsset)
                {
                    if (!iAsset.IsRemovable)
                    {
                        EditorUtility.DisplayDialog("Delete Item", "Item is not removable.", "OK");
                        return;
                    }
                }

                if (EditorUtility.DisplayDialog("Delete Item", "Remove item and delete asset?", "Yes", "Cancel"))
                {
                    targetList.RemoveAt(list.index);
                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            };
        }

        public void DoLayout()
        {
            list.DoLayoutList();
        }
    }
}