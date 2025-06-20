using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public class Vector2Variable : Variable
    {
        public static implicit operator Vector2(Vector2Variable variable) => variable?.value ?? Vector2.zero;
        public static implicit operator Vector2Variable(Vector2 value) => new(value);
        
        [SerializeField] private Vector2 value;

        public Vector2 Value
        {
            get => value;
            set => this.value = value;
        }

        public override object GetValue() => value;

        public override void SetValue(object value)
        {
            if (value is not Vector2 v) throw new ArgumentException("Value must be a Vector2");
            this.value = v;
        }
        public override Type ValueType => typeof(Vector2);

        public Vector2Variable()
        {
        }

        public Vector2Variable(Vector2 value)
        {
            this.value = value;
        }
    }
}