using System;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Formerly known as <see cref="ModelCapability"/>, this enum represents the features that a model can support.
    /// It is used to determine the capabilities of a model and to check if a specific feature is supported.
    /// </summary>
    [Flags]
    public enum ModelFeature
    {
        None = 0,

        // Core Language Capabilities
        [InspectorName("Text Generation")] TextGeneration = 1 << 0,
        [InspectorName("Fine-tuning")] FineTuning = 1 << 1,
        [InspectorName("Streaming")] Streaming = 1 << 2,
        [InspectorName("Structured Outputs")] StructuredOutputs = 1 << 3,
        [InspectorName("Code Execution")] CodeExecution = 1 << 4,
        [InspectorName("Function Calling")] FunctionCalling = 1 << 5,
        [InspectorName("Caching")] Caching = 1 << 6,

        // Media Capabilities
        [InspectorName("Image Generation")] ImageGeneration = 1 << 7,
        [InspectorName("Image Inpainting")] ImageInpainting = 1 << 8,
        [InspectorName("Speech Generation")] SpeechGeneration = 1 << 9,
        [InspectorName("Speech Recognition")] SpeechRecognition = 1 << 10,
        [InspectorName("Sound FX Generation")] SoundFXGeneration = 1 << 11,
        [InspectorName("Voice Changer")] VoiceChanger = 1 << 12,
        [InspectorName("Video Generation")] VideoGeneration = 1 << 13,

        // Other
        [InspectorName("Text Embedding")] TextEmbedding = 1 << 14,
        [InspectorName("Moderation")] Moderation = 1 << 15,
        [InspectorName("Search")] Search = 1 << 16,
        [InspectorName("Realtime Interaction")] Realtime = 1 << 17,
        [InspectorName("Computer Control")] ComputerUse = 1 << 18,

        // Added
        [InspectorName("Voice Isolation")] VoiceIsolation = 1 << 19,
        [InspectorName("Reasoning / Logic")] Reasoning = 1 << 20,
        [InspectorName("Distillation / Compression")] Distillation = 1 << 21,
        [InspectorName("Predicted Outputs")] PredictedOutputs = 1 << 22,
    }
}