using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// The base structured datatype containing multi-part content of a message.
    /// It's a <see cref="ChatMessage"/> in a demonic form. 
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Ordered Parts that constitute a single message. Parts may have different MIME types.
        /// </summary>
        [JsonProperty("parts")] public ContentPart[] Parts { get; set; }

        /// <summary>
        /// Optional.
        /// The producer of the content. Must be either <see cref="ChatRole.User"/> or <see cref="ChatRole.Model"/>.
        /// Useful to set for multi-turn conversations, otherwise can be left blank or unset.
        /// </summary>
        [JsonProperty("role")] public ChatRole Role { get; set; }

        public Content() { }

        public Content(ChatRole role, ContentPart[] parts)
        {
            Parts = parts;
            Role = role;
        }

        public Content(ChatRole role, string text, Blob inlineData = null)
        {
            Parts = new ContentPart[] { new() { Text = text, InlineData = inlineData } };
            Role = role;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A datatype containing media that is part of a multi-part <see cref="Content"/> message.
    /// <para>
    /// A <see cref="ContentPart"/> consists of data which has an associated datatype.A <see cref="ContentPart"/> can only contain one of the accepted types in <see cref="ContentPart.Data"/>.
    /// </para>
    /// <para>
    /// A <see cref="ContentPart"/> must have a fixed IANA MIME type identifying the type and subtype of the media if the  <see cref="ContentPart.InlineData"/> field is filled with raw bytes.
    /// </para>
    /// </summary>
    public class ContentPart
    {
        /// <summary>
        /// Inline text.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Inline media bytes.
        /// </summary>
        [JsonProperty("inlineData")] public Blob InlineData { get; set; }

        /// <summary>
        /// A predicted FunctionCall returned from the model that contains a string
        /// representing the <see cref="FunctionDeclaration.Name"/> with the arguments and their values.
        /// </summary>
        [JsonProperty("functionCall")] public FunctionCall FunctionCall { get; set; }

        /// <summary>
        /// The result output of a FunctionCall that contains a string representing the <see cref="FunctionDeclaration.Name"/>
        /// and a structured JSON object containing any output from the function is used as context to the model.
        /// </summary>
        [JsonProperty("functionResponse")] public FunctionResponse FunctionResponse { get; set; }

        /// <summary>
        /// URI based data.
        /// </summary>
        [JsonProperty("fileData")] public GoogleFileData FileData { get; set; }

        [JsonIgnore] public bool IsImage => InlineData?.IsImage ?? false;
        [JsonIgnore] public File<Texture2D> ImageContent { get; set; } //TODO:


        public ContentPart() { }
        public ContentPart(FunctionResponse response) => FunctionResponse = response;


        public static ContentPart FromText(string text)
        {
            return new ContentPart { Text = text };
        }

        public static ContentPart FromBase64(string base64File, MIMEType mimeType)   // url is the network url that you uploaded or something
        {
            return new ContentPart
            {
                InlineData = new Blob
                {
                    MimeType = mimeType,
                    Data = base64File
                }
            };
        }

        public static ContentPart FromUrl(string fileUrl)   // url is the network url that you uploaded or something
        {
            return new ContentPart
            {
                FileData = new GoogleFileData(fileUrl)
            };
        }
    }

    /// <summary>
    /// Interface for sending an image.
    /// </summary>
    public class Blob
    {
        /// <summary>
        /// MimeType of Image
        /// </summary>
        [JsonProperty("mimeType")] public MIMEType MimeType { get; set; }

        /// <summary>
        /// Image as a base64 string.
        /// </summary>
        [JsonProperty("data")] public string Data { get; set; }

        [JsonIgnore] public bool IsImage => MimeType.IsImage();
    }
}
