using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public abstract class Variable
    { 
        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract Type ValueType { get; }

        public static Type GetVariableTypeFor(Type type)
        {
            if (type == typeof(float)) return typeof(FloatVariable);
            if (type == typeof(int)) return typeof(IntVariable);
            if (type == typeof(bool)) return typeof(BoolVariable);
            if (type == typeof(string)) return typeof(StringVariable);
            if (type == typeof(Vector2)) return typeof(Vector2Variable);
            if (type == typeof(Vector3)) return typeof(Vector3Variable);
            return null;
        }
    }
}