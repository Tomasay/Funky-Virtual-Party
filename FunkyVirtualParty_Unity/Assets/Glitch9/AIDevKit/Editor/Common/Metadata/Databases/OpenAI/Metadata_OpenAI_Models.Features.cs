using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_OpenAI_Models
    {
        private static readonly (Modality, Modality, ModelFeature) kGPTBase = (
            Modality.Text,
            Modality.Text,
            ModelFeature.TextGeneration | ModelFeature.FineTuning);

        private static readonly (Modality, Modality, ModelFeature) kGPTLatest = (
            Modality.Text | Modality.Image,
            Modality.Text,
            ModelFeature.TextGeneration | ModelFeature.Streaming | ModelFeature.FunctionCalling | ModelFeature.StructuredOutputs);

        private static readonly (Modality, Modality, ModelFeature) kRealtime = (
            Modality.Text | Modality.Audio,
            Modality.Text | Modality.Audio,
            ModelFeature.Realtime | ModelFeature.FunctionCalling);

        private static readonly (Modality, Modality, ModelFeature) kSearch = (
            Modality.Text,
            Modality.Text,
            ModelFeature.TextGeneration | ModelFeature.Search);

        internal static readonly Dictionary<string, (Modality i, Modality o, ModelFeature f)> ModelPatterns = new()
        {
            ["babbage"] = kGPTBase,
            ["curie"] = kGPTBase,
            ["davinci"] = kGPTBase,
            ["gpt-3.5"] = kGPTBase,
            ["realtime"] = kRealtime,
            ["search"] = kSearch,
            ["embedding"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.TextEmbedding
            ),
            ["gpt-4.0"] = kGPTLatest,
            ["gpt-4.1-turbo"] = kGPTLatest,
            ["gpt-4.1-turbo-16k"] = kGPTLatest,
            ["gpt-4.1-turbo-32k"] = kGPTLatest,
            ["gpt-4.1"] = kGPTLatest,
            ["gpt-4o-transcribe"] = (
                Modality.Text | Modality.Audio,
                Modality.Text,
                ModelFeature.SpeechRecognition
            ),
            ["gpt-4o-mini-transcribe"] = (
                Modality.Text | Modality.Audio,
                Modality.Text,
                ModelFeature.SpeechRecognition
            ),
            ["gpt-4o"] = kGPTLatest,
            ["gpt-4-turbo"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.Streaming | ModelFeature.FineTuning | ModelFeature.FunctionCalling
            ),
            ["gpt-4"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.Streaming | ModelFeature.FineTuning
            ),
            ["gpt-3.5"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.FineTuning
            ),
            ["whisper"] = (
                Modality.Audio,
                Modality.Text,
                ModelFeature.SpeechRecognition
            ),
            ["dall-e-2"] = (
                Modality.Text,
                Modality.Image,
                ModelFeature.ImageGeneration | ModelFeature.ImageInpainting
            ),
            ["dall-e-3"] = (
                Modality.Text,
                Modality.Image,
                ModelFeature.ImageGeneration
            ),
            ["gpt-image"] = (
                Modality.Text | Modality.Image,
                Modality.Image,
                ModelFeature.ImageGeneration | ModelFeature.ImageInpainting
            ),
            ["tts-1"] = (
                Modality.Text,
                Modality.Audio,
                ModelFeature.SpeechGeneration
            ),
            ["text-moderation"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.Moderation
            ),
            ["omni-moderation"] = (
                Modality.Text | Modality.Image,
                Modality.Text,
                ModelFeature.Moderation
            ),
            ["computer-use-preview"] = (
                Modality.Text | Modality.Image,
                Modality.Text,
                ModelFeature.ComputerUse
            ),
            ["o4"] = kGPTLatest,
            ["o3-mini"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.Streaming | ModelFeature.FunctionCalling | ModelFeature.StructuredOutputs
            ),
            ["o3"] = kGPTLatest,
            ["o1-mini"] = (
                Modality.Text,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.Streaming
            ),
            ["o1-pro"] = (
                Modality.Text | Modality.Image,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.FunctionCalling | ModelFeature.StructuredOutputs
            ),
            ["o1"] = kGPTLatest,

            // added new on 2025-06-15
            ["codex-mini-latest"] = (
                Modality.Text | Modality.Image,
                Modality.Text,
                ModelFeature.TextGeneration | ModelFeature.Streaming | ModelFeature.FunctionCalling | ModelFeature.StructuredOutputs
            ),
        };

    }
}