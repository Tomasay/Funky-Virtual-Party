using System;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit
{
    [JsonConverter(typeof(TextContentPartConverter))]
    public class TextContentPart : ContentPart
    {
        [JsonProperty("text")] public TextRef Text;
        public override string ToString() => Text?.Value ?? string.Empty;

        public TextContentPart() { Type = ContentPartType.Text; }
        public TextContentPart(string text) : this()
        {
            Text = new TextRef(text);
        }
    }

    public class TextRef
    {
        public static implicit operator TextRef(string text) => new(text);

        /// <summary>
        /// The data that makes up the Text.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The annotations that are associated with the text.
        /// </summary>
        public Annotation[] Annotations { get; set; }

        public TextRef(string text)
        {
            Value = text;
        }

        public TextRef(string text, params Annotation[] annotations)
        {
            Value = text;
            Annotations = annotations;
        }

        [JsonConstructor]
        public TextRef() { }
    }


    public enum AnnotationType
    {
        [ApiEnum("file_citation")] FileCitation,
        [ApiEnum("file_path")] FilePath,
    }

    [JsonConverter(typeof(AnnotationConverter))]
    public class Annotation
    {
        /// <summary>
        /// The type of annotation.
        /// </summary>
        [JsonProperty("type")] public AnnotationType Type { get; set; }
        [JsonProperty("file_citation")] public FileRef FileCitation { get; set; }
        [JsonProperty("file_path")] public FileRef FilePath { get; set; }

        /// <summary>
        /// The start index of the annotation.
        /// </summary>
        [JsonProperty("start_index")] public int StartIndex { get; set; }

        /// <summary>
        /// The end index of the annotation.
        /// </summary>
        [JsonProperty("end_index")] public int EndIndex { get; set; }

        /// <summary>
        /// The value of the annotation.
        /// </summary>
        [JsonProperty("value")] public string Value { get; set; }
    }

    public class AnnotationConverter : JsonConverter<Annotation>
    {
        public override void WriteJson(JsonWriter writer, Annotation value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue(value.Type.ToApiValue());
            writer.WritePropertyName("start_index");
            writer.WriteValue(value.StartIndex);
            writer.WritePropertyName("end_index");
            writer.WriteValue(value.EndIndex);
            writer.WritePropertyName("value");
            writer.WriteValue(value.Value);

            if (value.FileCitation != null)
            {
                writer.WritePropertyName("file_citation");
                serializer.Serialize(writer, value.FileCitation);
            }

            if (value.FilePath != null)
            {
                writer.WritePropertyName("file_path");
                serializer.Serialize(writer, value.FilePath);
            }

            writer.WriteEndObject();
        }

        public override Annotation ReadJson(JsonReader reader, System.Type objectType, Annotation existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException("Expected StartObject token.");
            }

            JObject jObject = JObject.Load(reader);
            Annotation annotation = new Annotation();

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "type":
                        annotation.Type = ApiEnumUtil.Parse<AnnotationType>(property.Value.Value<string>());
                        break;
                    case "start_index":
                        annotation.StartIndex = property.Value.Value<int>();
                        break;
                    case "end_index":
                        annotation.EndIndex = property.Value.Value<int>();
                        break;
                    case "value":
                        annotation.Value = property.Value.Value<string>();
                        break;
                    case "file_citation":
                        annotation.FileCitation = property.Value.ToObject<FileRef>(serializer);
                        break;
                    case "file_path":
                        annotation.FilePath = property.Value.ToObject<FileRef>(serializer);
                        break;
                }
            }

            return annotation;
        }
    }

    public class TextContentPartConverter : JsonConverter<TextContentPart>
    {
        public override void WriteJson(JsonWriter writer, TextContentPart value, JsonSerializer serializer)
        {
            //GNDebug.Mark($"Writing JSON for TextContentPart, value: {value}");
            // if (value == null || value.Text == null)
            // {
            //     // writer.WriteNull();
            //     return;
            // }

            // if (value.Text.Annotations == null)
            // {
            //     // Simple string
            //     writer.WriteValue(value.Text.Value);
            // }
            // else
            // {
            //     writer.WriteStartObject();
            //     writer.WritePropertyName("text");
            //     writer.WriteStartObject();
            //     writer.WritePropertyName("value");
            //     writer.WriteValue(value.Text.Value);
            //     writer.WritePropertyName("annotations");
            //     serializer.Serialize(writer, value.Text.Annotations);
            //     writer.WriteEndObject(); // end of text
            //     writer.WriteEndObject(); // end of part
            // }
            //obj["type"] = value.Type.ToApiValue();

            ContentPartType type = value?.Type ?? ContentPartType.Text;
            string text = value?.Text?.Value ?? "-empty-";

            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(type.ToApiValue());

            writer.WritePropertyName("text");

            if (value?.Text?.Annotations == null)
            {
                // Simple string
                writer.WriteValue(text);
            }
            else
            {
                writer.WriteStartObject();

                writer.WritePropertyName("value");
                writer.WriteValue(text);

                writer.WritePropertyName("annotations");
                serializer.Serialize(writer, value.Text.Annotations);

                writer.WriteEndObject(); // end of "text" object
            }

            writer.WriteEndObject(); // end of entire part
        }

        public override TextContentPart ReadJson(JsonReader reader, Type objectType, TextContentPart existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //GNDebug.Mark($"Reading JSON for TextContentPartConverter, token type: {reader.TokenType}");
            if (reader.TokenType == JsonToken.String)
            {
                return new TextContentPart((string)reader.Value);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject jObject = JObject.Load(reader);

                // Case 1: flat {"text": "hello"}
                if (jObject.TryGetValue("text", out JToken textToken))
                {
                    //GNDebug.Mark($"Text token detected: {textToken.Type}");

                    if (textToken.Type == JTokenType.String)
                    {
                        return new TextContentPart(textToken.Value<string>());
                    }

                    // Case 2: nested {"text": { "value": ..., "annotations": [...] }}
                    if (textToken.Type == JTokenType.Object)
                    {
                        string value = textToken["value"]?.Value<string>();
                        Annotation[] annotations = textToken["annotations"]?.ToObject<Annotation[]>(serializer);
                        return new TextContentPart
                        {
                            Text = new TextRef(value, annotations)
                        };
                    }
                }
            }

            throw new JsonSerializationException("Invalid format for TextContentPart.");
        }
    }

}
