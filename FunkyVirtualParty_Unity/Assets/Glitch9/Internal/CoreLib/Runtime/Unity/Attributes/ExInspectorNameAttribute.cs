using System;
using UnityEngine;

namespace Glitch9.Editor
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ExInspectorNameAttribute : InspectorNameAttribute
    {
        public readonly string shortDisplayName;
        public ExInspectorNameAttribute(string displayName, string shortDisplayName) : base(displayName)
        {
            this.shortDisplayName = shortDisplayName;
        }
    }
}