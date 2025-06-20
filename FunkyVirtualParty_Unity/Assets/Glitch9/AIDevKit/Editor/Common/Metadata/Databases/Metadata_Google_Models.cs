

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_Google_Models
    {
        internal static Modality ResolveInputModality(ModelFeature cap, string id, string family)
        {
            if (id.Contains("veo-")) return Modality.Text | Modality.Image;

            Modality modal = default;

            if (cap.HasFlag(ModelFeature.SpeechGeneration)) modal |= Modality.Text;
            if (cap.HasFlag(ModelFeature.SpeechRecognition)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.SoundFXGeneration)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.TextGeneration)) modal |= ResolveLLMInputModality(family);
            if (cap.HasFlag(ModelFeature.ImageGeneration)) modal |= Modality.Image;
            if (cap.HasFlag(ModelFeature.VideoGeneration)) modal |= Modality.Video;
            if (cap.HasFlag(ModelFeature.TextEmbedding)) modal |= Modality.TextEmbedding;
            if (cap.HasFlag(ModelFeature.Moderation)) modal |= Modality.Text;

            return modal;
        }

        private static Modality ResolveLLMInputModality(string family)
        {
            if (family == ModelFamily.Gemini) return Modality.Text | Modality.Image | Modality.Audio | Modality.Video;
            return Modality.Text;
        }

        internal static Modality ResolveOutputModality(ModelFeature cap, string id)
        {
            Modality modal = default;

            if (id.Contains("veo-")) modal |= Modality.Video;
            if (id.Contains("tts")) modal |= Modality.Audio;

            // Gemini 2.0 Flash: Text, images (experimental), and audio (coming soon) 
            if (id.Contains("gemini-2.0-flash"))
            {
                if (id.Contains("live")) modal |= Modality.Audio;
                if (id.Contains("image")) modal |= Modality.Image;
                if (id.Contains("video")) modal |= Modality.Video;
                if (id.Contains("audio")) modal |= Modality.Audio;
            }

            if (cap.HasFlag(ModelFeature.TextEmbedding)) modal |= Modality.TextEmbedding;
            if (cap.HasFlag(ModelFeature.ImageGeneration)) modal |= Modality.Image;
            if (cap.HasFlag(ModelFeature.VideoGeneration)) modal |= Modality.Video;
            if (cap.HasFlag(ModelFeature.TextGeneration)) modal |= Modality.Text;
            if (cap.HasFlag(ModelFeature.SoundFXGeneration)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.SpeechGeneration)) modal |= Modality.Audio;
            if (cap.HasFlag(ModelFeature.SpeechRecognition)) modal |= Modality.Text;

            return modal;
        }

        internal static ModelFeature ResolveExtraCapability(Modality outputModal, string id)
        {
            ModelFeature cap = default;

            if (outputModal.HasFlag(Modality.Image)) cap |= ModelFeature.ImageGeneration;
            if (outputModal.HasFlag(Modality.Video)) cap |= ModelFeature.VideoGeneration;
            if (outputModal.HasFlag(Modality.Audio)) cap |= ModelFeature.SpeechGeneration;
            if (id.Contains("tts")) cap |= ModelFeature.SpeechGeneration;
            if (id.Contains("veo")) cap |= ModelFeature.VideoGeneration;

            return cap;
        }

        internal static ModelCatalogueEntry Resolve(ModelCatalogueEntry entry)
        {
            AIDevKitDebug.Mark($"Resolving Google model metadata for '{entry.Id}'...");

            // Missing Properties:
            // ✓ Type (Resolved inside GoogleModelData.cs)
            // ✓ IsTrainable (Resolved inside GoogleModelData.cs)
            // ✓ CreatedAt 
            // ✓ InputModality, OutputModality

            // entry.Name = entry.Id;

            // 출시일이 위의 modelReleaseDates에 포함되어 있는지 확인
            if (ReleaseDates.TryGetValue(entry.Id, out UnixTime releaseDate))
            {
                entry.CreatedAt = releaseDate;
            }
            else
            {
                // 출시일이 없으면 ResolveCreatedAt 메서드를 사용하여 날짜를 추출
                entry.CreatedAt ??= NaturalDateParser.ResolveTimeFromDescription(entry.Description);
                entry.CreatedAt ??= UnixTime.Now; // 그래도 출시일이 없으면 기본값으로 현재 날짜를 사용
            }

            entry.SetPrices(Prices.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Price, Api.Google, notFound)));

            entry.InputModality = ResolveInputModality(entry.Feature, entry.Id, entry.Family);
            entry.OutputModality = ResolveOutputModality(entry.Feature, entry.Id);
            entry.Feature |= ResolveExtraCapability(entry.OutputModality, entry.Id);

            (int performance, int speed) = Performances.GetBestMatch(entry.Id, (notFound) => MetadataLogger.Missing(MetadataType.Performance, Api.Google, notFound));
            entry.Performance = performance;
            entry.Speed = speed;

            entry.Name = ModelMetadataUtil.ResolveLegacyName(entry.Id, entry.Name);
            entry.Description = TextSplitter.SplitToParagraphs(entry.Description);

            return entry;
        }
    }
}