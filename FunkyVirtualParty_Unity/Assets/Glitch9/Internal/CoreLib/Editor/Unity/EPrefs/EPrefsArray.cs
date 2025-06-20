using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Glitch9.Editor.Collections
{
    public class EPrefsArray<T> : IList<T>
    {
        public int Count => _array.Length;
        public bool IsReadOnly => false;

        private readonly string _prefsKey;
        private T[] _array;
        public string ListName
        {
            get
            {
                if (_listName == null)
                {
                    string itemName = typeof(T).Name;
                    _listName = $"EPrefsArray<{itemName}>";
                }
                return _listName;
            }
        }
        private string _listName;

        private readonly T[] defaultValue;
        private bool _initialized;

        private void InitializeIfNeeded()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                if (!EditorPrefs.HasKey(_prefsKey))
                {
                    //Debug.LogWarning($"Key not found 101: {_prefsKey}. Using default value: {defaultValue}");
                    _array = defaultValue;
                    Save();
                    return;
                }

                T[] savedValue = Load();
                _array = savedValue ?? defaultValue; // 로드된 값이 null이면 defaultValue 사용
                if (_array == null || savedValue == null) // 로드된 값이 null이거나 첫 로드에서 값을 얻지 못한 경우
                {
                    //Debug.LogWarning($"Key not found 102: {_prefsKey}. Using default value: {defaultValue}");
                    _array = defaultValue;
                    Save();
                }
            }
            catch (Exception e)
            {
                LogService.Exception(e);
                _array = defaultValue; // 예외 발생 시 defaultValue를 사용
                Save();
            }
        }

        public EPrefsArray(string prefsKey)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;
            this.defaultValue = Array.Empty<T>();
        }

        public EPrefsArray(string prefsKey, T[] defaultValue)
        {
            if (string.IsNullOrEmpty(prefsKey)) throw new ArgumentException("prefsKey cannot be null or empty");
            _prefsKey = prefsKey;
            this.defaultValue = defaultValue;
        }

        public static implicit operator T[](EPrefsArray<T> prefsArray) => prefsArray._array;

        public IEnumerator<T> GetEnumerator()
        {
            InitializeIfNeeded();
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            InitializeIfNeeded();
            return GetEnumerator();
        }

        public void Add(T item)
        {
            // throw new NotSupportedException("Add is not supported on EPrefsArray. Arrays have fixed size.");
            List<T> list = new(_array)
            {
                item
            };
            _array = list.ToArray();
            Save();
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
            Save();
        }

        public bool Contains(T item)
        {
            return Array.Exists(_array, element => EqualityComparer<T>.Default.Equals(element, item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_array, 0, array, arrayIndex, _array.Length);
        }

        public bool Remove(T item)
        {
            //throw new NotSupportedException("Remove is not supported on EPrefsArray. Arrays have fixed size.");
            List<T> list = new(_array);
            bool removed = list.Remove(item);
            _array = list.ToArray();
            Save();
            return removed;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_array, item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Insert is not supported on EPrefsArray. Arrays have fixed size.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("RemoveAt is not supported on EPrefsArray. Arrays have fixed size.");
        }

        public void Replace(T[] newArray)
        {
            if (newArray == null) throw new ArgumentNullException(nameof(newArray));

            if (newArray.Length != _array.Length)
            {
                Array.Resize(ref _array, newArray.Length);
            }

            Array.Copy(newArray, _array, Math.Min(newArray.Length, _array.Length));
            Save();
        }


        public T this[int index]
        {
            get
            {
                InitializeIfNeeded();
                return _array[index];
            }
            set
            {
                _array[index] = value;
                Save();
            }
        }

        private T[] Load()
        {
            try
            {
                string json = EditorPrefs.GetString(_prefsKey, "[]");
                return JsonConvert.DeserializeObject<T[]>(json, JsonConfig.DefaultSerializerSettings);
            }
            catch (Exception e)
            {
                EditorPrefsUtil.HandleFailedDeserialization(_prefsKey, ListName, e);
                return default;
            }
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(_array, JsonConfig.DefaultSerializerSettings);
            EditorPrefs.SetString(_prefsKey, json);
        }
    }
}
