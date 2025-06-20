using Newtonsoft.Json;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum ContentPartType
    {
        [ApiEnum("text")] Text,
        [ApiEnum("image_url")] ImageUrl,
        [ApiEnum("image_file")] ImageFile,  // Added 2024.05.24 
        [ApiEnum("file")] File,             // Added 2025.04.27
        [ApiEnum("input_audio")] Audio,     // Added 2025.04.27
    }

    public abstract class ContentPart
    {
        /// <summary>
        /// Required. The type of the content part.
        /// </summary>
        [JsonProperty("type")] public ContentPartType Type { get; set; }

        private bool? _isBase64;
        [JsonIgnore]
        public bool IsBase64
        {
            get
            {
                if (_isBase64 == null)
                {
                    if (this is ImageBase64ContentPart)
                    {
                        _isBase64 = true;
                    }
                    else if (this is FileBase64ContentPart)
                    {
                        _isBase64 = true;
                    }
                    else if (this is AudioBase64ContentPart)
                    {
                        _isBase64 = true;
                    }
                    else if (this is ImageUrlContentPart imageUrlPart)
                    {
                        _isBase64 = imageUrlPart.Image?.IsBase64 ?? false;
                    }
                    else
                    {
                        _isBase64 = false;
                    }
                }

                return _isBase64.Value;
            }
        }
    }
}