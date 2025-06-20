using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.Collections
{
    public class PrefsDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;

        private readonly string _prefsKey;
        private readonly Dictionary<TKey, TValue> _dictionary = new();
        public string DictionaryName
        {
            get
            {
                if (_dictionaryName == null)
                {
                    string keyName = typeof(TKey).Name;
                    string valueName = typeof(TValue).Name;
                    _dictionaryName = $"PrefsDictionary<{keyName}, {valueName}>";
                }
                return _dictionaryName;
            }
        }
        private string _dictionaryName;

        public PrefsDictionary(string prefsKey)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;

            try
            {
                Dictionary<TKey, TValue> savedValue = Load();
                if (savedValue != null) _dictionary = savedValue;
            }
            catch (Exception e)
            {
                LogService.Exception(e);
            }
        }

        public PrefsDictionary(string prefsKey, Dictionary<TKey, TValue> defaultValue)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;

            if (!PlayerPrefs.HasKey(_prefsKey))
            {
                _dictionary = defaultValue;
                Save();
                return;
            }

            try
            {
                Dictionary<TKey, TValue> savedValue = Load();
                _dictionary = savedValue ?? defaultValue;
                if (_dictionary == null || savedValue == null)
                {
                    _dictionary = defaultValue;
                    Save();
                }
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                _dictionary = defaultValue;
                Save();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
            Save();
        }

        public void Clear()
        {
            _dictionary.Clear();
            Save();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(_dictionary[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                _dictionary.Remove(item.Key);
                Save();
                return true;
            }
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            Save();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            bool removed = _dictionary.Remove(key);
            if (removed)
            {
                Save();
            }
            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                Save();
            }
        }

        private Dictionary<TKey, TValue> Load()
        {
            try
            {
                string json = PlayerPrefs.GetString(_prefsKey, "{}");
                return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json, JsonConfig.DefaultSerializerSettings);
            }
            catch (Exception e)
            {
                PrefsUtil.HandleFailedDeserialization(_prefsKey, DictionaryName, e);
                return default;
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(_dictionary, JsonConfig.DefaultSerializerSettings);
            PlayerPrefs.SetString(_prefsKey, json);
            PlayerPrefs.Save();
        }
    }
}
