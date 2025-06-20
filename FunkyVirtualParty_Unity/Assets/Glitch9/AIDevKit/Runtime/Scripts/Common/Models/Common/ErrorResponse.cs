using System;
using System.Text;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public class ErrorResponse
    {
        /// <summary>
        /// The type of error (e.g., "invalid_request_error", "server_error").
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// Error code, if any.
        /// </summary>
        [JsonProperty("code")] public string Code { get; set; }

        /// <summary>
        /// A human-readable error message.
        /// </summary>
        [JsonProperty("message")] public string Message { get; set; }

        /// <summary>
        /// Parameter related to the error, if any.
        /// </summary>
        [JsonProperty("param")] public string Param { get; set; }

        /// <summary>
        /// The event_id of the client event that caused the error, if applicable.
        /// </summary>
        [JsonProperty("event_id")] public string EventId { get; set; }

        /// <summary>
        /// This is from Google API, but I don't really know what it is.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        public override string ToString()
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.AppendLine($"{Message}");
                if (!string.IsNullOrEmpty(Type)) sb.AppendLine($"Type: {Type}");
                if (!string.IsNullOrEmpty(Param)) sb.AppendLine($"Param: {Param}");
                if (!string.IsNullOrEmpty(Code)) sb.AppendLine($"Code: {Code}");
                if (!string.IsNullOrEmpty(EventId)) sb.AppendLine($"Event ID: {EventId}");
                if (!string.IsNullOrEmpty(Status)) sb.AppendLine($"Status: {Status}");
                return sb.ToString();
            }
        }

        internal string GetMessage()
        {
            if (!string.IsNullOrEmpty(Message)) return Message;
            // If the message is empty, return the type and code
            return $"{Type} ({Code})";
        }

        public ErrorResponse() { }
        internal ErrorResponse(string message) => Message = message;
        internal ErrorResponse(Exception exception)
        {
            Type = exception.GetType().Name;
            Message = exception.Message;

            if (exception.InnerException != null)
            {
                Message += $" Inner Exception: {exception.InnerException.Message}";
            }

            if (exception.StackTrace != null)
            {
                Message += $"\nStack Trace: {exception.StackTrace}";
            }
        }
    }

    public class ErrorResponseWrapper
    {
        [JsonProperty("error")] public ErrorResponse Error { get; set; }
        public override string ToString() => Error?.ToString() ?? string.Empty;
    }
}
