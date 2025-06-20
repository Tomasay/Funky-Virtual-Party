using System.Linq;
using Glitch9.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class VoiceCatalogueUtil
    {
        internal static bool AddToLibrary(VoiceCatalogueTreeViewItem item)
        {
            if (item == null) return false;

            VoiceCatalogueEntry entry = item.Data;
            if (entry == null) return false;

            Voice modelData = AddToLibrary(entry);
            if (modelData == null) throw new System.Exception($"Failed to add {entry.Api} {typeof(Voice).Name} to {typeof(VoiceLibrary).Name}.");

            item.InMyLibrary = true;
            return true;
        }

        internal static bool RemoveFromLibrary(VoiceCatalogueTreeViewItem item)
        {
            if (item == null) return false;

            VoiceCatalogueEntry entry = item.Data;
            if (entry == null) return false;

            item.InMyLibrary = !RemoveFromLibrary(entry.Api, entry.Id);

            return !item.InMyLibrary;
        }

        internal static void UpdateData(Voice voice, VoiceCatalogueEntry serverData)
        {
            if (voice == null) throw new System.Exception($"{typeof(Voice).Name} inside {typeof(Voice).Name} is null.");

            voice.UpdateData(
                api: serverData.Api,
                id: serverData.Id,
                name: serverData.Name,
                custom: serverData.IsCustom,
                gender: serverData.Gender,
                age: serverData.Age,
                language: serverData.Language
            );

            EditorUtility.SetDirty(voice);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void AddToLibrary(string id)
        {
            VoiceCatalogueEntry serverData = VoiceCatalogue.Instance.GetEntry(id)
                ?? throw new System.Exception($"Failed to retrieve {id} {typeof(Voice).Name} from {typeof(VoiceCatalogue).Name}.");

            AddToLibrary(serverData);
        }

        private static Voice AddToLibrary(VoiceCatalogueEntry serverData)
        {
            Api api = serverData.Api;
            string id = serverData.Id;

            if (string.IsNullOrWhiteSpace(id)) throw new System.Exception($"{typeof(Voice).Name} ID is null or empty.");

            string displayName = serverData.Name;
            string scriptableObjectName;

            if (api == Api.ElevenLabs)
            {
                scriptableObjectName = displayName.ToSnakeCase();
            }
            else
            {
                scriptableObjectName = id.ToSnakeCase();
            }

            string internalResourcesPath = AIDevKitEditorPath.GetInternalResourcesPath();

            string targetDir = $"{internalResourcesPath}/Voices";

            System.IO.Directory.CreateDirectory(targetDir);
            Voice obj = ScriptableObject.CreateInstance<Voice>();
            UpdateData(obj, serverData);

            scriptableObjectName = ScriptableObjectUtil.FixSOName(scriptableObjectName);

            string filePath = $"{targetDir}/{scriptableObjectName}.asset";
            Debug.Log($"Creating [{typeof(Voice).Name}] Scriptable Object: " + filePath);

            AssetDatabase.CreateAsset(obj, filePath);
            EditorUtility.SetDirty(obj);
            VoiceLibrary.Add(obj);
            Debug.Log($"Adding {obj} to {typeof(VoiceLibrary).Name}...");

            VoicePopupGUI.ForceUpdateCache();

            return obj;
        }

        internal static bool RemoveFromLibrary(Api api, string id)
        {
            if (api == Api.None || api == Api.All || string.IsNullOrWhiteSpace(id)) return false;

            if (AIDevKitConfig.SystemDefaultVoices.Contains(id))
            {
                Debug.LogWarning($"Cannot remove {api} {typeof(Model).Name} from {typeof(VoiceLibrary).Name}. This is a default model.");
                return false;
            }

            RemoveFromLibrary(id);

            return true;
        }

        internal static void RemoveFromLibrary(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new System.Exception($"{typeof(Model).Name} ID is null or empty.");

            Voice voice = VoiceLibrary.Get(id);
            if (voice == null) throw new System.Exception($"Failed to remove {id} voice from {typeof(VoiceLibrary).Name}.");

            VoiceLibrary.Remove(voice);

            // VoiceData is a Scriptable Object, so we need to delete the file itself.
            string path = AssetDatabase.GetAssetPath(voice);
            if (!string.IsNullOrWhiteSpace(path)) AssetDatabase.DeleteAsset(path);
        }
    }
}