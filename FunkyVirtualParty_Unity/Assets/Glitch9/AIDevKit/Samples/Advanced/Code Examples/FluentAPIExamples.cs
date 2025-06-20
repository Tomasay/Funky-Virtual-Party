using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.IO.Json.Schema;
using UnityEngine;

namespace Glitch9.AIDevKit.Examples
{
    public class FluentAPIExamples
    {
        public async void GENTextExample()
        {
            // simple text generation
            string text = await "Generate some random texts."
                .GENResponse()
                .SetModel(OpenAIModel.GPT4o)
                .SetInstruction("Generate a story about a cat.")
                .ExecuteAsync();

            // stream
            static void onTextReceived(string text)
                => Debug.Log(text);

            await "Generate some random texts."
                .GENResponse()
                .SetModel(OpenAIModel.GPT4o)
                .SetInstruction("Generate a story about a cat.")
                .OnStreamText(onTextReceived)
                .StreamAsync();
        }

        /// <summary>
        /// Sample class for structured output using OpenAIJsonSchemaResponseAttribute.
        /// </summary>
        [StrictJsonSchema("cat", Description = "A cat object.", Strict = true)]
        public class Cat
        {
            /*
            [IMPORTANT] If "Strict" is set to true, ALL properties must have "Required" set to true.
            */

            [JsonSchemaProperty("name", Description = "The name of the cat.", Required = true)]
            public string Name { get; set; }

            [JsonSchemaProperty("age", Description = "The age of the cat.", Required = true)]
            public int Age { get; set; }

            [JsonSchemaProperty("color", Description = "The color of the cat.", Required = true)]
            public string Color { get; set; }
        }

        public async void GENObjectExample()
        {
            // simple text generation with structured output
            Cat cat = await "Generate a random cat object."
                .GENStruct<Cat>()
                .SetModel(OpenAIModel.GPT4o)
                .ExecuteAsync();
        }

        public async void GENImageExample()
        {
            Sprite sprite = await "Generate some random image."
                .GENImage()
                .SetModel(OpenAIModel.DallE3)
                .ExecuteAsync();

            Texture2D texture = await "Generate some random image."
                .GENImage()
                .SetModel(OpenAIModel.DallE3)
                .ExecuteAsync();

            Texture2D[] textures = await "Generate some random images."
                .GENImage()
                .SetCount(4)
                .SetModel(OpenAIModel.DallE2)
                .ExecuteAsync();
        }

        public async void GENInpaintExample()
        {
            Texture2D sourceImage = null; // Assuming you have a source image to edit

            Sprite sprite = await sourceImage
                .GENInpaint("Add a cat to this image.")
                .SetModel(GoogleModel.Gemini2_0_Flash_Exp_Image_Generation)
                .ExecuteAsync();

            Texture2D texture = await sourceImage
                .GENInpaint("Add a cat to this image.")
                .SetModel(GoogleModel.Gemini2_0_Flash_Exp_Image_Generation)
                .ExecuteAsync();
        }

        // public async void GENImageVariationExample()
        // {
        //     Texture2D sourceImage = null; // Assuming you have a source image to edit

        //     Sprite sprite = await sourceImage
        //         .GENVariation()
        //         .SetModel(OpenAIModel.DallE2)
        //         .ExecuteAsync();

        //     Texture2D texture = await sourceImage
        //         .GENVariation()
        //         .SetModel(OpenAIModel.DallE2)
        //         .ExecuteAsync();
        // }

        public async void GENSpeechExample()
        {
            AudioClip audioClip = await "Where is my cat?"
                .GENSpeech()
                .SetModel(OpenAIModel.TTS1)
                .SetVoice(OpenAIVoice.Fable)
                .ExecuteAsync();
        }

        public async void GENTranscriptExample()
        {
            AudioClip audioClip = null; // Assuming you have an audio clip to recognize speech from

            string text = await audioClip
                .GENTranscript()
                .SetModel(OpenAIModel.Whisper1)
                .SetLanguage(SystemLanguage.English)
                .ExecuteAsync();
        }

        public async void GENSequenceExample()
        {
            var sequence = new GENSequence()
                .AppendText("Generate some text to use for image generation.".GENResponse())
                .AppendTextToImage((text) => text.GENImage());

            await sequence.ExecuteAsync();
        }
    }
}