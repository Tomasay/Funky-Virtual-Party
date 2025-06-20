using System;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Generate image(s) from a prompt using the Imagen 3 model, 
    /// or generate a video from a prompt using the Veo model.
    /// Both Imagen 3 and Veo models are only available on the google paid tier.
    /// </summary>
    [JsonConverter(typeof(PredictionRequestConverter))]
    public class PredictionRequest : GenerativeAIRequest
    {
        [JsonProperty("prompt")] public string Prompt { get; set; }
        [JsonProperty("config")] public GenerateMediaConfig Config { get; set; } = new();

        public override void ValidateBody()
        {
            if (string.IsNullOrEmpty(Prompt))
                throw new ArgumentNullException(nameof(Prompt), "Prompt cannot be null or empty.");

            if (Config.NumberOfImages < 1 || Config.NumberOfImages > 4)
                throw new ArgumentOutOfRangeException(nameof(Config.NumberOfImages), "Number of images must be between 1 and 4.");
        }

        public class Builder : GenerativeAIRequestBuilder<Builder, PredictionRequest>
        {
            public Builder SetPrompt(string prompt)
            {
                _req.Prompt = prompt;
                return this;
            }

            public Builder SetNumberOfImages(int numberOfImages)
            {
                _req.Config.NumberOfImages = numberOfImages;
                return this;
            }

            public Builder SetAspectRatio(AspectRatio aspectRatio)
            {
                _req.Config.AspectRatio = aspectRatio;
                return this;
            }

            public Builder SetPersonGeneration(PersonGeneration personGeneration)
            {
                _req.Config.PersonGeneration = personGeneration;
                return this;
            }
        }
    }

    public class GenerateMediaConfig
    {
        /// <summary>
        /// The number of images to generate, from 1 to 4 (inclusive). 
        // The default is 4.
        /// </summary> 
        public int NumberOfImages { get; set; } = 4;

        /// <summary>
        /// Changes the aspect ratio of the generated image. 
        /// Supported values are "1:1", "3:4", "4:3", "9:16", and "16:9". The default is "1:1".
        /// </summary> 
        public AspectRatio AspectRatio { get; set; } = AspectRatio.Square;

        /// <summary>
        /// Allow the model to generate images of people.
        /// </summary> 
        public PersonGeneration PersonGeneration { get; set; } = PersonGeneration.AllowAdult;
    }

    public class PredictionRequestConverter : JsonConverter<PredictionRequest>
    {
        public override PredictionRequest ReadJson(JsonReader reader, Type objectType, PredictionRequest existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization is not implemented for GenerateImagesRequest.");
        }

        public override void WriteJson(JsonWriter writer, PredictionRequest value, JsonSerializer serializer)
        {
            /*
            curl -X POST \
            "https://generativelanguage.googleapis.com/v1beta/models/imagen-3.0-generate-002:predict?key=GEMINI_API_KEY" \
            -H "Content-Type: application/json" \
            -d '{
                "instances": [
                {
                    "prompt": "Fuzzy bunnies in my kitchen"
                }
                ],
                "parameters": {
                "sampleCount": 4
                }
            }'
            */

            writer.WriteStartObject();
            writer.WritePropertyName("instances");
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WritePropertyName("prompt");
            writer.WriteValue(value.Prompt);
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WritePropertyName("parameters");
            writer.WriteStartObject();
            writer.WritePropertyName("sampleCount");
            writer.WriteValue(value.Config.NumberOfImages);
            writer.WritePropertyName("aspectRatio");
            writer.WriteValue(value.Config.AspectRatio.ToApiValue());
            writer.WritePropertyName("personGeneration");
            writer.WriteValue(value.Config.PersonGeneration.ToApiValue());
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}