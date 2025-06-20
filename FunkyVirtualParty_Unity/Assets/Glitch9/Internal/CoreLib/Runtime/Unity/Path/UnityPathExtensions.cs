using System;
using System.IO;
using UnityEngine;

namespace Glitch9
{
    /// <summary>
    /// Dealing the path in Unity can be very painful.
    /// This class provides a set of extensions to make it easier to work with file paths in Unity.
    /// (It's still painful, but at least it's easier.)
    /// </summary>
    public static class UnityPathExtensions
    {
        /// <summary>
        /// Gets the full local path (starting with C:/ or D:/) for a UnityFilePath object.
        /// </summary>
        /// <param name="path">The UnityFilePath object</param>
        /// <returns>The resolved local path as a string</returns>
        public static string ToFullPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            path = path.Replace("\\", "/").Trim();
            path = path.TrimStart('/');

            if (Path.IsPathRooted(path))
                return Path.GetFullPath(path).Replace("\\", "/");

            string fullPath;

#if UNITY_EDITOR
            string assetsRoot = Application.dataPath.Replace("\\", "/");

            // ✅ 핵심: path가 Assets/로 시작하지 않으면 강제로 붙이기
            if (!path.StartsWith("Assets/"))
            {
                path = "Assets/" + path;
            }

            fullPath = Path.GetFullPath(Path.Combine(assetsRoot.Replace("/Assets", ""), path)).Replace("\\", "/");
#else
            fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path)).Replace("\\", "/");
#endif

            return fullPath;
        }

        /// <summary>
        /// Converts a relative path to an absolute path.
        /// </summary>
        internal static string ToRelativePath(this string path)
        {
            // string dataPath = Path.Combine(Application.dataPath, path);
            // return dataPath.FixSlashes().FixDoubleAssets();

            return UnityPathUtil.ConvertPath(path, UnityPathType.Assets);
        }

        /// <summary>
        /// Converts a relative path to an absolute path in the persistent data directory.
        /// </summary>
        internal static string ToPersistentDataPath(this string path)
        {
            // // 경로 구분자를 정확히 처리하고 절대 경로를 정확하게 검증
            // string fullPath = Path.Combine(Application.persistentDataPath, "");
            // if (filePath.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
            //     return filePath;

            // return Path.Combine(Application.persistentDataPath, filePath);

            return UnityPathUtil.ConvertPath(path, UnityPathType.PersistentData);
        }

        /// <summary>
        /// Converts a relative path to an absolute path in the StreamingAssets directory.
        /// </summary> 
        internal static string ToStreamingAssetsPath(this string path)
        {
            // const string kPrefix = "StreamingAssets/";
            // if (path.StartsWith(kPrefix)) path = path.Substring(kPrefix.Length); // 접두어 제거 
            // return Path.Combine(Application.streamingAssetsPath, path);

            return UnityPathUtil.ConvertPath(path, UnityPathType.StreamingAssets);
        }

        /// <summary>
        /// Common mistake when dealing with file paths in Unity is using two slashes (//) instead of one (/).
        /// Often occurs when combining two paths and one of them is already a slash in the end.
        /// This method fixes that by replacing all backslashes (\) with slashes (/) and removing double slashes (//).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string FixSlashes(this string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            return path.Replace('\\', '/').Replace("//", "/");
        }

        /// <summary>
        /// Common mistake when dealing with file paths in Unity is not starting the path with "Assets/".
        /// This method fixes that by adding "Assets/" to the beginning of the path if it doesn't already start with it.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string FixDoubleAssets(this string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            path = path.FixSlashes(); // 먼저 슬래시를 고치고 시작
            if (path.Contains("Assets/Assets/Assets")) return path.Replace("Assets/Assets/Assets", "Assets");
            if (path.Contains("Assets/Assets")) return path.Replace("Assets/Assets", "Assets");
            return path;
        }

        internal static string ToFileUri(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), "filePath is null or empty.");

            if (filePath.StartsWith("http://") || filePath.StartsWith("https://"))
                return filePath;

            if (!filePath.StartsWith("file://"))
                filePath = "file://" + filePath;

            return filePath.Replace("\\", "/"); // FixSlashes 대체
        }
    }
}