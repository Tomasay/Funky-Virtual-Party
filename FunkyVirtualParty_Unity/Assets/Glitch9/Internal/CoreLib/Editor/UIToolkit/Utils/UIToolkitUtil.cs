using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public static class UIToolkitUtil
    {
        private const string kCoreMarkerFileName = "ui_toolkit_marker";
        private const string kExColorsStyle = "ExColors";
        private const string kExStylesStyle = "ExStyles";
        private static Dictionary<string, string> _stylePathsCache = new();

        private static string GetBasePath(string markerFileName)
        {
            // 현재 스크립트의 경로를 기반으로 Assets/USSTest/Editor/Styles 폴더 경로를 반환
            if (_stylePathsCache.TryGetValue(markerFileName, out string cachedPath))
            {
                return cachedPath;
            }

            string directory = FindDirectory(markerFileName);
            if (string.IsNullOrEmpty(directory))
            {
                Debug.LogError($"Directory with marker file '{markerFileName}' not found.");
                return null;
            }

            _stylePathsCache[markerFileName] = directory;
            return directory;
        }

        private static string FindDirectory(string markerFileName)
        {
            // find using AssetDatabase
            string[] guids = AssetDatabase.FindAssets(markerFileName);
            if (guids.Length == 0)
            {
                Debug.LogError($"Directory with marker file '{markerFileName}' not found.");
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            string directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
            {
                Debug.LogError($"Directory with marker file '{markerFileName}' not found.");
                return null;
            }

            // Return the directory path
            return directory;
        }

        // 마커 파일은 .txt파일이고, arg로 보낼때는 .txt빼고 파일이름만 보내야함
        // additionalStyles도 markerFileName과 동일한 경로에 있어야함 (나중에 필요시 경로 받는 형태로 확장)
        internal static void InitializeRootElement(VisualElement root, string style, string marker, params string[] additionalStyles)
        {
            if (root == null)
            {
                Debug.LogError("Root VisualElement is null. Cannot apply styles.");
                return;
            }

            root.Clear();

            string basePath = GetBasePath(marker);
            var visualTree = LoadVisualTreeAsset(style, basePath);
            var styleSheet = LoadStyleSheetAsset(style, basePath);

            if (visualTree == null)
            {
                Debug.LogError($"VisualTreeAsset '{style}' not found in path: {basePath}/{style}.uxml");
                return;
            }

            if (styleSheet == null)
            {
                Debug.LogError($"StyleSheet '{style}' not found in path: {basePath}/{style}.uss");
                return;
            }

            visualTree.CloneTree(root);
            root.styleSheets.Add(styleSheet);

            if (additionalStyles != null && additionalStyles.Length > 0)
            {
                foreach (var additionalStyle in additionalStyles)
                {
                    AddStyleSheetToRoot(root, additionalStyle, basePath);
                }
            }

            // add default shared styles
            var coreBasePath = GetBasePath(kCoreMarkerFileName);
            AddStyleSheetToRoot(root, kExStylesStyle, coreBasePath);
            AddStyleSheetToRoot(root, kExColorsStyle, coreBasePath);
        }

        private static void AddStyleSheetToRoot(VisualElement root, string styleName, string basePath)
        {
            var styleSheet = LoadStyleSheetAsset(styleName, basePath);

            if (styleSheet == null)
            {
                Debug.LogError($"StyleSheet '{styleName}' not found in path: {basePath}/{styleName}.uss");
                return;
            }

            root.styleSheets.Add(styleSheet);
        }

        private static StyleSheet LoadStyleSheetAsset(string styleName, string basePath)
        {
            string fileName = $"{styleName}.uss";
            return LoadAssetAtPathINTERNAL<StyleSheet>(fileName, basePath);
        }

        private static VisualTreeAsset LoadVisualTreeAsset(string styleName, string basePath)
        {
            string fileName = $"{styleName}.uxml";
            return LoadAssetAtPathINTERNAL<VisualTreeAsset>(fileName, basePath);
        }

        private static T LoadAssetAtPathINTERNAL<T>(string fileName, string basePath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(basePath))
            {
                Debug.LogError("Base path for styles not found.");
                return null;
            }

            string stylePath = Path.Combine(basePath, fileName);
            return AssetDatabase.LoadAssetAtPath<T>(stylePath);
        }

        internal static void SetupTextArea(TextField textField, float height, EventCallback<ChangeEvent<string>> onInputValueChanged)
        {
            textField.AddManipulator(new BlinkingCursor());
            //textField.verticalScrollerVisibility = ScrollerVisibility.Auto;
            textField.style.position = Position.Absolute;
            textField.multiline = true;
            textField.style.unityTextOverflowPosition = TextOverflowPosition.Start;
            textField.style.top = 0f;
            textField.style.left = 0f;
            textField.style.right = 0f;
            textField.style.bottom = 0f;
            textField.style.overflow = Overflow.Visible;
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.RegisterValueChangedCallback(onInputValueChanged);

            textField.style.height = height;
            textField.parent.style.height = height;
        }
    }
}