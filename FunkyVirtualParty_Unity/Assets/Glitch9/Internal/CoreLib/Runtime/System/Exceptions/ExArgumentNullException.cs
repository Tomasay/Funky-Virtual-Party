using System;
using System.Runtime.CompilerServices;

namespace Glitch9
{
    public class ExArgumentNullException : ArgumentNullException
    {
        public ExArgumentNullException(string paramName, [CallerMemberName] string callerMemberName = "") : base(paramName,
            $"Argument '{paramName}' cannot be null in method '{callerMemberName}'.")
        {
        }
    }
}