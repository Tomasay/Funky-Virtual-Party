using System;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Represents the endpoints available for AI models.
    /// </summary>
    [Flags]
    public enum ModelEndpoint
    {
        /// <summary>
        /// Error endpoint, used when no valid endpoint is specified.
        /// </summary> 
        None = 0,

        [Endpoint("Completions (Legacy)", "v1/completions")]
        Completions = 1 << 0,

        [Endpoint("Chat Completions", "v1/chat/completions")]
        ChatCompletions = 1 << 1,

        [Endpoint("Fine Tuning", "v1/fine-tuning")]
        FineTuning = 1 << 2,

        [Endpoint("Embeddings", "v1/embeddings")]
        Embeddings = 1 << 3,

        [Endpoint("Moderations", "v1/moderations")]
        Moderations = 1 << 4,

        [Endpoint("Image Generation", "v1/images/generations")]
        ImageGeneration = 1 << 5,

        [Endpoint("Image Inpainting", "v1/images/edits")]
        ImageInpainting = 1 << 6,

        [Endpoint("Speech to Text", "v1/audio/transcriptions")]
        AudioTranscriptions = 1 << 7,

        [Endpoint("Speech to Text (Translation)", "v1/audio/translations")]
        AudioTranslations = 1 << 8,

        [Endpoint("Text to Speech", "v1/audio/speech")]
        SpeechGeneration = 1 << 9,

        [Endpoint("Assistants", "v1/assistants")]
        Assistants = 1 << 10,

        [Endpoint("Batch", "v1/batch")]
        Batch = 1 << 11,

        [Endpoint("Realtime", "v1/realtime")]
        Realtime = 1 << 12,

        [Endpoint("Responses", "v1/responses")]
        Responses = 1 << 13,
    }
}