using System;

namespace Glitch9
{
    public class EmptyStringException : Exception
    {
        public EmptyStringException(string paramName) : base($"Parameter(string) '{paramName}' cannot be null, empty or whitespace.")
        {
        }
    }
}