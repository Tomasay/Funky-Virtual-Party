using System.IO;
using UnityEngine;

namespace Glitch9.IO.Files
{
    public static class AudioTypeUtil
    {
        public static AudioType ParseFromExtension(string fileExt)
        {
            if (string.IsNullOrEmpty(fileExt)) return AudioType.UNKNOWN;
            if (!fileExt.StartsWith(".")) fileExt = "." + fileExt;

            switch (fileExt)
            {
                case ".wav": return AudioType.WAV;
                case ".mp3": return AudioType.MPEG;
                case ".ogg": return AudioType.OGGVORBIS;
                case ".aif":
                case ".aiff": return AudioType.AIFF;
                case ".acc": return AudioType.ACC;
                case ".it": return AudioType.IT;
                case ".mod": return AudioType.MOD;
                case ".s3m": return AudioType.S3M;
                case ".xm": return AudioType.XM;
                case ".xma": return AudioType.XMA;
                case ".vag": return AudioType.VAG;
                default:
                    LogService.Error($"Unsupported Audio Format: {fileExt}");
                    return AudioType.UNKNOWN;
            }
        }

        public static AudioType ParseFromPath(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return ParseFromExtension(extension);
        }

        public static string GetExtension(this AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.WAV: return ".wav";
                case AudioType.MPEG: return ".mp3";
                case AudioType.OGGVORBIS: return ".ogg";
                case AudioType.AIFF: return ".aif";
                case AudioType.ACC: return ".acc";
                case AudioType.IT: return ".it";
                case AudioType.MOD: return ".mod";
                case AudioType.S3M: return ".s3m";
                case AudioType.XM: return ".xm";
                case AudioType.XMA: return ".xma";
                case AudioType.VAG: return ".vag";
                default:
                    LogService.Error($"Unsupported Audio Format: {audioType}");
                    return string.Empty;
            }
        }
    }
}