using System;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit
{
    public enum ImageDetail
    {
        /// <summary>
        /// Default value
        /// </summary>
        [ApiEnum("auto")] Auto,

        /// <summary>
        /// Uses fewer tokens
        /// </summary>
        [ApiEnum("low")] Low,

        /// <summary>
        /// High resolution
        /// </summary>
        [ApiEnum("high")] High,
    }

    public class ImageUrlContentPart : ImageContentPart { }
    public class ImageBase64ContentPart : ImageContentPart { }
    public class ImageFileIdContentPart : ImageContentPart { }

    [JsonConverter(typeof(ImageContentPartConverter))]
    public class ImageContentPart : ContentPart
    {
        public ImageRef Image { get; set; }


        public static ImageUrlContentPart FromUrl(string imageUrl, ImageDetail? detail = null)
        {
            return new ImageUrlContentPart
            {
                Type = ContentPartType.ImageUrl,
                Image = new ImageRef
                {
                    Url = imageUrl,
                    Detail = detail
                }
            };
        }

        public static ImageBase64ContentPart FromBase64(string base64Image, ImageDetail? detail = null)
        {
            // url format: data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA... (base64 string)
            return new ImageBase64ContentPart
            {
                Type = ContentPartType.ImageUrl,
                Image = new ImageRef
                {
                    FileData = base64Image,
                    Detail = detail
                }
            };
        }

        public static ImageFileIdContentPart FromId(string fileId, ImageDetail? detail = null)
        {
            return new ImageFileIdContentPart
            {
                Type = ContentPartType.ImageFile,
                Image = new ImageRef
                {
                    Url = fileId,
                    Detail = detail
                }
            };
        }
    }

    public class ImageRef : FileRef
    {
        /// <summary>
        /// Either a URL of the image or the base64 encoded image data.
        /// </summary>
        [JsonProperty("url")] public string Url { get; set; }

        /// <summary>
        /// Specifies the detail level of the image.
        /// low uses fewer tokens, you can opt in to high resolution using high.
        /// Default value is auto
        /// </summary>
        [JsonProperty("detail")] public ImageDetail? Detail { get; set; }

        [JsonIgnore] public bool IsBase64 => Url?.StartsWith("data:image/png;base64,") == true;
    }

    public class ImageContentPartConverter : JsonConverter<ImageContentPart>
    {
        private enum ImageContentPartType
        {
            Url,
            FileId,
            //Base64,
        }

        public override ImageContentPart ReadJson(JsonReader reader, Type objectType, ImageContentPart existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // reading is always a UrlImageContentPart 
            ImageContentPartType type = ImageContentPartType.Url;
            ImageRef imageRef = new();

            //GNDebug.Mark($"Reading JSON for ImageContentPartConverter, token type: {reader.TokenType}");

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value.ToString() == "image_file")
                    {
                        type = ImageContentPartType.FileId;
                        reader.Read(); // Advance to StartObject

                        while (reader.TokenType != JsonToken.EndObject)
                        {
                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                string propertyName = reader.Value.ToString();
                                reader.Read(); // Advance to value

                                if (propertyName == "detail")
                                {
                                    imageRef.Detail = serializer.Deserialize<ImageDetail>(reader);
                                }
                                else if (propertyName == "file_id")
                                {
                                    imageRef.FileId = reader.Value?.ToString();
                                }
                                else if (propertyName == "quote")
                                {
                                    imageRef.Quote = reader.Value?.ToString();
                                }
                                else if (propertyName == "filename")
                                {
                                    imageRef.Filename = reader.Value?.ToString();
                                }
                            }
                            else
                            {
                                reader.Read();
                            }
                        }
                    }
                    else if (reader.Value.ToString() == "image_url")
                    {
                        type = ImageContentPartType.Url;
                        reader.Read(); // Advance to StartObject

                        while (reader.TokenType != JsonToken.EndObject)
                        {
                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                string propertyName = reader.Value.ToString();
                                reader.Read(); // Advance to value

                                if (propertyName == "detail")
                                {
                                    imageRef.Detail = serializer.Deserialize<ImageDetail>(reader);
                                }
                                else if (propertyName == "url")
                                {
                                    imageRef.Url = reader.Value?.ToString();
                                }
                            }
                            else
                            {
                                reader.Read();
                            }
                        }
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            if (type == ImageContentPartType.FileId)
            {
                return new ImageFileIdContentPart
                {
                    Type = ContentPartType.ImageFile,
                    Image = imageRef
                };
            }
            else if (type == ImageContentPartType.Url)
            {
                if (imageRef.IsBase64)
                {
                    return new ImageBase64ContentPart
                    {
                        Type = ContentPartType.ImageUrl,
                        Image = imageRef
                    };
                }
                else
                {
                    return new ImageUrlContentPart
                    {
                        Type = ContentPartType.ImageUrl,
                        Image = imageRef
                    };
                }
            }

            throw new JsonSerializationException("Unexpected token type: " + reader.TokenType);
        }

        public override void WriteJson(JsonWriter writer, ImageContentPart value, JsonSerializer serializer)
        {
            if (value == null || value.Image == null)
            {
                //writer.WriteNull();
                return;
            }


            string imageKey = value.Type == ContentPartType.ImageFile ? "image_file" : "image_url";

            JObject obj = new() { [imageKey] = new JObject() };

            obj["type"] = value.Type.ToApiValue();

            if (value is ImageFileIdContentPart fileIdPart)
            {
                obj[imageKey]["file_id"] = fileIdPart.Image.FileId;
            }
            else if (value is ImageUrlContentPart urlPart)
            {
                obj[imageKey]["url"] = urlPart.Image.Url;
            }
            else if (value is ImageBase64ContentPart base64Part)
            {
                // url format: data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA... (base64 string) 
                obj[imageKey]["url"] = $"data:image/png;base64,{base64Part.Image.FileData}";
            }
            else
            {
                throw new NotSupportedException($"Unsupported ImageContentPart type: {value.GetType()}");
            }

            if (value.Image.Detail.HasValue)
            {
                obj[imageKey]["detail"] = value.Image.Detail.Value.ToString().ToLowerInvariant();
            }



            obj.WriteTo(writer);
        }
    }
}