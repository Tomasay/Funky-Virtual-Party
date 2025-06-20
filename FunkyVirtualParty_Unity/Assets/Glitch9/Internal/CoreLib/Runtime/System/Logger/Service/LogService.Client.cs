using System;
using System.Runtime.CompilerServices;

namespace Glitch9
{
    public partial class LogService
    {
        private const string kNativeLogTag = "Native (Android/iOS)";

        public static void Info(string msg,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            ContinueWithLogger(LogType.Info, null, msg, callerMemberName, callerFilePath);
        }

        public static void Info(string msg, object sender,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            ContinueWithLogger(LogType.Info, sender, msg, callerMemberName, callerFilePath);
        }

        public static void Warning(string msg,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            ContinueWithLogger(LogType.Warning, null, msg, callerMemberName, callerFilePath);
        }

        public static void Warning(string msg, object sender,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            ContinueWithLogger(LogType.Warning, sender, msg, callerMemberName, callerFilePath);
        }

        public static void Error(string msg,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            ContinueWithLogger(LogType.Error, null, msg, callerMemberName, callerFilePath);
        }

        public static void Error(string msg, object sender,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            ContinueWithLogger(LogType.Error, sender, msg, callerMemberName, callerFilePath);
        }

        public static void Exception(Exception e,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            string msg = e.Message + "\n" + e.StackTrace;
            ContinueWithLogger(LogType.Exception, null, msg, callerMemberName, callerFilePath);
        }

        public static void Exception(Exception e, object sender,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            string msg = e.Message + "\n" + e.StackTrace;
            ContinueWithLogger(LogType.Exception, sender, msg, callerMemberName, callerFilePath);
        }

        public static void Native(string msg, object sender = null)
        {
            ContinueWithLogger(LogType.NativeInfo, sender, msg, kNativeLogTag);
        }

        public static void NativeWarning(string msg, object sender = null)
        {
            ContinueWithLogger(LogType.NativeWarning, sender, msg, kNativeLogTag);
        }

        public static void NativeError(string msg, object sender = null)
        {
            ContinueWithLogger(LogType.NativeError, sender, msg, kNativeLogTag);
        }
    }
}