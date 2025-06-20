using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class EditorPathUtil
    {
        private static Dictionary<string, string> _folderCache = new();
        internal static string FindGlitch9Path() => FindFolder("Glitch9");
        internal static string FindFolder(params string[] folderNameCandidates)
        {
            if (folderNameCandidates == null || folderNameCandidates.Length == 0) return null;
            string cacheKey = string.Join("|", folderNameCandidates);
            if (_folderCache.TryGetValue(cacheKey, out var cachedPath)) return cachedPath;

            foreach (var candidate in folderNameCandidates)
            {
                if (string.IsNullOrEmpty(candidate)) continue;
                var guids = AssetDatabase.FindAssets($"{candidate} t:folder");
                foreach (var guid in guids)
                {
                    var folderPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(folderPath) && Path.GetFileName(folderPath) == candidate)
                    {
                        _folderCache[cacheKey] = folderPath;
                        return folderPath;
                    }
                }
            }

            Debug.LogError($"None of the folder candidates found: {string.Join(", ", folderNameCandidates)}");
            return "Assets/Glitch9";
        }
    }
}
