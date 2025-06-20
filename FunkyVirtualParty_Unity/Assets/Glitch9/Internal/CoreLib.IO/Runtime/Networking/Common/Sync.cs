using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Glitch9.IO.Networking
{
    public interface ISyncStorage
    {
        UniTask<object> GetDataAsync(string fieldName);
        void SetDataAsync(string fieldName, object value);
    }

    public sealed class Sync<T>
    {
        public static implicit operator T(Sync<T> sync) => sync.Value;

        private T _value;
        private readonly object _lock = new();
        private readonly ISyncStorage _syncStorage;
        private readonly string _fieldName;

        public Sync() { }

        public Sync(ISyncStorage syncStorage, string fieldName)
        {
            _syncStorage = syncStorage ?? throw new ArgumentNullException(nameof(syncStorage));
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        public Sync(ISyncStorage syncStorage, string fieldName, T value)
        {
            _syncStorage = syncStorage ?? throw new ArgumentNullException(nameof(syncStorage));
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            _value = value;
        }

        public T Value
        {
            get => GetValueAsync();
            set => SetValueAsync(value);
        }

        private T GetValueAsync()
        {
            lock (_lock)
            {
                if (_value != null) return _value;

                if (typeof(T) == typeof(string))
                {
                    _value = (T)(object)string.Empty;
                }
                else if (Nullable.GetUnderlyingType(typeof(T)) != null || !typeof(T).IsValueType)
                {
                    _value = Activator.CreateInstance<T>();
                }
                else
                {
                    _value = default;
                }

                return _value;
            }
        }

        private void SetValueAsync(T newValue)
        {
            lock (_lock)
            {
                if (_syncStorage == null ||
                    EqualityComparer<T>.Default.Equals(_value, newValue)) return;

                _value = newValue;

                if (_value == null) return;

                object converted = RESTConvert.ToCloudFormat(typeof(T), _value);
                if (converted == null) return;
                _syncStorage.SetDataAsync(_fieldName, converted);
            }
        }
    }
}
