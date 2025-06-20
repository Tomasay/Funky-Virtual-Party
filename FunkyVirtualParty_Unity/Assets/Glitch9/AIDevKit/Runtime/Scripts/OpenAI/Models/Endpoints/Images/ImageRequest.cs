using System.Runtime.CompilerServices;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    public class ImageCreationRequest : ImageRequest<ImageCreationRequest> { }
    public class ImageEditRequest : ImageRequest<ImageEditRequest> { }
    public class ImageVariationRequest : ImageRequest<ImageVariationRequest> { }

    /// <summary>
    /// Creates an image given a prompt: https://api.openai.com/v1/images/generations
    /// Creates an edited or extended image given an original image and a prompt: https://api.openai.com/v1/images/edits
    /// Creates a variation of a given image: https://api.openai.com/v1/images/variations
    /// </summary>
    public abstract class ImageRequest<T> : AIRequest where T : ImageRequest<T>
    {
        /// <summary>
        /// [Required for Variation Request]
        /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
        /// If mask is not provided, image must have transparency, which will be used as the mask.
        /// </summary>
        [JsonProperty("image")] public File<Texture2D> Image { get; set; }

        /// <summary>
        /// [Optional] An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
        /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        [JsonIgnore, JsonProperty("mask")] public File<Texture2D> Mask { get; set; }

        /// <summary>
        /// [Required for Request and Edit Request]
        /// A Text description of the desired image(s).
        /// The maximum length is 1000 characters for dall-e-2 and 4000 characters for dall-e-3.
        /// </summary>
        [JsonProperty("prompt")] public string Prompt { get; set; }

        /// <summary>
        /// The size of the generated images.
        /// Must be one of 256x256, 512x512, or 1024x1024 for dall-e-2.
        /// Must be one of 1024x1024, 1792x1024, or 1024x1792 for dall-e-3 models.
        /// </summary>
        /// <remarks>
        /// Defaults to 1024x1024
        /// </remarks>
        [JsonProperty("size")] public ImageSize? Size { get; set; }

        /// <summary>
        /// The quality of the image that will be generated.
        /// <see cref="ImageQuality.HighDefinition"/> creates images with finer details and greater consistency across the image.
        /// This param is only supported for <see cref="OpenAIModel.DallE3"/>.
        /// </summary>
        /// <remarks>Defaults to <see cref="ImageQuality.Standard"/> </remarks>
        [JsonProperty("quality")] public ImageQuality? Quality { get; set; }

        /// <summary>
        /// The style of the generated images.
        /// Vivid causes the model to lean towards generating hyper-real and dramatic images.
        /// Natural causes the model to produce more natural, less hyper-real looking images.
        /// This param is only supported for <see cref="OpenAIModel.DallE3"/>.
        /// </summary>
        /// <remarks> Defaults to <see cref="ImageStyle.Vivid"/> </remarks>
        [JsonProperty("style")] public ImageStyle? Style { get; set; }

        /// <summary>
        /// The format of the response. 
        /// </summary>
        [JsonProperty("response_format")] public ImageFormat? ResponseFormat { get; set; }

        public class Builder : ModelRequestBuilder<Builder, T>
        {
            public Builder SetResponseFormat(ImageFormat format)
            {
                _req.ResponseFormat = format;
                return this;
            }

            public Builder SetPrompt(string prompt)
            {
                _req.Prompt = prompt;
                return this;
            }

            public Builder SetQuality(ImageQuality quality)
            {
                _req.Quality = quality;
                return this;
            }

            public Builder SetStyle(ImageStyle style)
            {
                _req.Style = style;
                return this;
            }

            public Builder SetSize(ImageSize size)
            {
                _req.Size = size;
                return this;
            }

            public Builder SetImage(File<Texture2D> image)
            {
                _req.Image = image;
                return this;
            }

            public Builder SetImage(Texture2D texture)
            {
                _req.Image = new File<Texture2D>(texture, $"@{texture.name}.png");
                return this;
            }

            public Builder SetImage(Sprite sprite)
            {
                return SetImage(sprite.texture);
            }

            public Builder SetMask(File<Texture2D> mask)
            {
                _req.Mask = mask;
                return this;
            }

            public Builder SetMask(Texture2D texture)
            {
                _req.Mask = new File<Texture2D>(texture, $"@{texture.name}.png");
                return this;
            }

            public Builder SetMask(Sprite sprite)
            {
                return SetMask(sprite.texture);
            }

            public override T Build([CallerFilePath] string sender = "")
            {
                bool isDalle2 = _req.Model.IsDallE2();
                if (isDalle2)
                {
                    _req.Quality = null;
                    _req.Style = null;
                }

                if (_req.Image == null) return base.Build(sender);
                return base.Build(MIMEType.MultipartForm, sender);
            }
        }
    }
}
