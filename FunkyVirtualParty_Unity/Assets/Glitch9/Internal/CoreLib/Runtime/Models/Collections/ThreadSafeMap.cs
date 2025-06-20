using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Glitch9.Collections
{
    public class ThreadSafeMap<TKey, TValue> 
        where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, TValue> _concurrentStore;
        private readonly Func<TKey, TValue> _defaultFactory;

        public ThreadSafeMap(Func<TKey, TValue> defaultFactory)
        {
            ThrowIf.ArgumentIsNull(defaultFactory, nameof(defaultFactory));
            _defaultFactory = defaultFactory;
            _concurrentStore = new ConcurrentDictionary<TKey, TValue>();
        }

        public TValue Get(TKey key)
        {
            return _concurrentStore.GetOrAdd(key, _defaultFactory);
        }

        public Dictionary<TKey, TValue> GetDictionary()
        {
            return new Dictionary<TKey, TValue>(_concurrentStore);
        }
    }
}