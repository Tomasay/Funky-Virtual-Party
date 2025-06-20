using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public class BoolVariable : Variable
    {
        public static implicit operator bool(BoolVariable suffix) => suffix is { value: true };
        public static implicit operator BoolVariable(bool suffix) => new(suffix);

        [SerializeField] private bool value;

        public bool Value
        {
            get => value;
            set => this.value = value;
        }

        public override object GetValue() => value;
        public override void SetValue(object value)
        {
            if (value is not bool b) throw new ArgumentException("Value must be a bool");
            this.value = b;
        }
        public override Type ValueType => typeof(bool);

        public BoolVariable()
        {
        }

        public BoolVariable(bool value)
        {
            this.value = value;
        }
    }
}