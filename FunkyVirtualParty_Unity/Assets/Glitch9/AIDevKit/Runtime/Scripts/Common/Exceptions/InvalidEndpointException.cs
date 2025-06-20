using System;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Thrown when a GenAI provider does not support a specific feature.
    /// </summary>
    public class InvalidEndpointException : NotSupportedException
    {
        /// <summary>
        /// The name of the api provider that does not support the feature.
        /// </summary>
        public Api Api { get; }

        /// <summary>
        /// The name of the unsupported endpoint.
        /// Refer to the <see cref="GENTasks.EndpointType"/> for the list of supported endpoints.
        /// </summary>
        public int EndpointType { get; }

        public InvalidEndpointException(Api api, int endpointType) : base($"{api} does not support {GENTasks.EndpointType.GetName(endpointType)} endpoint.")
        {
            Api = api;
            EndpointType = endpointType;
        }
    }
}