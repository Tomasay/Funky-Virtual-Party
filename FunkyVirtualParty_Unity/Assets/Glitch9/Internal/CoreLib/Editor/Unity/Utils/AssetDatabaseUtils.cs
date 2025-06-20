using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Glitch9.Editor
{
    /// <summary>
    /// Utility methods for working with the Unity AssetDatabase.
    /// Includes methods for finding and pinging script files.
    /// </summary>
    public static class AssetDatabaseUtils
    {
        public static bool TryDeleteAsset<T>(T asset) where T : UnityEngine.Object
        {
            if (asset == null) return false;

            try
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete asset: {e.Message}");
                return false;
            }
        }

        public static void PingScriptFile(Type type)
        {
            // Find all MonoScript objects in the project
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            Debug.Log($"Found {guids.Length} MonoScripts in the project.");

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && script.GetClass() == type)
                {
                    Debug.Log($"Found {type.Name} script at {assetPath}");
                    // Ping the script in the Unity Editor
                    EditorGUIUtility.PingObject(script);
                    Selection.activeObject = script;
                    break;
                }
            }
        }

        public static void PingDirectory(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath)) return;

            if (Directory.Exists(dirPath))
            {
                string guid = AssetDatabase.AssetPathToGUID(dirPath);
                if (!string.IsNullOrEmpty(guid))
                {
                    AssetDatabase.GUIDToAssetPath(guid);
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dirPath));
                }
            }
        }

        public static void PingScriptableObject(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            string guid = AssetDatabase.AssetPathToGUID(path);
            if (!string.IsNullOrEmpty(guid))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (obj != null)
                {
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                }
            }
        }

        public static void SafeCreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (path.StartsWith("Assets/")) path = path[7..];

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }

        public static void SafeDeleteDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            string metaFile = path.EndsWith("/") || path.EndsWith("\\")
                ? path.TrimEnd('/', '\\') + ".meta"
                : path + ".meta";

            if (File.Exists(metaFile))
            {
                File.Delete(metaFile);
            }

            AssetDatabase.Refresh();
        }

        public static void RefreshIfEditor()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
