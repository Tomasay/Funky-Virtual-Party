using System;

namespace Glitch9.IO.Networking.RESTApi
{
    [Flags]
    public enum HttpMethod
    {
        /// <summary>
        /// This should not be used.
        /// </summary>
        Unset = 0,

        /// <summary>
        /// The POST method is used to submit an entity to the specified resource, often causing a change in state or side effects on the server.
        /// </summary>
        POST = 1 << 0,

        /// <summary>
        /// The GET method requests a representation of the specified resource. Requests using GET should only retrieve data.
        /// </summary>
        GET = 1 << 1,

        /// <summary>
        /// The PUT method replaces all current representations of the target resource with the request payload.
        /// </summary>
        PUT = 1 << 2,

        /// <summary>
        /// The DELETE method deletes the specified resource.
        /// </summary>
        DELETE = 1 << 3,

        /// <summary>
        /// The PATCH method is used to apply partial modifications to a resource.
        /// </summary>
        PATCH = 1 << 4,

        /// <summary>
        /// The HEAD method asks for a response identical to that of a GET request, but without the response body.
        /// </summary>
        HEAD = 1 << 5,
    }
}