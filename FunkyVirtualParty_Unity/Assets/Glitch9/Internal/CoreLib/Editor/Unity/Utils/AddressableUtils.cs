#if UNITY_ADDRESSABLES 
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace Glitch9.Editor
{
    public static class AddressableUtils
    {
        public static void OpenAddressableGroups()
        {
            EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
        }

        public static AddressableAssetGroup SafeGetGroup(string groupName, AddressableAssetSettings settings)
        {
            if (settings == null)
            {
                if (ShowDialog.Confirm("Addressable settings not found. Do you want to create a new Addressable settings?"))
                {
                    settings = AddressableAssetSettingsDefaultObject.Settings;

                    if (settings == null)
                    {
                        settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
                        AddressableAssetSettingsDefaultObject.Settings = settings;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if (settings == null)
                    {
                        ShowDialog.Error("Failed to create Addressable settings asset.");
                        return null;
                    }
                    else
                    {
                        Debug.Log("Addressable settings asset created successfully.");
                    }
                }
                else
                {
                    return null;
                }
            }

            if (string.IsNullOrEmpty(groupName))
            {
                ShowDialog.Error("Addressable Group name is empty. Please enter a name for the group.");
                return null;
            }

            AddressableAssetGroup group = settings.FindGroup(groupName);

            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));

                if (group == null)
                {
                    ShowDialog.Error($"Failed to create Addressable group '{groupName}'.");
                    return null;
                }
                else
                {
                    Debug.Log($"Addressable group '{groupName}' created successfully.");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            return group;
        }

        public static void AddAsset(string groupName, string labelName, string assetPath, bool createGroup = true)
        {
            // Get the Addressable Asset Settings which is the entry point to the Addressable Asset System.
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // Check if the group already exists.
            AddressableAssetGroup group = SafeGetGroup(groupName, settings);

            // Add assets to the group in the Addressable Asset Settings.
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            entry.address = fileName;
            entry.SetLabel(labelName, true, true, true);

            Debug.Log($"Asset '{assetPath}' added to addressable group '{groupName}'.");
        }

        public static void RemoveAssetEntry(string address, bool refresh = false)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            foreach (AddressableAssetGroup group in settings.groups)
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    if (entry.address == address)
                    {
                        group.RemoveAssetEntry(entry);
                        Debug.Log($"Asset '{address}' removed from addressable group '{group.Name}'.");
                    }
                }
            }

            if (refresh)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }


        public static bool TryAddAddressableAsset(string groupName, string address, UnityEngine.Object @object, params string[] labels)
        {
            // 1. 기본 세팅
            string assetPath = AssetDatabase.GetAssetPath(@object);
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = SafeGetGroup(groupName, settings);
            if (group == null) return false;

            // 2. 먼저 entry를 group에 넣음 (혹시 다른 group에 있다면 이동됨)
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGuid, group);
            if (entry == null) return false;

            // 3. 주소 충돌 검사: 동일한 주소 가진 entry가 있다면 (entry 자신이 아닌 경우) 제거
            var duplicate = group.entries
                .FirstOrDefault(e => e != entry && e.address == address);

            if (duplicate != null)
            {
                group.RemoveAssetEntry(duplicate);
            }

            // 4. 정상 등록 
            foreach (string label in labels)
            {
                if (settings.GetLabels().All(l => l != label))
                {
                    settings.AddLabel(label);
                }

                entry.SetLabel(label, true);
            }

            entry.address = address;

            //Debug.Log($"{@object.GetType()} '{address}' added to group '{groupName}' as '{address}'.");

            return true;
        }
    }
}
#endif
