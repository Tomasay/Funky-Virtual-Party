
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static class Metadata_ElevenLabs_Models
    {
        private static readonly Dictionary<string, UnixTime> elevenLabsModelReleaseDates = new()
        {
            { "eleven_monolingual_v1", new UnixTime(2023, 1, 1) }, // 출시일: 2023년 1월 1일 (추정)
            { "eleven_multilingual_v1", new UnixTime(2023, 5, 1) }, // 출시일: 2023년 5월 1일
            { "eleven_multilingual_v2", new UnixTime(2023, 8, 22) }, // 출시일: 2023년 8월 22일
            { "eleven_turbo_v2", new UnixTime(2023, 11, 9) }, // 출시일: 2023년 11월 9일
            { "eleven_multilingual_sts_v2", new UnixTime(2024, 2, 9) }, // 출시일: 2024년 2월 9일
            { "eleven_flash_v2", new UnixTime(2024, 3, 1) }, // 출시일: 2024년 3월 1일 (추정)
            { "eleven_flash_v2_5", new UnixTime(2024, 6, 1) }, // 출시일: 2024년 6월 1일 (추정)
            { "eleven_turbo_v2_5", new UnixTime(2024, 7, 19) }, // 출시일: 2024년 7월 19일
            { "eleven_english_sts_v2", new UnixTime(2024, 9, 1) }, // 출시일: 2024년 9월 1일 (추정)
        };

        internal static ModelFeature ResolveCapability(string family)
        {
            family = family.ToLower();

            if (family.Contains("changer")) return ModelFeature.VoiceChanger;
            return ModelFeature.SpeechGeneration;
        }

        internal static Modality ResolveInputModality(ModelFeature cap)
        {
            Modality modal = default;

            if (cap.HasFlag(ModelFeature.SpeechGeneration)) modal |= Modality.Text;
            if (cap.HasFlag(ModelFeature.SpeechRecognition)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.SoundFXGeneration)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.VideoGeneration)) modal |= Modality.Video;
            if (cap.HasFlag(ModelFeature.VoiceChanger)) modal |= Modality.Audio;

            return modal;
        }

        internal static Modality ResolveOutputModality(ModelFeature cap)
        {
            Modality modal = default;

            if (cap.HasFlag(ModelFeature.VideoGeneration)) modal |= Modality.Video;
            if (cap.HasFlag(ModelFeature.SoundFXGeneration)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.SpeechGeneration)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.SpeechRecognition)) modal |= Modality.Text;
            if (cap.HasFlag(ModelFeature.VoiceChanger)) modal |= Modality.Audio;

            return modal;
        }

        internal static ModelCatalogueEntry Resolve(ModelCatalogueEntry entry)
        {
            // Missing Properties:
            // ✓ Capability
            // ✘ Version
            // ✘ CreatedAt 
            // ✓ InputModality, OutputModality 

            //entry.Name = entry.Id;

            // check if the model ID is in the release dates dictionary
            if (elevenLabsModelReleaseDates.TryGetValue(entry.Id, out UnixTime releaseDate))
            {
                entry.CreatedAt = releaseDate;
            }
            else
            {
                entry.CreatedAt = UnixTime.Now;
            }

            entry.Feature = ResolveCapability(entry.Family);
            entry.InputModality = ResolveInputModality(entry.Feature);
            entry.OutputModality = ResolveOutputModality(entry.Feature);

            entry.Name = ModelMetadataUtil.ResolveLegacyName(entry.Id, entry.Name);
            entry.Description = TextSplitter.SplitToParagraphs(entry.Description);

            return entry;
        }

        internal static List<ModelCatalogueEntry> GetMissingModels() => MissingModels.missingModels;
        private static class MissingModels
        {
            internal readonly static List<ModelCatalogueEntry> missingModels = new() {
                new ModelCatalogueEntry() {
                    Api = Api.ElevenLabs,
                    Provider = "ElevenLabs",
                    Id = "scribe_v1",
                    Name = "Scribe v1",
                    Family = "Scribe",
                    ModelVersion = "v1",
                    Description = "Scribe v1 is our state-of-the-art speech recognition model designed for accurate transcription across 99 languages. It provides precise word-level timestamps and advanced features like speaker diarization and dynamic audio tagging.",
                    OwnedBy = "ElevenLabs",
                    CreatedAt = new UnixTime(2023, 1, 1),
                    Feature = ModelFeature.SpeechRecognition,
                    InputModality = Modality.Audio,
                    OutputModality = Modality.Text,
                },
                new ModelCatalogueEntry() {
                    Api = Api.ElevenLabs,
                    Provider = "ElevenLabs",
                    Id = "scribe_v1_experimental",
                    Name = "Scribe v1 Experimental",
                    Family = "Scribe",
                    ModelVersion = "v1",
                    Description = "Scribe v1 Experimental is our state-of-the-art speech recognition model with experimental features: improved multilingual performance, reduced hallucinations during silence, fewer audio tags, and better handling of early transcript termination.",
                    OwnedBy = "ElevenLabs",
                    CreatedAt = new UnixTime(2023, 1, 1),
                    Feature = ModelFeature.SpeechRecognition,
                    InputModality = Modality.Audio,
                    OutputModality = Modality.Text,
                }
            };
        }
    }
}