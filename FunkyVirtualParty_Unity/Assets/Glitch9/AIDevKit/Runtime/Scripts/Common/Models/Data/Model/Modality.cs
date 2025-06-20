using System;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// "Modality" refers to the type or form of data that a model is designed to process,
    /// either as input or output. In AI and machine learning contexts, modality describes 
    /// the nature of the information being handled — such as text, image, audio, or video.
    /// 
    /// For example:
    /// - A text-to-text model like GPT-4 processes text inputs and generates text outputs.
    /// - A text-to-image model like DALL·E takes text prompts and produces images.
    /// - A multimodal model like Gemini can process multiple types of data simultaneously, 
    ///   such as combining text and image inputs.
    /// 
    /// The concept of modality helps categorize models based on the kinds of sensory or 
    /// informational data they handle, and is especially important for understanding 
    /// the capabilities and limitations of a model.
    /// </summary>
    [Flags]
    public enum Modality
    {
        [ApiEnum("Text", "text")] Text = 1 << 0,
        [ApiEnum("Image", "image")] Image = 1 << 1,
        [ApiEnum("File", "file")] File = 1 << 2,
        [ApiEnum("Audio", "audio")] Audio = 1 << 3,
        [ApiEnum("Video", "video")] Video = 1 << 4,
        [ApiEnum("Text Embedding", "text_embedding")] TextEmbedding = 1 << 5,
    }

    internal static class ModalityExtensions
    {
        internal static List<string> ToStringList(this Modality? flags)
        {
            List<string> result = new();
            if (flags == null) return result;

            foreach (Modality modality in Enum.GetValues(typeof(Modality)))
            {
                if (modality != 0 && flags.Value.HasFlag(modality))
                {
                    result.Add(modality.ToString().ToLowerInvariant());
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Json converter to parse InputModality to List<string>.
    /// This is used to convert the InputModality enum to a list of strings for serialization.
    /// (BitFlag를 string[]로 변환 / OpenRouter API등에서 사용)
    /// </summary>
    internal class ModalityStringListConverter : JsonConverter<Modality>
    {
        public override Modality ReadJson(JsonReader reader, Type objectType, Modality existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var modalities = new Modality();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;

                    var modality = reader.Value.ToString().ToLowerInvariant();
                    if (Enum.TryParse(typeof(Modality), modality, true, out var result))
                    {
                        modalities |= (Modality)result;
                    }
                }
                return modalities;
            }

            return (Modality)Enum.Parse(typeof(Modality), reader.Value.ToString(), true);
        }

        public override void WriteJson(JsonWriter writer, Modality value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (Modality modality in Enum.GetValues(typeof(Modality)))
            {
                if (value.HasFlag(modality) && modality != 0)
                {
                    writer.WriteValue(modality.ToString().ToLowerInvariant());
                }
            }

            writer.WriteEndArray();
        }
    }
}