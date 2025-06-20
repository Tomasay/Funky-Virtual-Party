using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Provides fluent, chainable extension methods that create and configure AI generation tasks.
    /// These helpers let you start a task directly from the host object (string, AudioClip, Texture2D, etc.)
    /// and then continue the configuration via the task's fluent API.
    /// <para>Typical usage:</para>
    /// <code>
    /// // Create a chat-like text generation
    /// "Describe a cat playing piano."
    ///     .GENText()
    ///     .SetModel(OpenAIModel.GPT4o)
    ///     .ExecuteAsync();
    ///
    /// // Transcribe recorded speech
    /// audioClip.GENTranscript().ExecuteAsync();
    /// </code>
    /// </summary>
    public static class GENTaskExtensions
    {
        // ──────────────────────────────────────────────────────────────────────────────
        // TEXT
        // ──────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a text‑generation task that uses this string as the prompt.
        /// </summary>
        /// <param name="prompt">The prompt to send to the model.</param>
        /// <returns>A configurable <see cref="GENResponseTask"/> instance.</returns>
        /// <example>
        /// <code>
        /// "Tell me a joke."
        ///     .GENText()
        ///     .SetModel(OpenAIModel.GPT4o)
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public static GENResponseTask GENResponse(this string prompt, string formattedPrompt = null) => new(new Prompt(prompt, formattedPrompt));
        public static GENResponseTask GENResponse(this Prompt prompt) => new(prompt);

        [Obsolete("Use GENResponse instead.")]
        public static GENResponseTask GENText(this string prompt, string formattedPrompt = null) => GENResponse(new Prompt(prompt, formattedPrompt));

        public static GENCodeTask GENCode(this string prompt, string formattedPrompt = null) => new(new Prompt(prompt, formattedPrompt));
        public static GENCodeTask GENCode(this Prompt prompt) => new(prompt);

        /// <summary>
        /// Creates an image generation task using this string as the image prompt.
        /// 
        /// Example:
        ///     "A cat surfing a wave".GENImage().SetModel(ImageModel.DallE3).ExecuteAsync();
        /// </summary>
        public static GENImageTask GENImage(this string prompt, string formattedPrompt = null) => new(new Prompt(prompt, formattedPrompt));
        public static GENImageTask GENImage(this Prompt prompt) => new(prompt);

        /// <summary>
        /// Creates an image editing task by applying the given prompt to this image.
        /// 
        /// Note:
        ///     - DALL·E 3 image editing is not supported with the OpenAI API yet. (Only available in ChatGPT)
        /// 
        /// Example:
        ///     texture.GENImageEdit("Add sunglasses").SetModel(ImageModel.DallE2).ExecuteAsync();
        /// </summary>
        public static GENInpaintTask GENInpaint(this InpaintPrompt prompt) => new(prompt);
        public static GENInpaintTask GENInpaint(this Texture2D prompt, string instruction) => GENInpaint(new(instruction, prompt));
        public static GENInpaintTask GENInpaint(this Sprite prompt, string instruction) => GENInpaint(new(instruction, prompt.texture));

        /// <summary>
        /// Creates a task to generate variations (remixes) of the given image.
        /// 
        /// Note:
        ///    - DALL·E 3 image variation is not supported with the OpenAI API yet. (Only available in ChatGPT)
        /// 
        /// Example:
        ///     texture.GENImageVariation().SetModel(ImageModel.DallE2).ExecuteAsync();
        /// </summary>
        // public static GENImageVariationTask GENVariation(this Texture2D prompt) => new(prompt);
        // public static GENImageVariationTask GENVariation(this Sprite prompt) => new(prompt.texture);

        /// <summary>
        /// Creates a text-to-speech (TTS) task that reads this string aloud using a realistic AI voice.
        /// 
        /// Example:
        ///     "Hello there!".GENSpeech().SetVoice(ElevenLabsVoice.Rachel).ExecuteAsync();
        /// </summary>
        public static GENSpeechTask GENSpeech(this string prompt, string formattedPrompt = null) => new(new Prompt(prompt, formattedPrompt));
        public static GENSpeechTask GENSpeech(this Prompt prompt) => new(prompt);

        /// <summary>
        /// Creates a speech-to-text (STT) task that transcribes spoken words from this audio clip.
        /// 
        /// Example:
        ///     audioClip.GENTranscript().ExecuteAsync();
        /// </summary>
        public static GENTranscriptTask GENTranscript(this AudioClip prompt) => new(prompt);

        /// <summary>
        /// Translates the speech in this audio clip into English text.
        /// Useful for multilingual voice input.
        /// 
        /// Example:
        ///     audioClip.GENTranslation().ExecuteAsync();
        /// </summary>
        public static GENTranslationTask GENTranslation(this AudioClip prompt) => new(prompt);

        /// <summary>
        /// Generates structured JSON output by interpreting the text as instructions for a specific object type.
        /// Type T should be decorated with OpenAIJsonSchemaResponseAttribute or JsonSchemaAttribute.
        /// 
        /// Example:
        ///     "Create a product listing".GENObject<Product>().ExecuteAsync();
        /// </summary>
        public static GENStructTask<T> GENStruct<T>(this string prompt, string formattedPrompt = null) where T : class => new(new Prompt(prompt, formattedPrompt));
        public static GENStructTask<T> GENStruct<T>(this Prompt prompt) where T : class => new(prompt);

        [Obsolete("Use GENStruct instead.")]
        public static GENStructTask<T> GENObject<T>(this string prompt, string formattedPrompt = null) where T : class => new(new Prompt(prompt, formattedPrompt));
        public static GENStructTask<T> GENObject<T>(this Prompt prompt) where T : class => new(prompt);

        /// <summary>
        /// Generates a sound effect based on this prompt text.
        /// 
        /// Example:
        ///     "Footsteps on snow".GENSoundEffect().ExecuteAsync();
        /// </summary>
        public static GENSoundEffectTask GENSoundEffect(this string prompt, string formattedPrompt = null) => new(new Prompt(prompt, formattedPrompt));

        /// <summary>
        /// Changes the voice in this audio clip using AI voice transformation.
        /// 
        /// Example:
        ///     audioClip.GENVoiceChange().SetVoice(ElevenLabsVoice.Rachel).ExecuteAsync();
        /// </summary>
        public static GENVoiceChangeTask GENVoiceChange(this AudioClip prompt) => new(prompt);

        /// <summary>
        /// Isolates vocals or removes background noise from this audio clip.
        /// 
        /// Example:
        ///     audioClip.GENAudioIsolation().ExecuteAsync();
        /// </summary>
        public static GENAudioIsolationTask GENAudioIsolation(this AudioClip prompt) => new(prompt);

        /// <summary>
        /// Generates a video based on this prompt text.
        /// 
        /// Example:
        ///    "A cat surfing a wave".GENVideoGen().ExecuteAsync();
        /// </summary>
        public static GENVideoTask GENVideo(this string prompt, string formattedPrompt = null) => new(new Prompt(prompt, formattedPrompt));

        /// <summary>
        /// Generates a video based on this prompt image.
        /// 
        /// Example:
        ///   texture.GENVideoGen().ExecuteAsync();
        /// </summary> 
        public static GENVideoTask GENVideo(this Texture2D prompt) => new(prompt);

        /// <summary>
        /// Creates a moderation task to evaluate potentially harmful or sensitive content.
        /// This can be used to detect categories like hate speech, violence, self-harm, etc., 
        /// depending on the model and provider.
        ///
        /// Example:
        ///     content.GENModeration().SetModel(OpenAIModel.Moderation).ExecuteAsync();
        /// </summary>
        /// <param name="prompt">The content to be evaluated for moderation.</param>
        /// <returns>A <see cref="GENModerationTask"/> that can be configured and executed.</returns>
        public static GENModerationTask GENModeration(this string prompt, IEnumerable<SafetySetting> safetySettings) => new(prompt, safetySettings);
        public static GENModerationTask GENModeration(this Prompt prompt, IEnumerable<SafetySetting> safetySettings) => new(prompt, safetySettings);
    }
}
