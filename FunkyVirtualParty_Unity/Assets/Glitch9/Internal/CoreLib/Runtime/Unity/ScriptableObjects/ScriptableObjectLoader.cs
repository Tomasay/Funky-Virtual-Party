using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Glitch9.ScriptableObjects
{
    public static class ScriptableObjectLoader
    {
        // Do not use this path when using Resources.Load
        // Only use this path when using AssetDatabase.LoadAssetAtPath 
        private const string kResources = "Resources";
        private static Dictionary<System.Type, ScriptableObject> _singletonCache = new();

        internal static string FixSOName(string name)
        {
            // check if name has invalid characters like '/', '\', ':', '*', '?', '"', '<', '>', '|'
            // use a regex to replace them with '_'
            return System.Text.RegularExpressions.Regex.Replace(name, @"[\/\\:\*\?""<>|]", "_");
        }

        public static T LoadSingleton<T>(string dirPath, bool create = false) where T : ScriptableObject
        {
            if (typeof(T).Name == dirPath) dirPath = kResources;
            // #if UNITY_EDITOR
            //             return EditorImpl.LoadSingletonEditor<T>(dirPath, create);
            // #else
            return LoadSingletonRuntime<T>(null);
            //#endif
        }

        internal static T LoadSingletonRuntime<T>(string path) where T : ScriptableObject
        {
            // if (string.IsNullOrEmpty(path))
            // {
            //     return Resources.Load<T>(typeof(T).Name);
            // }

            // if (Path.HasExtension(path)) // if path contains extension, remove it
            // {
            //     string fileName = Path.GetFileNameWithoutExtension(path);
            //     string dir = Path.GetDirectoryName(path);
            //     path = $"{dir}/{fileName}";
            // }

            // T asset = Resources.Load<T>(path);
            // if (asset == null) Debug.LogWarning($"{path} does not exist in the Resources folder.");
            // return asset;

            T[] all = Resources.LoadAll<T>("");
            if (all == null || all.Length == 0)
            {
                Debug.LogWarning($"No ScriptableObject of type {typeof(T).Name} found in Resources.");
                return null;
            }

            if (all.Length > 1)
            {
                Debug.LogWarning($"Multiple ScriptableObjects of type {typeof(T).Name} found in Resources. Using the first one.");
            }

            return all[0];
        }

        // #if UNITY_EDITOR
        //         private static class EditorImpl
        //         {
        //             internal static T LoadSingletonEditor<T>(string dirPath, bool create) where T : ScriptableObject
        //             {
        //                 if (_singletonCache.TryGetValue(typeof(T), out var cached))
        //                     return (T)cached;

        //                 string typeName = typeof(T).Name;
        //                 string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");

        //                 foreach (var guid in guids)
        //                 {
        //                     string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        //                     if (Path.GetFileNameWithoutExtension(path) == typeName)
        //                     {
        //                         T loaded = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        //                         _singletonCache[typeof(T)] = loaded;
        //                         return loaded;
        //                     }
        //                 }

        //                 if (!create) return null;

        //                 T created = ScriptableObject.CreateInstance<T>();
        //                 string assetPath = $"Assets/{dirPath}/{typeName}.asset".Replace("\\", "/");

        //                 string dir = Path.GetDirectoryName(assetPath);
        //                 if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        //                 UnityEditor.AssetDatabase.CreateAsset(created, assetPath);
        //                 UnityEditor.EditorUtility.SetDirty(created);
        //                 UnityEditor.AssetDatabase.SaveAssets();
        //                 UnityEditor.AssetDatabase.Refresh();

        //                 _singletonCache[typeof(T)] = created;
        //                 return created;
        //             }
        //         }
        // #endif
    }
}
