using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal abstract class Catalogue<TSelf, TEntry, TInterface>
        where TSelf : Catalogue<TSelf, TEntry, TInterface>, new()
        where TEntry : class, TInterface, ICatalogueEntry
        where TInterface : class, IData
    {
        internal static TSelf Instance => _instance ??= new TSelf();
        private static TSelf _instance;
        internal List<TEntry> Entries { get; set; }
        private readonly string _entryName;
        private readonly string _entryNamePlural;

        protected Catalogue()
        {
            string filePath = GetCataloguePath();

            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                Entries = JsonConvert.DeserializeObject<List<TEntry>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Converters = new List<JsonConverter>
                    {
                        new ModalityStringListConverter(),
                    },
                });
            }

            Entries ??= new();

            if (this is ModelCatalogue)
            {
                _entryName = "Model";
                _entryNamePlural = "models";
            }
            else if (this is VoiceCatalogue)
            {
                _entryName = "Voice";
                _entryNamePlural = "voices";
            }
            else
            {
                throw new Exception($"Unknown AssetCatalogue type: {typeof(TSelf).Name}");
            }
        }

        //internal abstract UniTask CheckForUpdatesAsync();
        internal TEntry GetEntry(string id) => Entries.FirstOrDefault(x => x.Id == id);
        internal bool HasEntry(string id) => Entries.Any(x => x.Id == id);
        protected abstract string GetCataloguePath();
        protected abstract UniTask<List<TInterface>> RetrieveAllEntriesAsync(bool isAutoUpdate = false);
        internal abstract TEntry CreateEntry(TInterface softRef);
        protected abstract List<TEntry> GetMissingAPIEntries();
        protected abstract void ShowCatalogueUpdateWindow(List<TInterface> newEntries, List<TEntry> deprecatedEntries);

        internal async UniTask CheckForUpdatesAsync(bool debugMode, bool isAutoUpdate)
        {
            List<TInterface> retrievedEntries = await RetrieveAllEntriesAsync(isAutoUpdate);

            if (retrievedEntries.Count == 0)
            {
                Debug.LogError($"Critical Error: No {_entryNamePlural} found. Please check your API keys and settings.");
                return;
            }

            List<TEntry> missingApiEntries = GetMissingAPIEntries(); // These are the models that are available, but API doesn't return them

            if (missingApiEntries != null && missingApiEntries.Count > 0)
            {
                foreach (var entry in missingApiEntries)
                {
                    // check if the model is already in the catalogue
                    if (retrievedEntries.All(x => x.Id != entry.Id))
                    {
                        retrievedEntries.Add(entry);
                        Debug.Log($"Found missing {_entryName.ToLower()} (missing form the response): {entry.Name} ({entry.Id})");
                    }
                    else
                    {
                        Debug.Log($"{_entryName} already exists in the catalogue: {entry.Name} ({entry.Id})");
                    }
                }
            }

            var retrievedIds = new HashSet<string>(retrievedEntries.Select(x => x.Id));
            var existingIds = new HashSet<string>(Entries.Select(x => x.Id));

            List<TInterface> newEntries = retrievedEntries
                .Where(x => !existingIds.Contains(x.Id))
                .ToList();

            List<TEntry> deprecatedEntries = Entries
                .Where(x => (debugMode || !x.IsDeprecated) && !retrievedIds.Contains(x.Id))
                .ToList();

            if (newEntries.Count > 0 || deprecatedEntries.Count > 0)
            {
                AddOrUpdateEntries(newEntries, true);

                ShowCatalogueUpdateWindow(newEntries, deprecatedEntries);

                if (deprecatedEntries.Count > 0)
                {
                    DeprecateEntries(deprecatedEntries);
                    if (AIDevKitEditorSettings.AutoDeleteDeprecatedModels)
                    {
                        Debug.Log($"Auto-deleting {deprecatedEntries.Count} deprecated {_entryNamePlural}.");
                        RemoveDeprecatedEntries();
                    }
                }
            }
        }

        public async void UpdateCatalogue(Action<bool> onComplete)
        {
            try
            {
                await UpdateCatalogueAsync();
                onComplete?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating catalogue: {e.Message}");
                onComplete?.Invoke(false);
            }
        }

        public async UniTask UpdateCatalogueAsync()
        {
            Debug.Log($"Updating {_entryName} Catalogue...");

            List<TInterface> allEntries = await RetrieveAllEntriesAsync();

            if (allEntries.Count == 0)
            {
                Debug.LogError($"Critical Error: No {_entryNamePlural} found. Please check your API keys and settings.");
                return;
            }

            Debug.Log($"Found {allEntries.Count} {_entryNamePlural}.");

            AddOrUpdateEntries(allEntries);
        }

        protected void AddOrUpdateEntries(List<TInterface> assets, bool overwrite = false)
        {
            foreach (TInterface asset in assets)
            {
                if (asset == null)
                {
                    Debug.LogError($"Critical Error: {asset} is null.");
                    continue;
                }

                if (HasEntry(asset.Id))
                {
                    if (overwrite)
                    {
                        TEntry entry = CreateEntry(asset);
                        // replace the existing entry
                        int index = Entries.FindIndex(x => x.Id == asset.Id);
                        if (index >= 0)
                        {
                            Entries[index] = entry;
                            Debug.Log($"Updated {_entryName}: {entry.Name} ({entry.Id})");
                        }
                        else
                        {
                            Debug.LogError($"Critical Error: Could not find entry with ID {asset.Id} to update.");
                        }
                    }
                }
                else
                {
                    TEntry entry = CreateEntry(asset);
                    Entries.Add(entry);
                }
            }

            Save();
        }

        protected void DeprecateEntries(List<TEntry> entries)
        {
            foreach (TEntry data in entries) DeprecateEntry(data);
            Save();
        }

        internal void DeprecateEntry(TEntry entry)
        {
            if (entry == null)
            {
                Debug.LogError($"Critical Error: {entry} is null.");
                return;
            }

            if (HasEntry(entry.Id))
            {
                TEntry existingEntry = GetEntry(entry.Id);
                existingEntry.IsDeprecated = true;
                Debug.Log($"Deprecated {_entryName}: {existingEntry.Name} ({existingEntry.Id})");
                Save();
            }
            else
            {
                Debug.LogError($"Critical Error: Could not find entry with ID {entry.Id} to deprecate.");
            }
        }

        internal void RemoveDeprecatedEntries()
        {
            List<TEntry> deprecatedEntries = Entries.Where(x => x.IsDeprecated).ToList();
            foreach (TEntry entry in deprecatedEntries)
            {
                Entries.Remove(entry);
                Debug.Log($"Removed deprecated {_entryName}: {entry.Name} ({entry.Id})");
            }
            Save();
        }

        internal void RemoveEntries(params string[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                Debug.LogError("Critical Error: No IDs provided to remove entries.");
                return;
            }

            List<TEntry> entriesToRemove = Entries.Where(x => ids.Contains(x.Id)).ToList();

            if (entriesToRemove.Count == 0)
            {
                Debug.LogError($"Critical Error: No entries found with the provided IDs: {string.Join(", ", ids)}");
                return;
            }

            foreach (TEntry entry in entriesToRemove)
            {
                Entries.Remove(entry);
                Debug.Log($"Removed {_entryName}: {entry.Name} ({entry.Id})");
            }

            Save();
        }

        internal void RemoveEntry(TEntry entry)
        {
            if (entry == null)
            {
                Debug.LogError($"Critical Error: {entry} is null.");
                return;
            }

            if (HasEntry(entry.Id))
            {
                Entries.Remove(entry);
                Debug.Log($"Removed {_entryName}: {entry.Name} ({entry.Id})");
                Save();
            }
            else
            {
                Debug.LogError($"Critical Error: Could not find entry with ID {entry.Id} to remove.");
            }
        }

        internal void RemoveDuplicates()
        {
            List<TEntry> duplicates = Entries.GroupBy(x => x.Id).Where(g => g.Count() > 1).SelectMany(g => g.Skip(1)).ToList();
            foreach (TEntry duplicate in duplicates)
            {
                Entries.Remove(duplicate);
                Debug.Log($"Removed duplicate {_entryName}: {duplicate.Name} ({duplicate.Id})");
            }
            Save();
        }

        protected void Save()
        {
            string filePath = GetCataloguePath();
            string json = JsonConvert.SerializeObject(Entries, Formatting.Indented);
            string directory = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(directory)) System.IO.Directory.CreateDirectory(directory);
            System.IO.File.WriteAllText(filePath, json);
        }

        internal void UpdateSingleEntry(TEntry entry)
        {
            if (entry == null)
            {
                Debug.LogError($"Critical Error: {entry} is null.");
                return;
            }

            if (HasEntry(entry.Id))
            {
                int index = Entries.FindIndex(x => x.Id == entry.Id);
                if (index >= 0)
                {
                    Entries[index] = entry;
                    Debug.Log($"Updated {_entryName}: {entry.Name} ({entry.Id})");
                }
                else
                {
                    Debug.LogError($"Critical Error: Could not find entry with ID {entry.Id} to update.");
                }
            }
            else
            {
                Entries.Add(entry);
                Debug.Log($"Added new {_entryName}: {entry.Name} ({entry.Id})");
            }

            Save();
        }

        internal Dictionary<Api, List<string>> GetAllEntryIds()
        {
            Dictionary<Api, List<string>> allEntryIds = new();

            foreach (TEntry entry in Entries)
            {
                if (!allEntryIds.ContainsKey(entry.Api))
                {
                    allEntryIds[entry.Api] = new List<string>();
                }
                allEntryIds[entry.Api].Add(entry.Id);
            }

            return allEntryIds;
        }
    }
}