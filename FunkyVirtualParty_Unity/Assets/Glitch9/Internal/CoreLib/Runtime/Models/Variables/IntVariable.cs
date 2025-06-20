using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public class IntVariable : Variable
    {
        public static implicit operator int(IntVariable suffix) => suffix?.value ?? 0;
        public static implicit operator IntVariable(int suffix) => new(suffix);

        [SerializeField] private int value;

        public int Value
        {
            get => value;
            set => this.value = value;
        }

        public override object GetValue() => value;
        public override void SetValue(object value)
        {
            if (value is not int i) throw new ArgumentException("Value must be an int");
            this.value = i;
        }
        public override Type ValueType => typeof(int);

        public IntVariable()
        {
        }

        public IntVariable(int value)
        {
            this.value = value;
        }

    }
}