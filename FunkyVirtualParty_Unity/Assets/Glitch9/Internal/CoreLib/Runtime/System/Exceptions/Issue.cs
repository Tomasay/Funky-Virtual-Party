using System;

namespace Glitch9
{
    public class Issue : Exception
    {
        public ExceptionType Type { get; set; }

        public Issue(ExceptionType type, string message = null) : base($"{type.GetMessage()}: {message ?? string.Empty}")
        {
            Type = type;
        }
    }
}