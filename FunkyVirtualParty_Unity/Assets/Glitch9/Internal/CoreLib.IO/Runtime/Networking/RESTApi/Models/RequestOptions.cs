using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using System;
using System.Threading;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Options for <see cref="RESTRequest"/> and <see cref="RESTRequest{T}"/>.
    /// This class is used to set the parameters for the http request.
    /// </summary>
    public class RequestOptions
    {
        public static RequestOptions MultipartFormData => new()
        {
            MIMEType = MIMEType.MultipartForm,
        };

        private const int kDefaultRetryCount = 3;
        private const int kDefaultRetryDelayInSeconds = 1;

        /// <summary>
        /// The identifier of the request.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The local sender of the request.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// The directory path where the file will be downloaded.
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Optional. External cancellation token for the request.
        /// </summary>
        public CancellationToken? Token { get; set; }

        /// <summary>
        /// Optional. The content type of the request. Default is Json
        /// </summary>
        public virtual MIMEType MIMEType { get; set; } = MIMEType.Json;

        /// <summary>
        /// Optional. The audio format of the request. D
        /// Default is null.
        /// </summary>
        public AudioFormat OutputAudioFormat { get; set; }

        /// <summary>
        /// The number of retries of the request. Default is 3
        /// </summary>
        public int MaxRetry { get; set; } = kDefaultRetryCount;

        /// <summary>
        /// Seconds of delay to make a retry. Default is 1
        /// </summary>
        public float RetryDelayInSec { get; set; } = kDefaultRetryDelayInSeconds;

        /// <summary>
        /// The custom timeout for the request.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this is set, 
        /// it means you are requesting a stream response from the server. 
        /// </summary>
        public IStreamHandler StreamHandler { get; set; }

        /// <summary>
        /// If this is set, the request will not log any information.
        /// </summary>
        public bool IgnoreLogs { get; set; } = false;
    }
}