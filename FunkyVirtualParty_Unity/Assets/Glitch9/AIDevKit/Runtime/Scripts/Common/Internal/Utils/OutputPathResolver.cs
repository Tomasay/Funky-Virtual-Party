using System.IO;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// AIDevKit 내부에서 사용하는 파일 경로를 Resolve하는 유틸리티 클래스
    /// </summary>
    internal class OutputPathResolver
    {
        private const string kOutputFileNameFormat = "{api}-{id}";

        internal static string ResolveOutputFileName(Api api, string keyword, MIMEType mimeType)
        {
            string fileName = kOutputFileNameFormat.Replace("{api}", api.ToString().ToLower()).Replace("{id}", keyword.GetLastSegment());
            return FileUtil.GetUniqueFileName(fileName, mimeType, FileNamingRule.DateTime);
        }

        internal static string ResolveOutputFileName(AIDevKitAsset @ref, MIMEType mimeType) => ResolveOutputFileName(@ref.Api, @ref.Id, mimeType);
        internal static string ResolveOutputFilePath(string outputDir, AIDevKitAsset @ref, MIMEType mimeType) => ResolveOutputFilePath(outputDir, @ref.Api, @ref.Id, mimeType);

        internal static string ResolveOutputFilePath(string outputDir, Api provider, string id, MIMEType mimeType)
        {
            string folder = string.IsNullOrEmpty(outputDir) ? AIDevKitSettings.OutputPath : outputDir;
            string fileName = ResolveOutputFileName(provider, id, mimeType);

            return Path.Combine(folder, fileName);
        }

        internal static string AddIndexToPath(string absolutePath, int fileIndex)
        {
            string ext = Path.GetExtension(absolutePath);
            string fileName = Path.GetFileNameWithoutExtension(absolutePath);
            string dir = Path.GetDirectoryName(absolutePath);
            if (string.IsNullOrEmpty(dir)) dir = Application.persistentDataPath;
            return Path.Combine(dir, $"{fileName}_{fileIndex}{ext}");
        }
    }
}