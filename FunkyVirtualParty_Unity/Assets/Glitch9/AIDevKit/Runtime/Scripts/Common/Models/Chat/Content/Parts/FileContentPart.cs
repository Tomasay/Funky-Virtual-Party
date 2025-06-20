using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public class FileBase64ContentPart : FileContentPart { }
    public class FileIdContentPart : FileContentPart { }
    public class FileContentPart : ContentPart
    {
        [JsonProperty("file")] public FileRef File { get; set; }
        public override string ToString() => File?.Filename;

        public static FileBase64ContentPart FromBase64(string base64File, string filename)
        {
            return new FileBase64ContentPart
            {
                Type = ContentPartType.File,
                File = new FileRef
                {
                    FileData = base64File,
                    Filename = filename
                }
            };
        }

        public static FileBase64ContentPart FromBase64(string base64File, MIMEType mimeType)
        {
            return new FileBase64ContentPart
            {
                Type = ContentPartType.File,
                File = new FileRef
                {
                    FileData = base64File,
                    MimeType = mimeType
                }
            };
        }


        public static FileBase64ContentPart FromId(string fileId)
        {
            return new FileBase64ContentPart
            {
                Type = ContentPartType.File,
                File = new FileRef
                {
                    FileId = fileId,
                }
            };
        }
    }

    public class FileRef
    {
        /// <summary>
        /// Optional. The base64 encoded file data, used when passing the file to the model as a string.
        /// </summary>
        [JsonProperty("file_data")] public string FileData { get; set; }

        /// <summary>
        /// Optional. The ID of an uploaded file to use as input.
        /// </summary>
        [JsonProperty("file_id")] public string FileId { get; set; }

        /// <summary>
        /// Optional. The name of the file, used when passing the file to the model as a string.
        /// </summary>
        [JsonProperty("filename")] public string Filename { get; set; }

        /// <summary>
        /// The specific quote in the file.
        /// </summary>
        [JsonProperty("quote")] public string Quote { get; set; }


        [JsonIgnore]
        public MIMEType MimeType
        {
            get
            {
                if (_mimeType == null)
                {
                    _mimeType = MIMEType.Unknown;
                    if (Filename != null) _mimeType = MIMETypeUtil.ParseFromPath(Filename);
                }
                return _mimeType.Value;
            }
            set => _mimeType = value;
        }

        private MIMEType? _mimeType;
    }
}