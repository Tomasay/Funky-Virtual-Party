using System;
using UnityEngine;

namespace Glitch9.Editor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LabelAttribute : PropertyAttribute
    {
        public string Name { get; private set; }

        public LabelAttribute(string name)
        {
            Name = name;
        }
    }
}
