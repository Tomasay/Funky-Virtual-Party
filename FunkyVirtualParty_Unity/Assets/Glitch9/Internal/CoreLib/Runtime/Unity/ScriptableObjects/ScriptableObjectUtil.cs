using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Glitch9.ScriptableObjects
{
    public static class ScriptableObjectExtensions
    {
        public static void SaveAsset<T>(this T scriptableObject) where T : ScriptableObject
        {
#if UNITY_EDITOR
            if (scriptableObject == null) return;
            UnityEditor.EditorUtility.SetDirty(scriptableObject);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }

    public static class ScriptableObjectUtil
    {
        // Do not use this path when using Resources.Load
        // Only use this path when using AssetDatabase.LoadAssetAtPath
        private const string kAssetsResourcesPath = "Assets/Resources";
        private const string kResources = "Resources";
        private static Dictionary<System.Type, ScriptableObject> _singletonCache = new();

        internal static string FixSOName(string name)
        {
            // check if name has invalid characters like '/', '\', ':', '*', '?', '"', '<', '>', '|'
            // use a regex to replace them with '_'
            return System.Text.RegularExpressions.Regex.Replace(name, @"[\/\\:\*\?""<>|]", "_");
        }

#if UNITY_EDITOR

        // Version 3.2 (2025.04.11)
        public static TScriptableObject FindOrCreateSO<TScriptableObject>(
            string fileNameWithoutExt,
            string relativePath
        ) where TScriptableObject : ScriptableObject
        {
            // 1. Try load from Resources first (without extension) -----------------------------------
            TScriptableObject res = Resources.Load<TScriptableObject>(fileNameWithoutExt);
            if (res != null) return res;

            string absolutePath = relativePath.ToFullPath();
            if (!Directory.Exists(absolutePath)) Directory.CreateDirectory(absolutePath);

            // check if the file already exists, if it does, it means the file is broken somehow.
            // create a backup by changing the filename to {filename}_{bk}.asset

            string filePath = $"{absolutePath}/{fileNameWithoutExt}.asset".FixSlashes();
            string assetPath = $"{relativePath}/{fileNameWithoutExt}.asset".FixSlashes().FixDoubleAssets();

            // 2. Check if file exists physically but is broken ---------------------------------------
            if (File.Exists(filePath))
            {
                TScriptableObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TScriptableObject>(assetPath);

                if (asset == null)
                {
                    // Backup the broken file
                    string backupPath = $"{absolutePath}/{fileNameWithoutExt}_bk.asset".FixSlashes();
                    File.Move(filePath, backupPath);
                    Debug.LogWarning($"Existing scriptable object asset was corrupted and moved to <color=yellow>{backupPath}</color>");
                }
                else
                {
                    return asset;
                }
            }

            // 3. Create new instance
            TScriptableObject obj = ScriptableObject.CreateInstance<TScriptableObject>();
            UnityEditor.AssetDatabase.CreateAsset(obj, assetPath);

            // 4. Verify creation success
            TScriptableObject created = UnityEditor.AssetDatabase.LoadAssetAtPath<TScriptableObject>(assetPath)
                ?? throw new ScriptableCreateException(typeof(TScriptableObject), assetPath);

            Debug.Log($"{typeof(TScriptableObject).Name} created at <color=yellow>{assetPath}</color>");

            UnityEditor.EditorUtility.SetDirty(created);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            return obj;
        }

        public static TScriptableObject CreateSOInResources<TScriptableObject>(string objectName = null, string customPath = null) where TScriptableObject : ScriptableObject
        {
            if (string.IsNullOrEmpty(objectName)) objectName = typeof(TScriptableObject).Name;
            TScriptableObject created = ScriptableObject.CreateInstance<TScriptableObject>()
                ?? throw new ScriptableCreateException(typeof(TScriptableObject), kAssetsResourcesPath);

            string resourcesPath = customPath ?? kAssetsResourcesPath;
            UnityEditor.AssetDatabase.CreateAsset(created, $"{resourcesPath}/{objectName}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            return created;
        }

#endif

        public static T LoadSingleton<T>(string dirPath, bool create = false) where T : ScriptableObject
        {
            if (typeof(T).Name == dirPath) dirPath = kResources;

#if UNITY_EDITOR
            return LoadSingletonEditor<T>(dirPath, create);
#else
            return LoadSingletonRuntime<T>(null);
#endif
        }

#if UNITY_EDITOR
        private static T LoadSingletonEditor<T>(string dirPath, bool create) where T : ScriptableObject
        {
            if (_singletonCache.TryGetValue(typeof(T), out var cached))
                return (T)cached;

            string typeName = typeof(T).Name;

            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");

            if (guids.Length > 1)
            {
                Debug.LogWarning($"Multiple ScriptableObjects of type <color=yellow>{typeName}</color> found. You should only have one ScriptableObject of <color=yellow>{typeName}</color> in your project.");
            }

            foreach (var guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == typeName)
                {
                    T loaded = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    _singletonCache[typeof(T)] = loaded;
                    return loaded;
                }
            }

            if (!create) return null;

            Debug.Log($"<color=yellow>{typeName}</color> does not exist in the project. Creating at <color=yellow>{dirPath}</color>");
            T created = Create(dirPath, typeName, ScriptableObject.CreateInstance<T>());
            _singletonCache[typeof(T)] = created;
            return created;
        }

        private static T Create<T>(string dirPath, string fileName, T obj) where T : ScriptableObject
        {
            string path = $"Assets/{dirPath}/{fileName}.asset".FixDoubleAssets().FixSlashes();
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir) && dir != null)
            {
                Debug.Log($"Creating directory from <color=yellow>{dirPath}</color> => <color=cyan>{dir}</color>");
                Directory.CreateDirectory(dir);
            }
            UnityEditor.AssetDatabase.CreateAsset(obj, path);
            UnityEditor.EditorUtility.SetDirty(obj);
            UnityEditor.AssetDatabase.Refresh();
            return obj;
        }
#else
        private static T LoadSingletonRuntime<T>(string path) where T : ScriptableObject
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
#endif


#if UNITY_EDITOR 

        // only works in editor
        public static T[] LoadAll<T>(string folder) where T : ScriptableObject
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });
            T[] assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return assets;
        }
#endif
    }
}
