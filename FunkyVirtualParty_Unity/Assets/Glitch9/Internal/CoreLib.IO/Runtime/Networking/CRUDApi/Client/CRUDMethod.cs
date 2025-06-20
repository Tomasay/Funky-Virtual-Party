using System;
using System.Collections.Generic;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// CRUD is abbreviation for Create, Read, Update, Delete.
    /// It is used to define the operations that can be performed on a REST resource.
    /// </summary>
    [Flags]
    public enum CRUDMethod
    {
        Unknown = 0,

        /// <summary>
        /// Creates REST resource using <seealso cref="HttpMethod.POST"/>.
        /// </summary>
        Create = 1 << 0,

        /// <summary>
        /// Gets REST resource using 
        /// <seealso cref="HttpMethod.GET"/> or sometimes
        /// <seealso cref="HttpMethod.POST"/>.
        /// </summary>
        Retrieve = 1 << 1,

        /// <summary>
        /// Updates REST resource using 
        /// <seealso cref="HttpMethod.PATCH"/>,
        /// <seealso cref="HttpMethod.POST"/> or sometimes
        /// <seealso cref="HttpMethod.PUT"/>. 
        /// </summary>
        Update = 1 << 2,

        /// <summary>
        /// Deletes REST resource using 
        /// <seealso cref="HttpMethod.DELETE"/> or sometimes
        /// <seealso cref="HttpMethod.POST"/>.
        /// </summary>
        Delete = 1 << 3,

        /// <summary>
        /// Queries a list of REST resources using 
        /// <seealso cref="HttpMethod.GET"/>.
        /// </summary>
        Query = 1 << 4,
    }

    internal static class CRUDMethodExtensions
    {
        private static readonly Dictionary<CRUDMethod, string> _messages = new()
        {
            { CRUDMethod.Create, "Creating" },
            { CRUDMethod.Update, "Updating" },
            { CRUDMethod.Retrieve, "Retrieving" },
            { CRUDMethod.Delete, "Deleting" },
            { CRUDMethod.Query, "Querying" }
        };

        public static string GetMessage(this CRUDMethod method)
        {
            if (_messages.TryGetValue(method, out var message))
            {
                return message;
            }

            return "Unknown CRUD operation executing";
        }
    }
}