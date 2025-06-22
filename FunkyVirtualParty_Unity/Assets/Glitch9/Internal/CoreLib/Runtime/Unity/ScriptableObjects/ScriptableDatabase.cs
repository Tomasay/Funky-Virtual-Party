using System.Collections.Generic;
using System.Linq;
using Glitch9.Collections;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.ScriptableObjects
{
    public abstract class ScriptableDatabase<TDb, TData, TSelf> : ScriptableResource<TSelf>
        where TDb : Database<TData>, new()
        where TData : class, IData, new()
        where TSelf : ScriptableDatabase<TDb, TData, TSelf>
    {
        [SerializeField, SerializeReference] private TDb data = new();
        public TDb Data => data;

        public static TDb DB => Instance.data ??= new TDb();
        public static int Count => DB.Count;
        public static bool IsEmpty => DB.IsNullOrEmpty();

        public static TData Get(string id)
        {
            if (LogIfNull()) return null;
            return DB.TryGetValue(id, out TData data) ? data : null;
        }

        public static bool TryGetValue(string id, out TData data)
        {
            if (LogIfNull())
            {
                data = null;
                return false;
            }
            return DB.TryGetValue(id, out data);
        }

        public static bool Contains(string id)
        {
            if (LogIfNull()) return false;
            return DB.ContainsKey(id);
        }

        public static void Add(TData data)
        {
            if (LogIfNull()) return;
            DB.Add(data.Id, data);
        }

        public static bool Remove(TData data)
        {
            if (LogIfNull()) return false;
            DB.Remove(data.Id);
            return true;
        }

        public static bool Remove(string id)
        {
            if (LogIfNull()) return false;
            DB.Remove(id);
            return true;
        }

        public static bool RemoveAt(int index)
        {
            if (LogIfNull() || index < 0 || index >= DB.Count) return false;
            var key = DB.Keys.ElementAt(index);
            DB.Remove(key);
            return true;
        }

        public static void Clear()
        {
            if (LogIfNull()) return;
            DB.Clear();
        }

        public static List<TData> ToList() => DB.Values.ToList();
        public static IEnumerable<TData> ToEnumerable() => DB.Values.AsEnumerable();

        public static void RemoveInvalidEntries()
        {
            Debug.Log($"Removing invalid entries from {typeof(TDb).Name}...");
            if (LogIfNull()) return;
            DB.RemoveAll(kvp => IsNullOrMissing(kvp.Value));
        }

        /// <summary>
        /// Checks whether the given UnityEngine.Object is either null or missing (destroyed).
        /// </summary>
        public static bool IsNullOrMissing(TData obj)
        {
            if (obj is UnityEngine.Object unityObj) return unityObj == null || unityObj.Equals(null);
            return obj == null;
        }

        public static async void BackupToJsonFile(string path)
        {
            if (LogIfNull()) return;
            if (DB.Count == 0) return;
            string jsonString = JsonConvert.SerializeObject(DB, JsonConfig.DefaultSerializerSettings);
            if (string.IsNullOrEmpty(jsonString)) return;
            await System.IO.File.WriteAllTextAsync(path, jsonString);
        }

        public static async void RestoreFromJsonFile(string path)
        {
            if (LogIfNull()) return;
            string jsonString = await System.IO.File.ReadAllTextAsync(path);
            Database<TData> data = JsonConvert.DeserializeObject<Database<TData>>(jsonString, JsonConfig.DefaultSerializerSettings);
            // add logs to cache, don't replace
            if (!data.IsNullOrEmpty()) DB.AddRange(data);
        }

        protected static bool LogIfNull()
        {
            if (DB == null)
            {
                string dataName = typeof(TData).Name;
                string repoName = Instance.GetType().Name;
                Debug.LogError($"There was an error while trying to access {dataName} list in ScriptableObject - {repoName}. Check the ScriptableObject file for errors.");
                return true;
            }
            return false;
        }
    }
}