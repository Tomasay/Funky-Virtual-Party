using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Glitch9.Editor.Collections
{
    public class EPrefsList<T> : IList<T>
    {
        public int Count => Value.Count;
        public bool IsReadOnly => false;
        public bool AllowDuplicates { get; set; } = true;
        public List<T> Value
        {
            get
            {
                InitializeIfNeeded();
                _cache ??= Load();
                return _cache;
            }

            set
            {
                _cache = value;
                Save();
            }
        }

        private List<T> _cache;
        private readonly List<T> _defaultValue;
        private readonly string _prefsKey;
        private bool _initialized;
        private readonly JsonSerializerSettings _settings;

        private void InitializeIfNeeded()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                if (!EditorPrefs.HasKey(_prefsKey))
                {
                    _cache = _defaultValue;
                    Save();
                    return;
                }

                List<T> savedValue = Load();

                if (savedValue == null)
                {
                    _cache = _defaultValue;
                    Save();
                }
                else
                {
                    _cache = savedValue;
                }
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                _cache = _defaultValue;
                Save();
            }
        }

        public EPrefsList(string prefsKey, List<T> defaultValue = null, bool allowDuplicates = true, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;
            _defaultValue = defaultValue ?? new();
            AllowDuplicates = allowDuplicates;
            _settings = settings ?? JsonConfig.DefaultSerializerSettings;
        }

        public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool Contains(T item) => Value.Contains(item);
        public int IndexOf(T item) => Value.IndexOf(item);
        public void CopyTo(T[] array, int arrayIndex) => Value.CopyTo(array, arrayIndex);

        public void Add(T item)
        {
            if (!AllowDuplicates && Value.Contains(item)) return;
            Value.Add(item);
            Save();
        }

        public void Clear()
        {
            Value.Clear();
            Save();
        }

        public bool Remove(T item)
        {
            bool removed = Value.Remove(item);
            if (removed) Save();
            return removed;
        }

        public void Insert(int index, T item)
        {
            Value.Insert(index, item);
            Save();
        }

        public void RemoveAt(int index)
        {
            Value.RemoveAt(index);
            Save();
        }

        public T this[int index]
        {
            get => Value[index];
            set
            {
                Value[index] = value;
                Save();
            }
        }

        private List<T> Load()
        {
            try
            {
                string json = EditorPrefs.GetString(_prefsKey, "[]");
                return JsonConvert.DeserializeObject<List<T>>(json, _settings);
            }
            catch (Exception e)
            {
                EditorPrefsUtil.HandleFailedDeserialization(_prefsKey, typeof(T).Name, e);
                return default;
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(Value, _settings);
            EditorPrefs.SetString(_prefsKey, json);
        }

        public void Replace(IList<T> newList)
        {
            Value.Clear();
            Value.AddRange(newList);
            Save();
        }
    }
}
