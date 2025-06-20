using Glitch9.IO.Networking.RESTApi;
using System.IO;

namespace Glitch9.IO.Files
{
    public enum MIMEType
    {
        [ApiEnum("Unknown", "", "")] Unknown,
        [ApiEnum("JSON", "application/json", "json")] Json,
        [ApiEnum("JSONL", "application/jsonl", "jsonl")] Jsonl,
        [ApiEnum("XML", "application/xml", "xml")] Xml,
        [ApiEnum("WWW Form", "application/x-www-form-urlencoded", "x-www-form")] WWWForm,
        [ApiEnum("Multipart Form", "multipart/form-data", "multipart")] MultipartForm,
        [ApiEnum("Plain Text", "text/plain", "plain")] PlainText,
        [ApiEnum("HTML", "text/html", "html")] HTML,
        [ApiEnum("CSV", "text/csv", "csv")] CSV,
        [ApiEnum("Octet Stream", "application/octet-stream", "octet")] OctetStream,
        [ApiEnum("PDF", "application/pdf", "pdf")] PDF,
        [ApiEnum("MPEG Audio", "audio/mpeg", "mpeg")] MPEG,
        [ApiEnum("WAV", "audio/wav", "wav")] WAV,
        [ApiEnum("FLAC", "audio/flac", "flac")] FLAC,
        [ApiEnum("OGG", "audio/ogg", "ogg")] OGG,
        [ApiEnum("AAC", "audio/aac", "aac")] AAC,
        [ApiEnum("Opus", "audio/opus", "opus")] Opus,
        [ApiEnum("PCM", "audio/pcm", "pcm")] PCM,
        [ApiEnum("uLaw", "audio/ulaw", "ulaw")] uLaw,
        [ApiEnum("aLaw", "audio/alaw", "alaw")] aLaw,
        [ApiEnum("MuLaw", "audio/basic", "mulaw")] MuLaw,
        [ApiEnum("JPEG", "image/jpeg", "jpeg")] JPEG,
        [ApiEnum("PNG", "image/png", "png")] PNG,
        [ApiEnum("GIF", "image/gif", "gif")] GIF,
        [ApiEnum("MP4", "video/mp4", "mp4")] MP4,
        [ApiEnum("AVI", "video/x-msvideo", "avi")] AVI,
        [ApiEnum("C", "text/x-c", "c")] C,
        [ApiEnum("C#", "text/x-csharp", "csharp")] CSharp,
        [ApiEnum("C++", "text/x-c++src", "c++")] CPP,
        [ApiEnum("Word Binary", "application/msword", "msword")] MsDoc,
        [ApiEnum("Word XML", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")] MsDocXML,
        [ApiEnum("PowerPoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx")] MsPowerPointPresentation,
        [ApiEnum("Excel XML", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx")] MsExcelXML,
        [ApiEnum("Java", "text/x-java-source", "java")] Java,
        [ApiEnum("Markdown", "text/markdown", "md")] Markdown,
        [ApiEnum("PHP", "application/x-httpd-php", "php")] HypertextPreprocessor,
        [ApiEnum("Python", "text/x-python", "python")] Python,
        [ApiEnum("Python Script", "text/x-python-script", "py")] PythonScript,
        [ApiEnum("Ruby", "text/x-ruby", "ruby")] Ruby,
        [ApiEnum("TeX", "application/x-tex", "tex")] TeX,
        [ApiEnum("CSS", "text/css", "css")] CascadingStyleSheets,
        [ApiEnum("JavaScript", "application/javascript", "javascript")] JavaScript,
        [ApiEnum("Shell Script", "application/x-sh", "sh")] ShellScript,
        [ApiEnum("TypeScript", "application/typescript", "ts")] TypeScript,
        [ApiEnum("TAR", "application/x-tar", "tar")] TapeArchive,
        [ApiEnum("ZIP", "application/zip", "zip")] ZIP,
    }


    public static class MIMETypeUtil
    {
        public static bool IsImage(this MIMEType mimeType) => StartsWith(mimeType, "image/");
        public static bool IsVideo(this MIMEType mimeType) => StartsWith(mimeType, "video/");
        public static bool IsAudio(this MIMEType mimeType) => StartsWith(mimeType, "audio/");

        private static bool StartsWith(this MIMEType mimeType, string keyword)
        {
            string mimeTypeString = mimeType.ToApiValue();
            return mimeTypeString.StartsWith(keyword);
        }

        public static RESTHeader GetHeader(this MIMEType mimeType) => new("Content-Type", mimeType.ToApiValue());
        public static MIMEType ParseApiValue(string contentTypeAsString)
        {
            if (string.IsNullOrEmpty(contentTypeAsString)) return MIMEType.Unknown;
            if (contentTypeAsString.Contains(";")) contentTypeAsString = contentTypeAsString.Split(';')[0].Trim();
            return contentTypeAsString switch
            {
                "application/json" => MIMEType.Json,
                "application/jsonl" => MIMEType.Jsonl,
                "application/xml" => MIMEType.Xml,
                "application/x-www-form-urlencoded" => MIMEType.WWWForm,
                "multipart/form-data" => MIMEType.MultipartForm,
                "text/plain" => MIMEType.PlainText,
                "text/html" => MIMEType.HTML,
                "text/csv" => MIMEType.CSV,
                "text/x-c" => MIMEType.C,
                "text/x-csharp" => MIMEType.CSharp,
                "text/x-c++" => MIMEType.CPP,
                "application/msword" => MIMEType.MsDoc,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => MIMEType.MsDocXML,
                "application/vnd.openxmlformats-officedocument.presentationml.presentation" => MIMEType.MsPowerPointPresentation,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => MIMEType.MsExcelXML,
                "text/x-java" => MIMEType.Java,
                "text/markdown" => MIMEType.Markdown,
                "text/x-php" => MIMEType.HypertextPreprocessor,
                "text/x-python" => MIMEType.Python,
                "text/x-script.python" => MIMEType.PythonScript,
                "text/x-ruby" => MIMEType.Ruby,
                "text/x-tex" => MIMEType.TeX,
                "text/css" => MIMEType.CascadingStyleSheets,
                "text/javascript" => MIMEType.JavaScript,
                "application/x-sh" => MIMEType.ShellScript,
                "application/typescript" => MIMEType.TypeScript,
                "application/octet-stream" => MIMEType.OctetStream,
                "application/pdf" => MIMEType.PDF,
                "audio/mpeg" => MIMEType.MPEG,
                "audio/wav" => MIMEType.WAV,
                "audio/flac" => MIMEType.FLAC,
                "audio/ogg" => MIMEType.OGG,
                "audio/aac" => MIMEType.AAC,
                "audio/opus" => MIMEType.Opus,
                "audio/ulaw" => MIMEType.uLaw,
                "audio/alaw" => MIMEType.aLaw,
                "audio/mulaw" => MIMEType.MuLaw,
                "audio/pcm" => MIMEType.PCM,
                "image/jpeg" => MIMEType.JPEG,
                "image/png" => MIMEType.PNG,
                "image/gif" => MIMEType.GIF,
                "video/mp4" => MIMEType.MP4,
                "video/x-msvideo" => MIMEType.AVI,
                "application/x-tar" => MIMEType.TapeArchive,
                "application/zip" => MIMEType.ZIP,
                _ => MIMEType.Json
            };
        }

        public static MIMEType ParseFromPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return MIMEType.Unknown;

            string extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".json" => MIMEType.Json,
                ".jsonl" => MIMEType.Jsonl,
                ".xml" => MIMEType.Xml,
                ".txt" => MIMEType.PlainText,
                ".html" => MIMEType.HTML,
                ".csv" => MIMEType.CSV,
                ".c" => MIMEType.C,
                ".cs" => MIMEType.CSharp,
                ".cpp" => MIMEType.CPP,
                ".doc" => MIMEType.MsDoc,
                ".docx" => MIMEType.MsDocXML,
                ".ppt" => MIMEType.MsPowerPointPresentation,
                ".pptx" => MIMEType.MsPowerPointPresentation,
                ".xls" => MIMEType.MsExcelXML,
                ".xlsx" => MIMEType.MsExcelXML,
                ".java" => MIMEType.Java,
                ".md" => MIMEType.Markdown,
                ".php" => MIMEType.HypertextPreprocessor,
                ".py" => MIMEType.Python,
                ".rb" => MIMEType.Ruby,
                ".tex" => MIMEType.TeX,
                ".css" => MIMEType.CascadingStyleSheets,
                ".js" => MIMEType.JavaScript,
                ".sh" => MIMEType.ShellScript,
                ".ts" => MIMEType.TypeScript,
                ".bin" => MIMEType.OctetStream,
                ".pdf" => MIMEType.PDF,
                ".mp3" => MIMEType.MPEG,
                ".wav" => MIMEType.WAV,
                ".flac" => MIMEType.FLAC,
                ".ogg" => MIMEType.OGG,
                ".aac" => MIMEType.AAC,
                ".opus" => MIMEType.Opus,
                ".pcm" => MIMEType.PCM,
                ".au" => MIMEType.MuLaw,
                ".ulaw" => MIMEType.uLaw,
                ".alaw" => MIMEType.aLaw,
                ".jpeg" => MIMEType.JPEG,
                ".jpg" => MIMEType.JPEG,
                ".png" => MIMEType.PNG,
                ".gif" => MIMEType.GIF,
                ".mp4" => MIMEType.MP4,
                ".avi" => MIMEType.AVI,
                ".tar" => MIMEType.TapeArchive,
                ".zip" => MIMEType.ZIP,
                _ => MIMEType.Unknown
            };
        }

        public static string GetExtension(this MIMEType mimeType)
        {
            return mimeType switch
            {
                MIMEType.Json => ".json",
                MIMEType.Jsonl => ".jsonl",
                MIMEType.Xml => ".xml",
                MIMEType.WWWForm => ".form",
                MIMEType.MultipartForm => ".form",
                MIMEType.PlainText => ".txt",
                MIMEType.HTML => ".html",
                MIMEType.CSV => ".csv",
                MIMEType.C => ".c",
                MIMEType.CSharp => ".cs",
                MIMEType.CPP => ".cpp",
                MIMEType.MsDoc => ".doc",
                MIMEType.MsDocXML => ".docx",
                MIMEType.MsPowerPointPresentation => ".ppt",
                MIMEType.MsExcelXML => ".xls",
                MIMEType.Java => ".java",
                MIMEType.Markdown => ".md",
                MIMEType.HypertextPreprocessor => ".php",
                MIMEType.Python => ".py",
                MIMEType.Ruby => ".rb",
                MIMEType.TeX => ".tex",
                MIMEType.CascadingStyleSheets => ".css",
                MIMEType.JavaScript => ".js",
                MIMEType.ShellScript => ".sh",
                MIMEType.TypeScript => ".ts",
                MIMEType.OctetStream => ".bin",
                MIMEType.PDF => ".pdf",
                MIMEType.MPEG => ".mp3",
                MIMEType.WAV => ".wav",
                MIMEType.FLAC => ".flac",
                MIMEType.OGG => ".ogg",
                MIMEType.AAC => ".aac",
                MIMEType.Opus => ".opus",
                MIMEType.PCM => ".pcm",
                MIMEType.uLaw => ".ulaw",
                MIMEType.aLaw => ".alaw",
                MIMEType.MuLaw => ".au",
                MIMEType.JPEG => ".jpeg",
                MIMEType.PNG => ".png",
                MIMEType.GIF => ".gif",
                MIMEType.MP4 => ".mp4",
                MIMEType.AVI => ".avi",
                MIMEType.TapeArchive => ".tar",
                MIMEType.ZIP => ".zip",
                _ => null,
            };
        }
    }
}