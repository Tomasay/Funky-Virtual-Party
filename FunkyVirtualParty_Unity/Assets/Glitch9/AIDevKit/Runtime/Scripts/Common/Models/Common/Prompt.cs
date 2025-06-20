
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    public class Prompt
    {
        public static implicit operator string(Prompt prompt) => prompt?.formattedText;
        public static implicit operator Prompt(string prompt) => new(prompt);

        [JsonIgnore] public string text;
        [JsonProperty("text")] public string formattedText;

        public Prompt(string prompt, string formattedPrompt = null)
        {
            text = prompt;
            formattedText = formattedPrompt ?? prompt;
        }
    }

    public class WeightedPrompt : Prompt
    {
        [JsonProperty("weight")] public float? weight;

        public WeightedPrompt(string prompt, float? weight = null, string formattedPrompt = null) : base(prompt, formattedPrompt)
        {
            this.weight = weight;
        }
    }

    /// <summary>
    /// A specialized prompt for inpainting tasks.<br/>
    /// This class is used to pass the instruction and the image to the inpainting model for
    /// <see cref="GENInpaintTask"/>.<br/>
    /// </summary>
    public class InpaintPrompt : Prompt
    {
        /// <summary>
        /// The image to be edited.<br/>
        /// </summary>
        public Texture2D image;

        /// <summary>
        /// Optional. 
        /// The mask to be used for inpainting.<br/>
        /// The mask should be a black and white image where the white area is the area to be edited.<br/>
        /// </summary>
        public Texture2D mask;

        public bool IsValid => image != null && !string.IsNullOrEmpty(text);

        public InpaintPrompt(string instruction, Texture2D image, Texture2D mask = null, string formattedInstruction = null) : base(instruction, formattedInstruction)
        {
            this.image = image;
            this.mask = mask;
        }
    }
}