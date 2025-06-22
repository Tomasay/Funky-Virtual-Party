using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    internal static class AIDevKitDebug
    {
        internal readonly static Prefs<bool> kDebugMode = new("AIDevKit.DebugMode", false);

        internal static void Mark(params int[] num)
        {
            if (!kDebugMode.Value) return;
            Debug.Log($"<color=magenta>-----------------{string.Join("-", num)}-----------------</color>");
        }

        internal static void Mark(string message)
        {
            if (!kDebugMode.Value) return;
            Debug.Log($"<color=magenta>-----------------{message}-----------------</color>");
        }

        internal static void Info(string message, string tag = "Debug")
        {
            if (!kDebugMode.Value) return;
            Debug.Log($"<color=magenta>[{tag}] {message}</color>");
        }

        internal static void Green(string message, string tag = "Debug")
        {
            if (!kDebugMode.Value) return;
            Debug.Log($"<color=#3cff4b>[{tag}] {message}</color>");
        }

        internal static void Red(string message, string tag = "Debug")
        {
            if (!kDebugMode.Value) return;
            Debug.Log($"<color=red>[{tag}] {message}</color>");
        }

        internal static void Blue(string message, string tag = "Debug")
        {
            if (!kDebugMode.Value) return;
            Debug.Log($"<color=cyan>[{tag}] {message}</color>");
        }

        internal static void Warning(string message)
        {
            if (!kDebugMode.Value) return;
            Debug.LogWarning(message);
        }

        internal static void Error(string message)
        {
            if (!kDebugMode.Value) return;
            Debug.LogError(message);
        }

        internal static string FormatErrorMessage(string rawErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(rawErrorMessage)) return "Unknown error has occurred.";

            if (rawErrorMessage.Contains("Unrecognized request argument supplied: reasoning_effort"))
            {
                return "This model does not support the reasoning_effort parameter. Please use reasoning models (e.g. o-series), or do not set the reasoning_effort parameter.";
            }

            return rawErrorMessage;
        }

        internal static string ExtractErrorMessage(Exception exception)
        {
            string message = exception?.Message;
            if (string.IsNullOrWhiteSpace(message))
                return "Unknown error has occurred.";

            try
            {
                // if (message.Contains("HTTP/1.1 403 Forbidden")) // remove 'HTTP/1.1 403 Forbidden'
                //     message = message.Replace("HTTP/1.1 403 Forbidden", "").Trim();

                // HTTP/1.1 403 Forbidden 뿐만 아니라 다른 HTTP 상태 코드도 포함될 수 있으므로, 일반적인 HTTP 상태 코드 패턴을 제거
                message = Regex.Replace(message, @"HTTP/\d\.\d \d{3}(?: [^\r\n]*)?", "").Trim();

                ErrorResponseWrapper errorResponse = JsonConvert.DeserializeObject<ErrorResponseWrapper>(message);

                if (errorResponse != null && errorResponse.Error != null)
                {
                    return errorResponse.Error.GetMessage();
                }
                else
                {
                    return message;
                }
            }
            catch
            {
                return message;
            }
        }
    }
}