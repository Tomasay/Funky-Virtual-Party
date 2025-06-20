using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public class Vector3Variable : Variable
    {
        public static implicit operator Vector3(Vector3Variable variable) => variable?.value ?? Vector3.zero;
        public static implicit operator Vector3Variable(Vector3 value) => new(value);

        [SerializeField] private Vector3 value;

        public Vector3 Value
        {
            get => value;
            set => this.value = value;
        }

        public override object GetValue() => value;

        public override void SetValue(object value)
        {
            if (value is not Vector3 v) throw new ArgumentException("Value must be a Vector3");
            this.value = v;
        }
        public override Type ValueType => typeof(Vector3);

        public Vector3Variable()
        {
        }

        public Vector3Variable(Vector3 value)
        {
            this.value = value;
        }
    }
}