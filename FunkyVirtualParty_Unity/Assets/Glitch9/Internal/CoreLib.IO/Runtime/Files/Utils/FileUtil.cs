using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public enum FileNamingRule
    {
        DateTime,
        GUID,
    }

    public static class FileUtil
    {
        public static async UniTask<byte[]> ReadAllBytesAsync(string filePath)
        {
            string absolutePath = filePath.ToFullPath();
            return await UniTask.RunOnThreadPool(() => System.IO.File.ReadAllBytes(absolutePath));
        }

        public static string GetUniqueFileName(MIMEType fileContentType, FileNamingRule namingRule) => GetUniqueFileName(null, fileContentType, namingRule);
        public static string GetUniqueFileName(string tag, MIMEType fileContentType, FileNamingRule namingRule)
        {
            tag ??= fileContentType.ToString();

            string baseName = namingRule switch
            {
                FileNamingRule.DateTime => BuildTimestampedName(tag),
                FileNamingRule.GUID => BuildGuidName(tag),
                _ => throw new ArgumentOutOfRangeException(nameof(namingRule), namingRule, null),
            };

            string extension = fileContentType.GetExtension();
            return $"{baseName}{extension}";
        }

        private static string BuildTimestampedName(string tag) => $"{tag}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}";
        private static string BuildGuidName(string tag) => $"{tag}_{Guid.NewGuid()}";

        public static async UniTask<RawFile> CreateRawFileAsync(byte[] fileBytes, string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await File.WriteAllBytesAsync(filePath, fileBytes);

            return new RawFile(fileBytes, filePath);// { FullPath = filePath, Value = fileBytes };
        }

        public static bool IsImage(this byte[] fileBytes)
        {
            List<byte[]> headers = new()
            {
                Encoding.ASCII.GetBytes("BM"),      // BMP
                Encoding.ASCII.GetBytes("GIF"),     // GIF
                new byte[] { 137, 80, 78, 71 },     // PNG
                new byte[] { 73, 73, 42 },          // TIFF
                new byte[] { 77, 77, 42 },          // TIFF
                new byte[] { 255, 216, 255, 224 },  // JPEG
                new byte[] { 255, 216, 255, 225 }   // JPEG CANON
            };

            return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
        }

        public static void RefreshIfEditor()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static bool ValidateFileSize(string localPath)
        {
            // check directory first, because this can cause StackOverflow
            string directory = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(directory))
            {
                LogService.Warning($"Directory does not exist: {directory}");
                return false;
            }

            if (File.Exists(localPath))
            {
                /* 파일 사이즈 체크 */
                FileInfo fileInfo = new(localPath);
                long size = fileInfo.Length;

                if (size < 1024) // 5kb
                {
                    LogService.Warning($"Detected a file with a size of less than 1KB: {localPath}");
                    return false;
                }
                return true;
            }

            return false;
        }

        public static List<T> GetFiles<T>(List<IFile> list) where T : IFile
        {
            if (!list.IsNullOrEmpty())
            {
                List<T> files = new();

                foreach (IFile content in list)
                {
                    if (content is T res) files.Add(res);
                }

                return files;
            }

            return new();
        }

        public static T GetFirstFile<T>(List<IFile> list) where T : IFile
        {
            if (!list.IsNullOrEmpty())
            {
                foreach (IFile content in list)
                {
                    if (content is T res) return res;
                }
            }
            return default;
        }
    }
}