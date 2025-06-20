using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.Collections
{
    public class PrefsList<T> : IList<T>
    {
        public int Count => _list.Count;
        public bool IsReadOnly => false;

        private readonly string _prefsKey;
        private readonly List<T> _list = new();
        public string ListName
        {
            get
            {
                if (_listName == null)
                {
                    string itemName = typeof(T).Name;
                    _listName = $"PrefsList<{itemName}>";
                }
                return _listName;
            }
        }
        private string _listName;

        public PrefsList(string prefsKey)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;

            try
            {
                List<T> savedValue = Load();
                if (savedValue != null) _list = savedValue;
            }
            catch (Exception e)
            {
                LogService.Exception(e);
            }
        }

        public PrefsList(string prefsKey, List<T> defaultValue)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;

            if (!PlayerPrefs.HasKey(_prefsKey))
            {
                _list = defaultValue;
                Save();
                return;
            }

            try
            {
                List<T> savedValue = Load();
                _list = savedValue ?? defaultValue;
                if (_list == null || savedValue == null)
                {
                    _list = defaultValue;
                    Save();
                }
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                _list = defaultValue;
                Save();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _list.Add(item);
            Save();
        }

        public void Clear()
        {
            _list.Clear();
            Save();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool removed = _list.Remove(item);
            if (removed)
            {
                Save();
            }
            return removed;
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            Save();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            Save();
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                Save();
            }
        }

        private List<T> Load()
        {
            try
            {
                string json = PlayerPrefs.GetString(_prefsKey, "[]");
                return JsonConvert.DeserializeObject<List<T>>(json, JsonConfig.DefaultSerializerSettings);
            }
            catch (Exception e)
            {
                PrefsUtil.HandleFailedDeserialization(_prefsKey, ListName, e);
                return default;
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(_list, JsonConfig.DefaultSerializerSettings);
            PlayerPrefs.SetString(_prefsKey, json);
            PlayerPrefs.Save();
        }

        public List<T> ToList()
        {
            return new List<T>(_list);
        }
    }
}
