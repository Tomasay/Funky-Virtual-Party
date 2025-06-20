using System;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Reason that a <see cref="ChatCompletion"/> request stopped generating tokens.
    /// </summary>
    [JsonConverter(typeof(NullableStopReasonConverter))]
    public enum StopReason
    {
        /// <summary>
        /// Default value (Unused).
        /// </summary>
        None,

        /// <summary>
        /// Natural stop point of the model or provided stop sequence.
        /// </summary>
        Stop,

        /// <summary>
        /// The maximum number of tokens as specified in the request was reached.
        /// </summary>
        MaxTokens,

        /// <summary>
        /// The content was flagged for safety reasons.
        /// </summary>
        Safety,

        /// <summary>
        /// The content was flagged for recitation reasons.
        /// </summary>
        Recitation,

        /// <summary>
        /// The model called a tool, or function_call.
        /// </summary>
        ToolCalls,

        /// <summary>
        /// Unknown reason.
        /// </summary>
        Other
    }

    internal class NullableStopReasonConverter : JsonConverter<StopReason?>
    {
        public override StopReason? ReadJson(JsonReader reader, Type objectType, StopReason? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            string value = reader.Value?.ToString();
            if (string.IsNullOrEmpty(value)) return StopReason.None;

            return value switch
            {
                "stop" or "STOP" => StopReason.Stop,
                "length" or "MAX_TOKENS" => StopReason.MaxTokens,
                "content_filter" or "SAFETY" => StopReason.Safety,
                "tool_calls" or "function_call" => StopReason.ToolCalls,
                "RECITATION" => StopReason.Recitation,
                "OTHER" => StopReason.Other,
                _ => StopReason.None,
            };
        }

        public override void WriteJson(JsonWriter writer, StopReason? value, JsonSerializer serializer) => throw new NotImplementedException();
    }

    internal static class FinishReasonExtensions
    {
        internal static string GetMessage(this StopReason reason)
        {
            return reason switch
            {
                StopReason.None => "The model has not stopped generating the tokens.",
                StopReason.Stop => "Natural stop point of the model or provided stop sequence.",
                StopReason.MaxTokens => "The model reached the maximum token limit.",
                StopReason.Safety => "The candidate content was flagged for safety reasons.",
                StopReason.Recitation => "The candidate content was flagged for recitation reasons.",
                StopReason.Other => "Unknown reason.",
                _ => "The model has not stopped generating the tokens.",
            };
        }
    }
}