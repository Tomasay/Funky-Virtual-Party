using System.IO;
using Glitch9.Collections;
using Glitch9.ScriptableObjects;
using UnityEngine;

namespace Glitch9.Editor
{
    public static class ScriptableObjectExtensions
    {
        public static void SaveAsset<T>(this T scriptableObject) where T : ScriptableObject
        {
            if (scriptableObject == null) return;
            UnityEditor.EditorUtility.SetDirty(scriptableObject);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
    }

    public static class ScriptableObjectUtil
    {
        internal const string kResources = "Resources";
        internal const string kAssetsResourcesPath = "Assets/Resources";

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

        internal static bool InitialLoad<TData>(Database<TData> db)
            where TData : class, IData, new()
        {
            // if db is empty, run FindAssets to load assets
            // if assets are not found, throw an error
            if (db.IsNullOrEmpty())
            {
                Debug.LogWarning($"The {db.GetType().Name} is empty. Finding assets...");
                FindAssets(db);
            }
            return !db.IsNullOrEmpty();
        }

        internal static void FindAssets<TData>(Database<TData> db)
            where TData : class, IData, new()
        {
            if (!typeof(ScriptableObject).IsAssignableFrom(typeof(TData)))
            {
                Debug.LogError($"The type {typeof(TData).Name} is not a ScriptableObject. Cannot reload assets.");
                return;
            }

            string typeName = typeof(TData).Name;
            Debug.Log($"Reloading {typeName} to {db.GetType().Name}...");

            db.Clear();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject");
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

                if (obj is TData data)
                {
                    Debug.Log($"Found and adding {typeof(TData).Name}: {data.Id}");
                    db.Add(data.Id, data);
                }
            }
        }

        // Version 3.2 (2025.04.11)
        public static TScriptableObject LoadOrCreateScriptableObject<TScriptableObject>(string fileNameWithoutExt, string relativePath)
            where TScriptableObject : ScriptableObject
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

        // public static T LoadSingletonEditor<T>(string dirPath, bool create) where T : ScriptableObject
        // {
        //     if (typeof(T).Name == dirPath) dirPath = kResources;

        //     if (ScriptableObjectLoader.singletonCache.TryGetValue(typeof(T), out var cached))
        //         return (T)cached;

        //     string typeName = typeof(T).Name;

        //     string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");

        //     if (guids.Length > 1)
        //     {
        //         Debug.LogWarning($"Multiple ScriptableObjects of type <color=yellow>{typeName}</color> found. You should only have one ScriptableObject of <color=yellow>{typeName}</color> in your project.");
        //     }

        //     foreach (var guid in guids)
        //     {
        //         string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        //         if (Path.GetFileNameWithoutExtension(path) == typeName)
        //         {
        //             T loaded = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        //             ScriptableObjectLoader.singletonCache[typeof(T)] = loaded;
        //             return loaded;
        //         }
        //     }

        //     if (!create) return null;

        //     Debug.Log($"<color=yellow>{typeName}</color> does not exist in the project. Creating at <color=yellow>{dirPath}</color>");
        //     T created = CreateINTERNAL(dirPath, typeName, ScriptableObject.CreateInstance<T>());
        //     ScriptableObjectLoader.singletonCache[typeof(T)] = created;
        //     return created;
        // }

        // private static T CreateINTERNAL<T>(string dirPath, string fileName, T obj) where T : ScriptableObject
        // {
        //     string path = $"Assets/{dirPath}/{fileName}.asset".FixDoubleAssets().FixSlashes();
        //     string dir = Path.GetDirectoryName(path);
        //     if (!Directory.Exists(dir) && dir != null)
        //     {
        //         Debug.Log($"Creating directory from <color=yellow>{dirPath}</color> => <color=cyan>{dir}</color>");
        //         Directory.CreateDirectory(dir);
        //     }
        //     UnityEditor.AssetDatabase.CreateAsset(obj, path);
        //     UnityEditor.EditorUtility.SetDirty(obj);
        //     UnityEditor.AssetDatabase.Refresh();
        //     return obj;
        // }
    }
}