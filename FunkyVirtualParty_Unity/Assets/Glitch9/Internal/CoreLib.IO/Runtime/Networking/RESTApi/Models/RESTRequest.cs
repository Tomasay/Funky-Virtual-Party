using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;


namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// This class is used to create a http request.
    /// It holds the endpoint, headers, form data and the request options.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the body of the request.
    /// This is used to send a request to the server and receive a response.
    /// The type of the body can be any class that inherits from <see cref="RequestBody"/>.
    /// </typeparam>
    public class RESTRequest<T> : RESTRequest
    {
        public T Body { get; set; }
        public override bool HasBody => true;
        public RESTRequest(string url, T body, RequestOptions options) :
            base(url, options ?? (body is RequestBody reqBody ? reqBody.options : null))
            => Body = body;
    }

    public interface IMultipartFormRequest
    {
    }

    /// <summary>
    /// This class is used to create a http request.
    /// It holds the endpoint, headers, form data and the request options.
    /// It is used to send a request to the server and receive a response.
    /// </summary>
    public class RESTRequest
    {
        /// <summary>
        /// Creates a RESTRequest with the given endpoint.
        /// This is used when you are sending a request with empty body.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static RESTRequest Temp(string url, RequestOptions options = null) => new(url, options);

        /// <summary>
        /// The endpoint of the request.
        /// This is the url of the server where the request will be sent.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The headers of the request.
        /// This is used to send additional information to the server.
        /// The headers can be used to set the content type, authorization, etc.
        /// </summary>
        public List<RESTHeader> Headers { get; protected set; } = new();

        /// <summary>
        /// Holds a reference to a <see cref="UnityWebRequest"/> object.
        /// This is used to send the request to the server and receive a response.
        /// </summary>
        public UnityWebRequest WebRequest { get; set; }

        /// <summary>
        /// Optional. The form data of the request.
        /// This is used to send data to the server in a form format.
        /// </summary>
        public WWWForm Form { get; set; }

        /// <summary>
        /// Indicates if the request has a body or not.
        /// This is used to determine if the request should be sent with a body or not.
        /// </summary>
        public virtual bool HasBody => false;

        // --- options ------------------------------------------------
        protected readonly RequestOptions _options;

        /// <summary>
        /// The unique identifier of the request. 
        /// </summary>
        public string Id => _options.Id;

        /// <summary>
        /// Optional. The local sender of the request.
        /// </summary>
        public string Sender => _options.Sender;

        /// <summary>
        /// Optional. If this is set, the response data will be saved to this path as a file.
        /// Supported formats:
        /// - TODO: Add supported formats
        /// </summary>
        public string OutputPath => _options.OutputPath;

        /// <summary>
        /// Optional. External cancellation token for the request.
        /// This is used to cancel the request from outside the request.
        /// </summary>
        public CancellationToken? Token { get => _options.Token; set => _options.Token = value; }

        /// <summary>
        /// The content type of the request. Default is Json
        /// </summary>
        public virtual MIMEType MIMEType => _options.MIMEType;

        /// <summary>
        /// The number of retries of the request. Default is 3
        /// </summary>
        public int MaxRetry => _options.MaxRetry;

        /// <summary>
        /// Seconds of delay to make a retry. Default is 1
        /// </summary>
        public float RetryDelayInSec => _options.RetryDelayInSec;

        /// <summary>
        /// The custom timeout for the request.
        /// </summary>
        public TimeSpan? Timeout { get => _options.Timeout; set => _options.Timeout = value; }

        /// <summary>
        /// If this is set, 
        /// it means you are requesting a stream response from the server. 
        /// </summary>
        public IStreamHandler StreamHandler => _options.StreamHandler;

        /// <summary>
        /// The audio format of the expected audio response. Default is null.
        /// </summary>
        public AudioFormat OutputAudioFormat => _options.OutputAudioFormat;

        /// <summary>
        /// Returns true if the request is a stream request.
        /// </summary>
        public bool IsStreamRequest => StreamHandler != null;

        /// <summary>
        /// Indicates if the request should be logged or not.
        /// </summary>
        public bool IgnoreLogs => _options.IgnoreLogs;


        public RESTRequest() { _options = new(); }
        public RESTRequest(string url, RequestOptions options = null)
        {
            Endpoint = url;
            _options = options ?? new();
        }

        internal IEnumerable<RESTHeader> GetHeaders(bool includeContentTypeHeader)
        {
            if (includeContentTypeHeader) yield return MIMEType.GetHeader();
            foreach (RESTHeader header in Headers) yield return header;
        }

        /// <summary>
        /// Adds a header to the request. 
        /// </summary> 
        public void AddHeader(RESTHeader header) => Headers.Add(header);

        /// <summary>
        /// Cancels this request (only if a cancellation token is set).
        /// This will throw a <see cref="OperationCanceledException"/> if the request is cancelled.
        /// </summary>
        public void CancelRequest()
        {
            if (Token == null)
            {
                Debug.LogError("Cancellation token is not set. Cannot cancel the request.");
                return;
            }

            if (Token.Value.IsCancellationRequested) return;
            Token.Value.ThrowIfCancellationRequested();
            Token = null;
            Debug.Log($"Request cancelled: {this}");
        }

        #region Equality Members
        public static bool operator ==(RESTRequest left, RESTRequest right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Id == right.Id;
        }

        public static bool operator !=(RESTRequest left, RESTRequest right)
        {
            return !(left == right);
        }

        protected bool Equals(RESTRequest other)
        {
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RESTRequest)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
        #endregion 
    }
}