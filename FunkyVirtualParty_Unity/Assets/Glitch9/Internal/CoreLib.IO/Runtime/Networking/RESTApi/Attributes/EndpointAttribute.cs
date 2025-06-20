using System;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Represents an endpoint for a REST API.
    /// This attribute can be applied to fields to specify the endpoint URL and a display name for the Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EndpointAttribute : InspectorNameAttribute
    {
        public string Url { get; protected set; }

        public EndpointAttribute(string displayName, string url) : base(displayName)
        {
            Url = url;
        }
    }
}