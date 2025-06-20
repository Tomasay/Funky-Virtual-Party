using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// An array of content parts with a defined type, 
    /// each can be of type Text or Image_Url when passing in images. 
    /// You can pass multiple images by adding multiple Image_Url content parts. 
    /// Image input is only supported when using the gpt-4-visual-preview model.
    /// </summary> 
    [JsonConverter(typeof(ContentPartWrapperConverter))]
    public class ContentPartWrapper
    {
        public static implicit operator ContentPartWrapper(string text) => new(text);
        public static implicit operator string(ContentPartWrapper content) => new(content.ToString());

        public ContentPartType Type => _part?.Type ?? ContentPartType.Text;
        private readonly ContentPart _part;
        private readonly string _text;
        private string _textCache;

        [JsonConstructor] public ContentPartWrapper() { }
        public ContentPartWrapper(string text) => _text = text;
        public ContentPartWrapper(ContentPart part) => _part = part;

        public bool HasValue => _text != null || _part != null;
        public bool IsString => _part == null;
        public bool IsNull => !HasValue;
        public bool IsBase64 => _part?.IsBase64 ?? false;


        public override string ToString()
        {
            if (_textCache == null)
            {
                if (_text != null)
                {
                    _textCache = _text;
                }
                else if (_part is TextContentPart textContentPart)
                {
                    _textCache = textContentPart.ToString();
                }
                else if (_part is ImageUrlContentPart imageContentPart)
                {
                    _textCache = imageContentPart.Image?.Url?.ToString() ?? string.Empty;
                }
                else if (_part is ImageFileIdContentPart imageFilePart)
                {
                    _textCache = imageFilePart.Image?.FileId?.ToString() ?? string.Empty;
                }
                else
                {
                    _textCache = string.Empty;
                }
            }

            return _textCache;
        }

        public ContentPart ToPart() => _part;
    }

    internal class ContentPartWrapperConverter : JsonConverter<ContentPartWrapper>
    {
        public override void WriteJson(JsonWriter writer, ContentPartWrapper value, JsonSerializer serializer)
        {
            AIDevKitDebug.Mark("Writing JSON for ContentPartWrapperConverter");

            if (!value.HasValue)
            {
                writer.WriteValue("-empty-");
                return;
            }

            if (value.IsString)
            {
                writer.WriteValue(value.ToString());
                return;
            }

            var part = value.ToPart();

            writer.WriteStartObject();

            // Write type first
            writer.WritePropertyName("type");
            writer.WriteValue(part.Type.ToString());

            // Then delegate rest to the serializer (merge approach)
            JObject obj = JObject.FromObject(part, serializer);
            foreach (var property in obj.Properties())
            {
                if (property.Name == "type") continue; // already written
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }


        public override ContentPartWrapper ReadJson(JsonReader reader, Type objectType, ContentPartWrapper existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // log api
            JToken token = JToken.Load(reader);

            //GNDebug.Mark("Reading JSON for ContentPartWrapperConverter, token type: " + token.Type);

            if (token.Type == JTokenType.String)
            {
                // GNDebug.Mark($"READ: String token detected: {token}");
                return new ContentPartWrapper(token.ToObject<string>());
            }
            else if (token.Type == JTokenType.Object)
            {
                //GNDebug.Mark(1);

                // GNDebug.Mark($"READ: Object token detected: {token}");
                if (token["type"] == null)   // Check if the token is an object and has a "type" property
                {
                    //GNDebug.Mark(2);
                    Debug.LogError("ChatContentPart: type property is missing in the object: " + token);
                    return null;
                }

                //GNDebug.Mark(3);
                ContentPartType type = token["type"].ToObject<ContentPartType>(serializer);

                // AIDevKitDebug.Mark($"READ: Object token detected: {token}");
                // AIDevKitDebug.Mark($"Type: {type}");

                if (type == ContentPartType.Text)
                {
                    TextContentPart textPart = token.ToObject<TextContentPart>(serializer);

                    if (textPart == null)
                    {
                        Debug.LogError("ChatContentPart: Failed to deserialize TextContentPart.");
                        return null;
                    }

                    return new ContentPartWrapper(textPart);
                }

                if (type == ContentPartType.ImageUrl)
                {
                    ImageContentPart imagePart = token.ToObject<ImageContentPart>(serializer);

                    if (imagePart == null)
                    {
                        Debug.LogError("ChatContentPart: Failed to deserialize ImageContentPart.");
                        return null;
                    }


                    return new ContentPartWrapper(imagePart);
                }

                if (type == ContentPartType.ImageFile)
                {
                    ImageFileIdContentPart imageFilePart = token.ToObject<ImageFileIdContentPart>(serializer);

                    if (imageFilePart == null)
                    {
                        Debug.LogError("ChatContentPart: Failed to deserialize ImageFileIdContentPart.");
                        return null;
                    }

                    return new ContentPartWrapper(imageFilePart);
                }

                AIDevKitDebug.Mark($"READ: Unknown token type detected: {type}");
            }

            //GNDebug.Mark($"READ: Unknown token type detected: {token.Type}");
            throw new JsonSerializationException("Unexpected token type when parsing ChatContentPart.");
        }
    }
}