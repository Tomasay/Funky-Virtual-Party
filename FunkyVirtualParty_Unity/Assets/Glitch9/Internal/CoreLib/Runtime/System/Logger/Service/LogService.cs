using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Glitch9
{
    public partial class LogService
    {
        public static event Action<LogData> OnLogAdded;

        private static class PrefsKeys
        {
            internal const string ENABLED = "GNLog.Enabled";
            internal const string COLORED = "GNLog.Colored";
            internal const string LOG_ERROR_ON_ERROR_INSTANCE_CREATED = "GNLog.LogErrorOnErrorInstanceCreated";
            internal const string LOG_STACKTRACE_ON_ERROR_INSTANCE_CREATED = "GNLog.LogStackTraceOnErrorInstanceCreated";
            internal const string SHOW_CALLER_MEMBER_NAME = "GNLog.ShowCallerMemberName";
        }

        private static class DefaultValues
        {
            internal const string UNKNOWN_SENDER = "Unknown";
            internal const bool ENABLED = true;
            internal const bool COLORED = false;
            internal const bool SHOW_CALLER_FILE_PATH = false;
            internal const bool LOG_ERROR_ON_ERROR_INSTANCE_CREATED = true;
            internal const bool LOG_STACKTRACE_ON_ERROR_INSTANCE_CREATED = false;
        }

        public static Prefs<bool> IsEnabled { get; set; } = new(PrefsKeys.ENABLED, DefaultValues.ENABLED);
        public static Prefs<bool> IsColored { get; set; } = new(PrefsKeys.COLORED, DefaultValues.COLORED);
        public static Prefs<bool> ShowCallerMemberName { get; set; } = new(PrefsKeys.SHOW_CALLER_MEMBER_NAME, DefaultValues.SHOW_CALLER_FILE_PATH);
        public static Prefs<bool> LogErrorOnErrorInstanceCreated { get; set; } = new(PrefsKeys.LOG_ERROR_ON_ERROR_INSTANCE_CREATED, DefaultValues.LOG_ERROR_ON_ERROR_INSTANCE_CREATED);
        public static Prefs<bool> LogStackTraceOnErrorInstanceCreated { get; set; } = new(PrefsKeys.LOG_STACKTRACE_ON_ERROR_INSTANCE_CREATED, DefaultValues.LOG_STACKTRACE_ON_ERROR_INSTANCE_CREATED);

        private static readonly Dictionary<LogType, string> kCachedColors = new();
        public static Stack<LogData> CachedLogs = new();

        public static void ClearCachedColorHex() => kCachedColors.Clear();
        public static string GetColorHex(LogType logType)
        {
            if (kCachedColors.TryGetValue(logType, out string hex)) return hex;
            switch (logType)
            {
                case LogType.Info: kCachedColors.Add(logType, Color.black.ToHex()); break;
                case LogType.Verbose: kCachedColors.Add(logType, Color.blue.ToHex()); break;
                case LogType.Warning: kCachedColors.Add(logType, ExColor.orange.ToHex()); break;
                case LogType.Error: kCachedColors.Add(logType, ExColor.clementine.ToHex()); break;
                case LogType.Exception: kCachedColors.Add(logType, ExColor.purple.ToHex()); break;
                case LogType.NativeInfo: kCachedColors.Add(logType, ExColor.gold.ToHex()); break;
                case LogType.NativeError: kCachedColors.Add(logType, ExColor.garnet.ToHex()); break;
            }
            return kCachedColors[logType];
        }

        public static void ContinueWithLogger(LogType logType, object sender, ExceptionType issue, string callerMemberName = null, string callerFilePath = null)
        {
            ContinueWithLogger(logType, sender, issue.ToString(), callerMemberName, callerFilePath);
        }

        public static void ContinueWithLogger(LogType logType, object sender, string msg, string callerMemberName = null, string callerFilePath = null)
        {
            if (string.IsNullOrEmpty(msg))
            {
                UnityEngine.Debug.LogError("Log cannot be processed. Message is null or empty.");
                return;
            }

            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                string senderAsString = ParseSender(sender, callerFilePath);

#if UNITY_EDITOR
                sb.Append(UnityEditor.EditorGUIUtility.isProSkin ? "<color=yellow>[" : "<color=blue>[");
#else
                sb.Append("[");
#endif
                sb.Append(senderAsString);

                if (ShowCallerMemberName && !string.IsNullOrEmpty(callerMemberName))
                {
                    sb.Append(callerMemberName);
                }

#if UNITY_EDITOR
                sb.Append("]</color> ");
#else
                sb.Append("] ");
#endif
                bool isColored = IsColored;

                if (isColored) sb.Append($"<color={GetColorHex(logType)}>");

                sb.Append(msg);

                if (isColored) sb.Append("</color>");

                string log = sb.ToString();

                LogData logData = new(logType, HandlePlatformSpecificFormat(log));

                switch (logType)
                {
                    case LogType.Info:
                    case LogType.Verbose:
                    case LogType.NativeInfo:
                    case LogType.NativeVerbose:
                        UnityEngine.Debug.Log(log);
                        break;
                    case LogType.Warning:
                    case LogType.NativeWarning:
                        UnityEngine.Debug.LogWarning(log);
                        break;
                    case LogType.Error:
                    case LogType.NativeError:
                    case LogType.Exception:
                    case LogType.NativeCritical:
                        UnityEngine.Debug.LogError(log);
                        break;
                }

                CachedLogs.Push(logData);
                OnLogAdded?.Invoke(logData);
            }
        }


        private static string HandlePlatformSpecificFormat(string log)
        {
#if UNITY_ANDROID
            if (log.Contains("\\"))
            {
                log = log.Substring(log.LastIndexOf("\\"));
                log = log.Replace("\\", "");
                log = "[ " + log;
            }
            return log;
#else
            return log;
#endif
        }


        private static string ParseSender(object sender, string callerFilePath)
        {
            if (sender == null)
            {
                if (string.IsNullOrEmpty(callerFilePath)) return DefaultValues.UNKNOWN_SENDER;
                return Path.GetFileNameWithoutExtension(callerFilePath);
            }

            if (sender is string s)
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (string.IsNullOrEmpty(callerFilePath)) return DefaultValues.UNKNOWN_SENDER;
                    else return Path.GetFileNameWithoutExtension(callerFilePath);
                }
                return s;
            }

            return sender.GetType().Name;
        }
    }
}
