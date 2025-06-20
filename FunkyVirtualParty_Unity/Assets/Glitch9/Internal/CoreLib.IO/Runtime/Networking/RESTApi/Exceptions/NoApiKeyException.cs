using System;

namespace Glitch9.IO.Networking.RESTApi
{
    public class NoApiKeyException : Exception
    {
        public NoApiKeyException(string apiName) : base($"{apiName} API requires an API key to be set.") { }
        public NoApiKeyException(Enum apiEnum) : base($"{apiEnum.GetInspectorName()} API requires an API key to be set.") { }
    }

    public class NoApiKeyQueryKeyException : Exception
    {
        public NoApiKeyQueryKeyException(string apiName) : base($"{apiName} API requires a query key for your API key.") { }
        public NoApiKeyQueryKeyException(Enum apiEnum) : base($"{apiEnum.GetInspectorName()} API requires a query key for your API key.") { }
    }
}