using System.IO;
using UnityEngine;

namespace Glitch9.Editor
{
    internal static class DirectoryFinder
    {
        // marker files are '.txt' files that are used to identify the directory
        internal static string FindDirectory(string markerFileName)
        {
            // find using AssetDatabase
            string[] guids = UnityEditor.AssetDatabase.FindAssets(markerFileName);
            if (guids.Length == 0)
            {
                Debug.LogError($"Directory with marker file '{markerFileName}' not found.");
                return null;
            }

            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            string directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
            {
                Debug.LogError($"Directory with marker file '{markerFileName}' not found.");
                return null;
            }

            // Return the directory path
            return directory;
        }
    }
}