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

        internal static string FormatErrorMessage(string error)
        {
            if (string.IsNullOrWhiteSpace(error)) return "Unknown error has occurred.";

            if (error.Contains("Unrecognized request argument supplied: reasoning_effort"))
            {
                return "This model does not support the reasoning_effort parameter. Please use reasoning models (e.g. o-series), or do not set the reasoning_effort parameter.";
            }

            return error;
        }
    }
}