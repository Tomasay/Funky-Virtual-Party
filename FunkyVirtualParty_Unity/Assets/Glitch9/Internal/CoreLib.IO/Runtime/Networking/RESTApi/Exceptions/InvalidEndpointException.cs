using System;

namespace Glitch9.IO.Networking.RESTApi
{
    public class InvalidEndpointException : Exception
    {
        public InvalidEndpointException(object sender, string apiName, HttpMethod method)
            : base($"{apiName} API does not support the {method} request type for the {sender.ToSenderName()} endpoint.")
        {
        }

        public InvalidEndpointException(object sender, Enum apiName, HttpMethod method)
            : base($"{apiName} API does not support the {method} request type for the {sender.ToSenderName()} endpoint.")
        {
        }
    }
}