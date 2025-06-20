using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Glitch9
{
    public static class LogExtensions
    {
        public static bool LogIfNull<T>(this T obj, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            if (obj == null)
            {
                HandleError($"{ParseFilename(callerFilePath)}'s {typeof(T).Name} is null.", callerMemberName, callerFilePath);
                return true;
            }
            return false;
        }

        public static bool LogIfNullOrEmpty<T>(this IEnumerable<T> collection, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            if (collection == null)
            {
                HandleError($"{ParseFilename(callerFilePath)}'s {typeof(T).Name} collection is null.", callerMemberName, callerFilePath);
                return true;
            }

            // Optimizing for ICollection<T> to use Count when available
            if (collection is ICollection<T> coll)
            {
                if (coll.Count == 0)
                {
                    HandleError($"{ParseFilename(callerFilePath)}'s {typeof(T).Name} collection is empty.", callerMemberName, callerFilePath);
                    return true;
                }
            }
            else
            {
                using IEnumerator<T> enumerator = collection.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    HandleError($"{ParseFilename(callerFilePath)}'s {typeof(T).Name} collection is empty.", callerMemberName, callerFilePath);
                    return true;
                }
            }

            return false;
        }

        public static bool LogIfNullOrWhiteSpace(this string text, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                HandleError($"{ParseFilename(callerFilePath)}'s string is null or whitespace.", callerMemberName, callerFilePath);
                return true;
            }
            return false;
        }

        public static bool LogIfSetInInspector<T>(this T obj, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            if (obj == null)
            {
                LogService.ContinueWithLogger(LogType.Error, callerMemberName, $"<color=blue><{callerMemberName}></color> is not set in inspector", callerMemberName, callerFilePath);
                return false;
            }
            return true;
        }

        public static bool LogIfNotSetInInspector<T>(this T obj, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            if (obj == null)
            {
                LogService.ContinueWithLogger(LogType.Error, callerMemberName, $"<color=blue><{callerMemberName}></color> is not set in inspector", callerMemberName, callerFilePath);
                return true;
            }
            return false;
        }

        private static string ParseFilename(string callerFilaPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(callerFilaPath);
        }

        private static void HandleError(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            LogService.ContinueWithLogger(LogType.Error, callerMemberName, message, callerMemberName, callerFilePath);
        }
    }
}
