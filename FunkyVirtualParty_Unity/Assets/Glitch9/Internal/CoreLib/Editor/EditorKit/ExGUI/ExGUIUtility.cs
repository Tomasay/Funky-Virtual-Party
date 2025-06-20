using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Glitch9.Editor
{
    public static partial class ExGUIUtility
    {
        public static void DrawHorizontalLine(float thickness = 1f, int padding = 0)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.x -= 20;
            r.width += 40;
            EditorGUI.DrawRect(r, new Color(0.635f, 0.635f, 0.635f));
        }

        public static void DrawTitleLine()
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(5f));
            r.height = 1.2f;
            r.width -= 4;
            EditorGUI.DrawRect(r, Color.gray);
        }

        public static void DrawCircle(Rect r, Texture2D image = null, float margin = 6)
        {
            var circleTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Codeqo/GUISkins/circle_android12.psd");
            GUI.DrawTexture(r, circleTex);
            r = new Rect(r.x + margin / 2, r.y + margin / 2, r.width - margin, r.height - margin);
            GUI.DrawTexture(r, image);
        }

        public static Rect DrawRoundedTextureAndroid(float size, Texture2D tex)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(size - 2), GUILayout.Height(size - 2));
            GUI.DrawTexture(r, tex);
            r = new Rect(r.x - 1, r.y - 1, r.width + 2, r.height + 2);
            GUI.Box(r, "", GUI.skin.label);
            return r;
        }

        public static Rect DrawRoundedTextureiOS(float size, Texture2D tex)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(size - 2), GUILayout.Height(size - 2));
            GUI.DrawTexture(r, tex);
            r = new Rect(r.x - 1, r.y - 1, r.width + 2, r.height + 2);
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                normal = { background = EditorTextures.iOSRoundedCorners }
            };
            GUI.Box(r, "", style);
            return r;
        }

        public static void OverrideSprite(SerializedProperty obj, SerializedProperty spr)
        {
            if (obj.objectReferenceValue != null && spr.objectReferenceValue == null)
            {
                if (obj.objectReferenceValue is GameObject go && go.TryGetComponent(out Image img))
                {
                    spr.objectReferenceValue = img.sprite;
                    Debug.Log("Sprite found. This game object already has a sprite.");
                }
            }
        }

        public static void DrawBorders(Rect rect)
        {
            float thickness = 1.2f;
            Color prevColor = GUI.color;
            GUI.color = Color.gray;

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), EditorGUIUtility.whiteTexture);

            GUI.color = prevColor;
        }

        public static void DrawTopAndBottomBorders(Rect rect)
        {
            float thickness = 1.2f;
            Color prevColor = GUI.color;
            GUI.color = Color.gray;

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness + 2f, rect.width, thickness), EditorGUIUtility.whiteTexture);

            GUI.color = prevColor;
        }

        public static void DrawTopBorder(Rect rect)
        {
            float thickness = 1.2f;
            Color prevColor = GUI.color;
            GUI.color = Color.gray;

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), EditorGUIUtility.whiteTexture);
            GUI.color = prevColor;
        }

        public static Texture2D CreateTexture(int width, int height, Color col)
        {
            Texture2D result = new(width, height);
            Color[] pix = Enumerable.Repeat(col, width * height).ToArray();
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public static void RecalculateColumnWidths(Rect position, int horizontalPaddingsSum, ref MultiColumnHeader header)
        {
            int count = header.state.columns.Length;
            float fixedTotal = horizontalPaddingsSum;
            List<int> auto = new();

            for (int i = 0; i < count; i++)
            {
                if (header.state.columns[i].autoResize) auto.Add(i);
                else fixedTotal += header.state.columns[i].minWidth;
            }

            float remain = position.width - fixedTotal;
            float widthPerAuto = remain / auto.Count;

            foreach (var col in header.state.columns)
                col.width = col.autoResize ? widthPerAuto : col.minWidth;
        }

        public static Rect AdjustTreeViewRect(Rect cellRect) => new(cellRect.x - 3, cellRect.y, cellRect.width + 6, cellRect.height);
        public static Rect AdjustTreeViewMenuRect(Rect cellRect) => new(cellRect.x - 3, cellRect.y - 1.2f, cellRect.width, cellRect.height);
        public static void DrawHorizontalBorder(float y) => GUI.DrawTexture(new Rect(0, y, EditorGUIUtility.currentViewWidth, 1.2f), EditorTextures.borderTexture);

        private const float kHPad = 7, kVPad = 4, kSpace = 3, kOffsetY = 2;

        public static ReorderableList CreateReorderableArray<T>(T[] array, Func<Rect, int, T, T> drawer, Action<T[]> onEdit, GUIContent label = null, float heightMul = 1f) =>
            CreateReorderableList(array, drawer, label, _ => heightMul, onEdit);

        public static ReorderableList CreateReorderableList<T>(T[] array, Func<Rect, int, T, T> drawer, GUIContent label, Func<int, float> heightMul, Action<T[]> onEdit = null)
        {
            label ??= new GUIContent("List");
            var list = new ReorderableList(array, typeof(T), true, true, true, true);
            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, label);

            list.drawElementCallback = (rect, i, a, f) =>
            {
                rect.y += kOffsetY;
                var box = new Rect(rect.x, rect.y, rect.width, rect.height - kSpace);
                GUI.Box(box, GUIContent.none, EditorStyles.helpBox);

                var content = new Rect(box.x + kHPad, box.y + kVPad, box.width - kHPad * 2, box.height - kVPad * 2 - kSpace);
                array[i] = drawer(content, i, array[i]);
            };

            list.elementHeightCallback = i => EditorGUIUtility.singleLineHeight * heightMul(i) + (kVPad * 2 + kSpace);

            list.onAddCallback = rl =>
            {
                Array.Resize(ref array, array.Length + 1);
                array[^1] = default;
                rl.list = array;
                onEdit?.Invoke(array);
            };

            list.onRemoveCallback = rl =>
            {
                if (!EditorUtility.DisplayDialog("Warning", "Delete this element?", "Yes", "No")) return;
                int idx = rl.index;
                if (idx < 0 || idx >= array.Length) return;
                array = array.Where((_, i) => i != idx).ToArray();
                rl.list = array;
                onEdit?.Invoke(array);
            };

            return list;
        }

        public static ReorderableList CreateReorderableDictionary(Dictionary<string, string> dict, GUIContent label = null, float heightMul = 1f)
        {
            label ??= new GUIContent("Dictionary");
            var list = new List<KeyValuePair<string, string>>(dict);
            var rl = new ReorderableList(list, typeof(KeyValuePair<string, string>), true, true, true, true);
            rl.drawHeaderCallback = rect => EditorGUI.LabelField(rect, label);

            rl.drawElementCallback = (rect, i, a, f) =>
            {
                var entry = list[i];
                rect.y += kOffsetY;
                var box = new Rect(rect.x, rect.y, rect.width, rect.height - kSpace);
                GUI.Box(box, GUIContent.none, EditorStyles.helpBox);
                var content = new Rect(box.x + kHPad, box.y + kVPad, box.width - kHPad * 2, box.height - kVPad * 2 - kSpace);

                float half = content.width / 2 - 2;
                string key = EditorGUI.TextField(new Rect(content.x, content.y, half, content.height), entry.Key);
                string val = EditorGUI.TextField(new Rect(content.x + half + 4, content.y, half, content.height), entry.Value);

                if (key != entry.Key || val != entry.Value)
                {
                    dict.Remove(entry.Key);
                    dict[key] = val;
                    list[i] = new(key, val);
                }
            };

            rl.elementHeightCallback = _ => EditorGUIUtility.singleLineHeight * heightMul + (kVPad * 2 + kSpace);

            rl.onAddCallback = _ =>
            {
                var newEntry = new KeyValuePair<string, string>("newKey", "newValue");
                list.Add(newEntry);
                dict.Add(newEntry.Key, newEntry.Value);
            };

            rl.onRemoveCallback = _ =>
            {
                var entry = list[rl.index];
                dict.Remove(entry.Key);
                ReorderableList.defaultBehaviours.DoRemoveButton(rl);
            };

            return rl;
        }

        public static void DragAndDropArea(Rect area, Action<string> onDrop)
        {
            if (onDrop == null) return;
            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!area.Contains(evt.mousePosition)) return;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(obj);
                            Debug.Log("Dragged file: " + path);
                            onDrop(path);
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }

        internal static Rect ResolveRect(string text, GUIStyle style, float maxWidth) =>
            GUILayoutUtility.GetRect(new GUIContent(text), style, GUILayout.Height(style.CalcHeight(new GUIContent(text), maxWidth)));

        internal static float GetLabelWidth()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float indentOffset = EditorGUI.indentLevel * 15f;
            return labelWidth - indentOffset;
        }

        public static float GetLineHeight(int lines) =>
            lines <= 0 ? 0f : EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * lines - 1;

        public static Color GetFontColor(EditorColor color)
        {
            return color switch
            {
                EditorColor.Green => ExColor.teal,
                EditorColor.Blue => ExColor.azure,
                EditorColor.Yellow => ExColor.citrine,
                EditorColor.Purple => ExColor.purple,
                EditorColor.Red => ExColor.rose,
                EditorColor.Orange => ExColor.orange,
                EditorColor.Gray => Color.gray,
                _ => ExGUI.IsDarkMode ? Color.white : Color.black,
            };
        }

        public static string GetHexColor(EditorColor color)
        {
            return color switch
            {
                EditorColor.Green => "#0dff0d",
                EditorColor.Blue => "#4592ff",
                EditorColor.Yellow => "#FFFF00",
                EditorColor.Purple => "#a538ff",
                EditorColor.Red => "#ff3838",
                EditorColor.Orange => "#FFA500",
                EditorColor.Gray => "#808080",
                _ => ExGUI.IsDarkMode ? "#FFFFFF" : "#000000",
            };
        }

        internal static bool SerializedPropertyIsValid(SerializedProperty property, SerializedPropertyType type)
        {
            if (property == null || property.propertyType != type)
            {
                Debug.LogError($"SerializedProperty must be of {type} type.");
                return false;
            }
            return true;
        }

        internal static string OpenFolderPanel(string path, string rootPath)
        {
            string directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                directory = rootPath;
            }

            string newFolderPath = EditorUtility.OpenFolderPanel("Select Folder", directory, "");
            if (!string.IsNullOrEmpty(newFolderPath) && newFolderPath != path)
            {
                path = newFolderPath;
            }

            return path;
        }

        internal static GUIStyle GetToggleStyle(int index, int count)
        {
            if (index == 0) return EditorStyles.miniButtonLeft;
            if (index == count - 1) return EditorStyles.miniButtonRight;
            return EditorStyles.miniButtonMid;
        }

        internal static float DivideViewWidthWithoutLabel(int count)
        {
            float width = EditorGUIUtility.currentViewWidth - 20;
            float totalWidth = 0f;
            for (int i = 0; i < count; i++)
            {
                totalWidth += EditorGUIUtility.labelWidth + 10f;
            }
            return (width - totalWidth) / count;
        }

        internal static void OpenFolder(string path, string defaultPath = null)
        {
            if (string.IsNullOrEmpty(path)) path = defaultPath ?? Application.persistentDataPath;
            path = Path.GetFullPath(path);
            Debug.Log($"Opening output folder: {path}");
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }
}
