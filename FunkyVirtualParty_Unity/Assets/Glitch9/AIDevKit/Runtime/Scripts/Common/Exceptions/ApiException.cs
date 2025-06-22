using System;

namespace Glitch9.AIDevKit
{
    public class ApiException : Exception
    {
        public Api Api { get; }
        public string Url { get; }

        public ApiException(Api api, string url, Exception exception) : base(AIDevKitDebug.ExtractErrorMessage(exception), exception)
        {
            Api = api;
            Url = url;
        }
    }
}