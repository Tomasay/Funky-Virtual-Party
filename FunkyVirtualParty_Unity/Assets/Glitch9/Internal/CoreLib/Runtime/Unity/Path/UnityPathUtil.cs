using System;
using System.IO;
using UnityEngine;

namespace Glitch9
{
    public static class UnityPathUtil
    {
        /*         
        Path Types Examples

        1. Application.dataPath                : C:/UnityProjects/Glitch9/AI Development Kit/Assets   
        2. Application.streamingAssetsPath     : C:/UnityProjects/Glitch9/AI Development Kit/Assets/StreamingAssets         
        3. Application.dataPath + "/Resources" : C:/UnityProjects/Glitch9/AI Development Kit/Assets/Resources           
        4. Application.persistentDataPath      : C:/Users/Codeqo/AppData/LocalLow/Glitch9/Routina  
        5. Application.temporaryCachePath      : C:/Users/Codeqo/AppData/Local/Temp/Glitch9/Routina        
        6. Application.consoleLogPath          : C:/Users/Codeqo/AppData/Local/Unity/Editor/Editor.log           
        */

        public static string ConvertPath(string path, UnityPathType to)
        {
            UnityPathType from = ResolveType(path);
            // Debug.Log($"ConvertPath: <color=yellow>{path}</color> from <color=cyan>{from}</color> to {to}");
            return ConvertINTERNAL(from, to, path).FixDoubleAssets();
        }

        private static string ConvertINTERNAL(UnityPathType from, UnityPathType to, string path)
        {
            // Just return the path if the from and to types are the same.
            if (from == to) return path.FixSlashes();

            // First convert the path to an absolute path.
            string absolutePath = from switch
            {
                UnityPathType.Assets => Path.Combine(Application.dataPath, path).FixSlashes(),// Assets 타입이라면 Application.dataPath에 결합
                UnityPathType.StreamingAssets => Path.Combine(Application.streamingAssetsPath, path).FixSlashes(),
                UnityPathType.PersistentData => Path.Combine(Application.persistentDataPath, path).FixSlashes(),
                UnityPathType.Absolute => path.FixSlashes(),
                UnityPathType.Url => path,// 변환하지 않음
                _ => path.FixSlashes(),
            };

            // Then convert the absolute path to the desired type.
            return to switch
            {
                UnityPathType.Assets => absolutePath.Replace(Application.dataPath, "Assets").FixSlashes(),
                UnityPathType.StreamingAssets => absolutePath.Replace(Application.streamingAssetsPath, "StreamingAssets").FixSlashes(),
                UnityPathType.PersistentData => absolutePath.Replace(Application.persistentDataPath, "PersistentData").FixSlashes(),
                UnityPathType.Absolute => absolutePath,// 이미 절대 경로가 되었으므로 그대로 반환
                UnityPathType.Url => absolutePath,// 로컬 경로를 웹 URL로 변환하는 건 일반적이지 않으므로 그대로 반환합니다.
                _ => absolutePath,
            };
        }

        /// <summary>
        /// Resolves the path type based on the given file path.
        /// </summary>
        /// <param name="unknownFormattedPath">The file path as a string</param>
        /// <returns>>The resolved path type</returns>
        internal static UnityPathType ResolveType(string unknownFormattedPath)
        {
            if (string.IsNullOrWhiteSpace(unknownFormattedPath))
            {
                Debug.LogError("Path is null or empty.");
                return UnityPathType.Unknown;
            }

            unknownFormattedPath = unknownFormattedPath.FixSlashes();

            // 웹 URL 확인
            if (unknownFormattedPath.StartsWith("http://") || unknownFormattedPath.StartsWith("https://"))
                return UnityPathType.Url;

            // Assets 경로 여부를 판단 (Application.dataPath로 시작하는지 확인) 
            if (unknownFormattedPath.StartsWith("Assets/"))
                return UnityPathType.Assets;

            // StreamingAssets나 Resources 폴더 관련 경로 판단
            if (unknownFormattedPath.Contains("/StreamingAssets/"))
                return UnityPathType.StreamingAssets;
            if (unknownFormattedPath.Contains("/Resources/"))
                return UnityPathType.Resources;

            // 특정 사용자 폴더 (PersistentData 등) 확인
            if (unknownFormattedPath.Contains(":/Users/"))
            {
                if (unknownFormattedPath.Contains("/Temp/"))
                    return UnityPathType.TemporaryCache;
                if (unknownFormattedPath.Contains("/Unity/Editor/"))
                    return UnityPathType.ConsoleLog;
                return UnityPathType.PersistentData;
            }

            // 절대 경로인지 판단 (예: C:/, D:/ 등)
            if (unknownFormattedPath.Contains(":/"))
                return UnityPathType.Absolute;

            return UnityPathType.Unknown;
        }

        internal static string ExtractResourcesPath(string fullPath)
        {
            // Assets/Resources/SFX/clip.wav -> SFX/clip (확장자 제거)
            var index = fullPath.IndexOf("Resources", StringComparison.OrdinalIgnoreCase);
            var resPath = fullPath.Substring(index + "Resources".Length + 1); // skip '/'
            return Path.ChangeExtension(resPath, null); // remove .wav etc
        }
    }
}