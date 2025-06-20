using UnityEditor;
using UnityEngine;
using System.IO;
namespace Glitch9.AIDevKit.Editor.Pro
{
    public static class MonoScriptUtil
    {
        public static string GetScriptSourceText(MonoScript monoScript)
        {
            if (monoScript == null)
                return "// MonoScript is null.";

            // Get the asset path from MonoScript
            string path = AssetDatabase.GetAssetPath(monoScript);
            if (string.IsNullOrEmpty(path))
                return "// Failed to get path.";

            // Read the source code from file
            try
            {
                return System.IO.File.ReadAllText(path);
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read script at path: {path}\n{e.Message}");
                return $"// Failed to read script: {e.Message}";
            }
        }
    }
}