using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public class FloatVariable : Variable
    {
        public static implicit operator float(FloatVariable suffix) => suffix?.value ?? 0;
        public static implicit operator FloatVariable(float suffix) => new(suffix);
        
        [SerializeField] private float value;
        
        public float Value
        {
            get => value;
            set => this.value = value;
        }

        public override object GetValue() => value;
        public override void SetValue(object value)
        {
            if (value is not float f) throw new ArgumentException("Value must be a float");
            this.value = f;
        }
        public override Type ValueType => typeof(float);

        public FloatVariable()
        {
        }

        public FloatVariable(float value)
        {
            this.value = value;
        }
    }
}