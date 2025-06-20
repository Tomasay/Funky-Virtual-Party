using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class EditorPrefsUtil
    {
        internal static void HandleFailedDeserialization(string prefsKey, string paramName, Exception e)
        {
            string json = EditorPrefs.GetString(prefsKey, string.Empty);
            Debug.LogError($"Error occurred while deserializing {paramName}: {e.Message}");
            Debug.LogError($"Failed JSON: {json}");

            if (!string.IsNullOrWhiteSpace(json))
            {
                // Create backup to a file
                string backupPath = Path.Combine(Application.persistentDataPath, $"EPrefs_{prefsKey}.json");
                Debug.LogError($"Creating JSON backup at: {backupPath}");
                File.WriteAllText(backupPath, json);
            }

            PlayerPrefs.DeleteKey(prefsKey);
        }

        internal static uint GetUInt(string prefsKey, uint defaultValue = 0)
        {
            if (!EditorPrefs.HasKey(prefsKey)) return defaultValue;
            string str = EditorPrefs.GetString(prefsKey, string.Empty);
            return uint.TryParse(str, out var result) ? result : defaultValue;
        }

        internal static void SetUInt(string prefsKey, uint value)
        {
            string str = value.ToString();
            EditorPrefs.SetString(prefsKey, str);
        }

        internal static long GetLong(string prefsKey, long defaultValue = 0)
        {
            if (!EditorPrefs.HasKey(prefsKey)) return defaultValue;
            string str = EditorPrefs.GetString(prefsKey, string.Empty);
            return long.TryParse(str, out var result) ? result : defaultValue;
        }

        internal static void SetLong(string prefsKey, long value)
        {
            string str = value.ToString();
            EditorPrefs.SetString(prefsKey, str);
        }
    }
}