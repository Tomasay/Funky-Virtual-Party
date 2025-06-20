using System;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Associates an enum with an external API name and an optional key for parsing substring matches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ApiEnumAttribute : InspectorNameAttribute
    {
        /// <summary>
        /// The name of the API that this enum value corresponds to.
        /// This is used for serialization and deserialization of enum values in API requests and responses.
        /// </summary>
        public string ApiName { get; protected set; }

        /// <summary>
        /// An optional key used to parse the enum value from a string.
        /// This is useful when the API returns values that are substrings of the enum names.
        /// </summary>
        public string ParseKey { get; protected set; }

        public ApiEnumAttribute(string apiName) : base(apiName) => ApiName = apiName;
        public ApiEnumAttribute(string displayName, string apiName) : base(displayName) => ApiName = apiName;
        public ApiEnumAttribute(string displayName, string apiName, string parseKey) : base(displayName)
        {
            ApiName = apiName;
            ParseKey = parseKey;
        }
    }
}