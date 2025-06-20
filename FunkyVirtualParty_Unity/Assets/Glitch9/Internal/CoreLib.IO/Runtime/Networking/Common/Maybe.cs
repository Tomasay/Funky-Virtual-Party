using System;
using UnityEngine;

namespace Glitch9.IO.Networking
{
    /// <summary>
    /// Ensures a value is assigned only once. This prevents redundant reassignment attempts 
    /// in scenarios where the value remains null, optimizing performance by eliminating 
    /// unnecessary checks or operations.
    /// </summary>
    public readonly struct Maybe<T>
    {
        public bool IsNothing => !_hasValue;
        private readonly T _value;
        private readonly bool _hasValue;

        public bool HasValue => _hasValue;
        public T Value
        {
            get
            {
                if (!_hasValue) return default;
                return _value;
            }
        }

        public Maybe(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public static implicit operator Maybe<T>(T value) => new(value);
        public static implicit operator Maybe<T>(Nothing nothing) => new();
    }

    /// <summary>
    /// Represents the absence of a value. When utilizing a Maybe<T> construct, null can be assigned 
    /// within Maybe<T> if T is a class. However, if T is a value type (struct), null assignment is not 
    /// permissible; hence this structure is utilized to represent a 'no value' state.
    /// </summary>
    public readonly struct Nothing
    {
        public static readonly Nothing Value = new();
    }

    public static class MaybeExtensions
    {
        public static T GetComponent<T>(ref this Maybe<T>? maybe, MonoBehaviour mono) where T : Component
        {
            if (maybe == null)
            {
                if (!mono.TryGetComponent<T>(out T value))
                {
                    maybe = new Nothing();
                    return null;
                }
                maybe = value;
            }

            if (maybe.HasValue) return maybe.Value.Value;
            return null;
        }

        public static T GetComponentInChildren<T>(ref this Maybe<T>? maybe, MonoBehaviour mono) where T : Component
        {
            if (maybe == null)
            {
                maybe = mono.GetComponentInChildren<T>(true);
                if (maybe == null)
                {
                    maybe = new Nothing();
                    return null;
                }
            }

            if (maybe.HasValue) return maybe.Value.Value;
            return null;
        }

        public static T Get<T>(ref this Maybe<T>? maybe, Func<T> getFunc)
        {
            if (maybe == null)
            {
                maybe = getFunc();
                if (maybe == null)
                {
                    maybe = new Nothing();
                    return default;
                }
            }

            if (maybe.HasValue) return maybe.Value.Value;
            return default;
        }
    }
}