using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    public class RESTResponse<T> : RESTResponse
    {
        public T Body { get; set; }
        public override bool HasBody => true;

        /// <summary>
        /// Creates a new instance of the <see cref="RESTResponse"/> class with the success flag set to true.
        /// This method is used to quickly create a successful result.
        /// </summary>
        /// <returns>A new instance of <see cref="RESTResponse"/> with <see cref="Result.IsSuccess"/> set to true.</returns>
        public static RESTResponse<T> StreamDoneT() => new() { IsSuccess = true, IsStreamDone = true };
    }

    /// <summary>
    /// The base class for all API objects returned by <see cref="RESTApiV5"/>.
    /// This class encapsulates common properties and methods for handling 
    /// various types of outputs from REST API responses.
    /// </summary>
    public class RESTResponse : Result
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// This ID is used to uniquely identify the API object.
        /// </summary>
        [JsonProperty("id")] public virtual string Id { get; set; }

        /// <summary>
        /// The HTTP status code of the API response.
        /// </summary>
        [JsonIgnore] public long StatusCode { get; set; } = 200;

        /// <summary>
        /// Returns true if it's a result of SSE event and the event is done.
        /// </summary>
        [JsonIgnore] public bool IsStreamDone { get; set; } = false;

        /// <summary>
        /// Whether the API response has an empty body.
        /// </summary>
        [JsonIgnore] public virtual bool HasBody => false;

        /// <summary>
        /// Text data output from the API
        /// </summary>
        [JsonIgnore] public virtual string TextOutput { get; set; }

        /// <summary>
        /// Binary data output from the API
        /// </summary>
        [JsonIgnore] public virtual byte[] BinaryOutput { get; set; }

        /// <summary>
        /// Image data output from the API as a Texture2D object
        /// </summary>
        [JsonIgnore] public virtual Texture2D ImageOutput { get; set; }

        /// <summary>
        /// Audio data output from the API as an AudioClip object
        /// </summary>
        [JsonIgnore] public virtual AudioClip AudioOutput { get; set; }

        /// <summary>
        /// File output from the API as a UnityFile object
        /// </summary>
        [JsonIgnore] public virtual RawFile FileOutput { get; set; }

        /// <summary>
        /// The absolute path where the output file is saved
        /// </summary>
        [JsonIgnore] public string OutputPath { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="RESTResponse"/> class with the success flag set to true.
        /// This method is used to quickly create a successful result.
        /// </summary>
        /// <returns>A new instance of <see cref="RESTResponse"/> with <see cref="Result.IsSuccess"/> set to true.</returns>
        public static RESTResponse StreamDone() => new() { IsSuccess = true, IsStreamDone = true };
    }
}
