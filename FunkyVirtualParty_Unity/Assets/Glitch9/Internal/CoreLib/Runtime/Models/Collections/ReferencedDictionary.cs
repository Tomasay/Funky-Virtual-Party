using System;
using System.Collections;
using System.Collections.Generic;

namespace Glitch9.Collections
{
    [Serializable]
    public class ReferencedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        public List<SerializedKeyValuePair<TKey, TValue>> serializedList = new();

        private Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                if (_dictionary == null) Deserialize();
                return _dictionary;
            }
        }
        private Dictionary<TKey, TValue> _dictionary;
        public int Count => Dictionary.Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).IsReadOnly;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Dictionary.Add(item.Key, item.Value);
            Serialize();
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (KeyValuePair<TKey, TValue> item in items)
            {
                Dictionary.Add(item.Key, item.Value);
            }
            Serialize();
        }

        public void Clear()
        {
            Dictionary.Clear();
            Serialize();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(Dictionary[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(Dictionary[item.Key], item.Value))
            {
                bool removed = Dictionary.Remove(item.Key);
                Serialize();
                return removed;
            }

            return false;
        }

        public bool RemoveAll(Predicate<KeyValuePair<TKey, TValue>> match)
        {
            List<TKey> keysToRemove = new();
            foreach (KeyValuePair<TKey, TValue> pair in Dictionary)
            {
                if (match(pair))
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (TKey key in keysToRemove)
            {
                Dictionary.Remove(key);
            }

            if (keysToRemove.Count > 0)
            {
                Serialize();
                return true;
            }

            return false;
        }

        // FindAll
        public List<TValue> FindAll(Predicate<TValue> match)
        {
            List<TValue> values = new();
            foreach (KeyValuePair<TKey, TValue> pair in Dictionary)
            {
                if (match(pair.Value))
                {
                    values.Add(pair.Value);
                }
            }

            return values;
        }

        public void Add(TKey key, TValue value)
        {
            Dictionary.AddOrUpdate(key, value);
            Serialize();
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            bool removed = Dictionary.Remove(key);
            if (removed)
            {
                Serialize();
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set
            {
                Dictionary[key] = value;
                Serialize();
            }
        }

        public ICollection<TKey> Keys => Dictionary.Keys;
        public ICollection<TValue> Values => Dictionary.Values;

        private void Serialize()
        {
            serializedList.Clear();
            foreach (KeyValuePair<TKey, TValue> kvp in Dictionary)
            {
                serializedList.Add(new SerializedKeyValuePair<TKey, TValue> { key = kvp.Key, value = kvp.Value });
            }
        }

        private void Deserialize()
        {
            if (serializedList == null)
            {
                serializedList = new List<SerializedKeyValuePair<TKey, TValue>>();
                return;
            }

            _dictionary = new();
            foreach (SerializedKeyValuePair<TKey, TValue> kvp in serializedList)
            {
                _dictionary[kvp.key] = kvp.value;
            }
        }
    }
}