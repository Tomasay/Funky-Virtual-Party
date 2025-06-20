using System;
using System.IO;
using UnityEngine;

namespace Glitch9
{
    internal class PrefsUtil
    {
        internal static void HandleFailedDeserialization(string prefsKey, string paramName, Exception e)
        {
            string json = PlayerPrefs.GetString(prefsKey, string.Empty);
            Debug.LogError($"Error occurred while deserializing {paramName}: {e.Message}");
            Debug.LogError($"Failed JSON: {json}");

            if (!string.IsNullOrWhiteSpace(json))
            {
                // Create backup to a file
                string backupPath = Path.Combine(Application.persistentDataPath, $"Prefs_{prefsKey}.json");
                Debug.LogError($"Creating JSON backup at: {backupPath}");
                File.WriteAllText(backupPath, json);
            }

            PlayerPrefs.DeleteKey(prefsKey);
        }
    }
}