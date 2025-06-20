using System;

namespace Glitch9.ScriptableObjects
{
    public class ScriptableCreateException : Exception
    {
        public ScriptableCreateException(Type soType, string path)
            : base($"Failed to create ScriptableObject '{soType.Name}' at path '{path}'.")
        {
        }
    }
}