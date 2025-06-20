using System;

namespace Glitch9
{
    [Flags]
    public enum LogType
    {
        Info = 1 << 0,
        Verbose = 1 << 1,
        Warning = 1 << 2,
        Error = 1 << 3,
        Exception = 1 << 4,
        NativeInfo = 1 << 5,
        NativeVerbose = 1 << 6,
        NativeWarning = 1 << 7,
        NativeError = 1 << 8,
        NativeCritical = 1 << 9,
    }
}