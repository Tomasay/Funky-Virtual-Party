using System;

namespace Glitch9
{
    public class LogListener
    {
        public Action<string> OnInfo;
        public Action<string> OnWarning;
        public Action<string> OnError;
    }
}