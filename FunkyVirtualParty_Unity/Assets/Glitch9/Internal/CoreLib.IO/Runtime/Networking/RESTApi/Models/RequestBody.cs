using Glitch9.IO.Files;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Glitch9.IO.Networking.RESTApi
{
    public class RequestBody
    {
        [JsonIgnore] public RequestOptions options = new();
        [JsonIgnore] public string Sender { get => options.Sender; set => options.Sender = value; }
        [JsonIgnore] public string OutputPath { get => options.OutputPath; set => options.OutputPath = value; }
        [JsonIgnore] public CancellationToken? Token { get => options.Token; set => options.Token = value; }
        [JsonIgnore] public MIMEType MIMEType { get => options.MIMEType; set => options.MIMEType = value; }
        [JsonIgnore] public IStreamHandler StreamHandler { get => options.StreamHandler; set => options.StreamHandler = value; }
        [JsonIgnore] public int MaxRetry { get => options.MaxRetry; set => options.MaxRetry = value; }
        [JsonIgnore] public float RetryDelayInSec { get => options.RetryDelayInSec; set => options.RetryDelayInSec = value; }
        [JsonIgnore] public TimeSpan? Timeout { get => options.Timeout; set => options.Timeout = value; }
        [JsonIgnore] public bool IgnoreLogs { get => options.IgnoreLogs; set => options.IgnoreLogs = value; }

        // Added 2025.03.30
        /// <summary>
        /// Validate the request. 
        /// This is used to check if all required properties are set.
        /// Fix the request if needed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual void ValidateBody() { }

        public abstract class RequestBodyBuilder<TBuilder, TReqBody>
            where TBuilder : RequestBodyBuilder<TBuilder, TReqBody>
            where TReqBody : RequestBody
        {
            internal TReqBody _req;
            public RequestBodyBuilder() => _req = Activator.CreateInstance<TReqBody>();

            /// <summary>
            /// Set unique id for the request.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public TBuilder SetRequestId(string id)
            {
                _req.options.Id = id;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the sender name of the request.
            /// </summary>
            /// <param name="sender"></param>
            /// <returns></returns>
            public virtual TBuilder SetSender(string sender)
            {
                _req.options.Sender = sender;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the number of retries of the request
            /// </summary>
            /// <param name="maxRetry"></param>
            /// <returns></returns>
            public TBuilder SetMaxRetry(int maxRetry)
            {
                _req.options.MaxRetry = maxRetry;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the delay of the retry in seconds
            /// </summary>
            /// <param name="retryDelayInSec"></param>
            /// <returns></returns>
            public TBuilder SetRetryDelay(float retryDelayInSec)
            {
                _req.options.RetryDelayInSec = retryDelayInSec;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set custom timeout for the request in seconds.
            /// If not set, the RESTClient will use its default timeout.
            /// </summary>
            /// <param name="timeoutInSec"></param>
            /// <returns></returns>
            public TBuilder SetTimeout(int timeoutInSec)
            {
                _req.options.Timeout = TimeSpan.FromSeconds(timeoutInSec);
                return (TBuilder)this;
            }

            /// <summary>
            /// Set custom timeout for the request.
            /// If not set, the RESTClient will use its default timeout.
            /// </summary>
            /// <param name="timeoutInSec"></param>
            /// <returns></returns>
            public TBuilder SetTimeout(TimeSpan timeout)
            {
                _req.options.Timeout = timeout;
                return (TBuilder)this;
            }

            public TBuilder SetCancellationToken(CancellationToken token)
            {
                _req.options.Token = token;
                return (TBuilder)this;
            }

            /// <summary>
            /// Set the stream handler for the request. This is used to handle the stream data.
            /// </summary>
            public TBuilder SetStreamHandler(IStreamHandler streamHandler)
            {
                _req.options.StreamHandler = streamHandler;
                return (TBuilder)this;
            }

            public TBuilder SetOutputPath(string path)
            {
                _req.options.OutputPath = path;
                return (TBuilder)this;
            }

            public TBuilder SetIgnoreLogs(bool ignoreLogs)
            {
                _req.options.IgnoreLogs = ignoreLogs;
                return (TBuilder)this;
            }

            public virtual TReqBody Build([CallerFilePath] string sender = "")
            {
                return Build(MIMEType.Json, sender);
            }

            public virtual TReqBody Build(MIMEType mimeType, [CallerFilePath] string sender = "")
            {
                if (string.IsNullOrEmpty(_req.options.Sender))
                {
                    _req.options.Sender = sender;
                    _req.options.Sender = Path.GetFileNameWithoutExtension(_req.options.Sender);
                }

                _req.options.MIMEType = mimeType;
                return _req;
            }
        }
    }
}