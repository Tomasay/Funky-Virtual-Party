using System;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Thrown when a GenAI provider does not support a specific feature.
    /// </summary>
    public class NotSupportedEndpointException : NotSupportedException
    {
        /// <summary>
        /// The name of the api provider that does not support the feature.
        /// </summary>
        public Api Api { get; }

        /// <summary>
        /// The name of the unsupported endpoint.
        /// Refer to the <see cref="GENTasks.RequestType"/> for the list of supported endpoints.
        /// </summary>
        public string RequestType { get; }

        public NotSupportedEndpointException(Api api, string requestType) : base($"{api} does not support {GENTasks.RequestType.GetDisplayName(requestType)} endpoint.")
        {
            Api = api;
            RequestType = requestType;
        }
    }
}