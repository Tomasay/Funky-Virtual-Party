using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor.IMGUI.Controls;

namespace Glitch9.AIDevKit.Editor
{
    public class ModelCatalogueFilter : TreeViewItemFilter
    {
        private static readonly EPrefs<Api> kApiProvider = new("AIDevKit.ModelCatalogue.ApiProvider", Api.All);
        private static readonly EPrefs<Api> kModelProvider = new("AIDevKit.ModelCatalogue.ModelProvider", Api.All);

        // only-show options
        private static readonly EPrefs<bool> kOnlyShowMissingModels = new("AIDevKit.ModelCatalogue.OnlyShowMissingModels", false);
        private static readonly EPrefs<bool> kOnlyShowDefaultModels = new("AIDevKit.ModelCatalogue.OnlyShowDefaultModels", false);
        private static readonly EPrefs<bool> kOnlyShowMyLibrary = new("AIDevKit.ModelCatalogue.OnlyShowMyLibrary", false);
        private static readonly EPrefs<bool> kOnlyShowOfficialModels = new("AIDevKit.ModelCatalogue.OnlyShowOfficialModels", false);
        private static readonly EPrefs<bool> kOnlyShowCustomModels = new("AIDevKit.ModelCatalogue.OnlyShowCustomModels", false);

        // show options
        private static readonly EPrefs<bool> kShowLegacyModels = new("AIDevKit.ModelCatalogue.ShowLegacyModels", true);
        private static readonly EPrefs<bool> kShowDeprecatedModels = new("AIDevKit.ModelCatalogue.ShowDeprecatedModels", true);

        // filter options
        private static readonly EPrefs<bool> kTextGeneration = new("AIDevKit.ModelCatalogue.TextGeneration", false);
        private static readonly EPrefs<bool> kStructuredOutput = new("AIDevKit.ModelCatalogue.StructuredOutput", false);
        private static readonly EPrefs<bool> kFunctionCalling = new("AIDevKit.ModelCatalogue.FunctionCalling", false);
        private static readonly EPrefs<bool> kCodeExecution = new("AIDevKit.ModelCatalogue.CodeExecution", false);
        private static readonly EPrefs<bool> kFineTuning = new("AIDevKit.ModelCatalogue.FineTuning", false);
        private static readonly EPrefs<bool> kStreaming = new("AIDevKit.ModelCatalogue.Streaming", false);
        private static readonly EPrefs<bool> kImageGeneration = new("AIDevKit.ModelCatalogue.ImageGeneration", false);
        private static readonly EPrefs<bool> kImageInpainting = new("AIDevKit.ModelCatalogue.ImageInpainting", false);
        private static readonly EPrefs<bool> kSpeechGeneration = new("AIDevKit.ModelCatalogue.SpeechGeneration", false);
        private static readonly EPrefs<bool> kSpeechRecognition = new("AIDevKit.ModelCatalogue.SpeechRecognition", false);
        private static readonly EPrefs<bool> kSoundFXGeneration = new("AIDevKit.ModelCatalogue.SoundFXGeneration", false);
        private static readonly EPrefs<bool> kVoiceChanger = new("AIDevKit.ModelCatalogue.VoiceChanger", false);
        private static readonly EPrefs<bool> kVideoGeneration = new("AIDevKit.ModelCatalogue.VideoGeneration", false);
        private static readonly EPrefs<bool> kTextEmbedding = new("AIDevKit.ModelCatalogue.TextEmbedding", false);
        private static readonly EPrefs<bool> kModeration = new("AIDevKit.ModelCatalogue.Moderation", false);
        private static readonly EPrefs<bool> kSearch = new("AIDevKit.ModelCatalogue.Search", false);
        private static readonly EPrefs<bool> kRealtime = new("AIDevKit.ModelCatalogue.Realtime", false);
        private static readonly EPrefs<bool> kComputerUse = new("AIDevKit.ModelCatalogue.ComputerUse", false);
        private static readonly EPrefs<bool> kVoiceIsolation = new("AIDevKit.ModelCatalogue.VoiceIsolation", false);


        internal static Api ApiProvider { get => kApiProvider.Value; set => kApiProvider.Value = value; }
        internal static Api ModelProvider { get => kModelProvider.Value; set => kModelProvider.Value = value; }


        internal static bool MissingModels { get => kOnlyShowMissingModels.Value; set => kOnlyShowMissingModels.Value = value; }
        internal static bool DefaultModels { get => kOnlyShowDefaultModels.Value; set => kOnlyShowDefaultModels.Value = value; }
        internal static bool InMyLibrary { get => kOnlyShowMyLibrary.Value; set => kOnlyShowMyLibrary.Value = value; }
        internal static bool OfficialModels { get => kOnlyShowOfficialModels.Value; set => kOnlyShowOfficialModels.Value = value; }
        internal static bool CustomModels { get => kOnlyShowCustomModels.Value; set => kOnlyShowCustomModels.Value = value; }


        internal static bool DeprecatedModels { get => kShowDeprecatedModels.Value; set => kShowDeprecatedModels.Value = value; }
        internal static bool LegacyModels { get => kShowLegacyModels.Value; set => kShowLegacyModels.Value = value; }

        internal static bool TextGeneration { get => kTextGeneration.Value; set => kTextGeneration.Value = value; }
        internal static bool StructuredOutputs { get => kStructuredOutput.Value; set => kStructuredOutput.Value = value; }
        internal static bool FunctionCalling { get => kFunctionCalling.Value; set => kFunctionCalling.Value = value; }
        internal static bool CodeExecution { get => kCodeExecution.Value; set => kCodeExecution.Value = value; }
        internal static bool FineTuning { get => kFineTuning.Value; set => kFineTuning.Value = value; }
        internal static bool Streaming { get => kStreaming.Value; set => kStreaming.Value = value; }
        internal static bool ImageGeneration { get => kImageGeneration.Value; set => kImageGeneration.Value = value; }
        internal static bool ImageInpainting { get => kImageInpainting.Value; set => kImageInpainting.Value = value; }
        internal static bool SpeechGeneration { get => kSpeechGeneration.Value; set => kSpeechGeneration.Value = value; }
        internal static bool SpeechRecognition { get => kSpeechRecognition.Value; set => kSpeechRecognition.Value = value; }
        internal static bool SoundFXGeneration { get => kSoundFXGeneration.Value; set => kSoundFXGeneration.Value = value; }
        internal static bool VoiceChanger { get => kVoiceChanger.Value; set => kVoiceChanger.Value = value; }
        internal static bool VideoGeneration { get => kVideoGeneration.Value; set => kVideoGeneration.Value = value; }
        internal static bool TextEmbedding { get => kTextEmbedding.Value; set => kTextEmbedding.Value = value; }
        internal static bool Moderation { get => kModeration.Value; set => kModeration.Value = value; }
        internal static bool Search { get => kSearch.Value; set => kSearch.Value = value; }
        internal static bool Realtime { get => kRealtime.Value; set => kRealtime.Value = value; }
        internal static bool ComputerUse { get => kComputerUse.Value; set => kComputerUse.Value = value; }
        internal static bool VoiceIsolation { get => kVoiceIsolation.Value; set => kVoiceIsolation.Value = value; }

        public override bool IsVisible(TreeViewItem item)
        {
            if (item is ModelCatalogueTreeViewItem i)
            {
                if (InMyLibrary && !i.InMyLibrary) return false;
                if (ApiProvider != Api.All && ApiProvider != i.Api) return false;
                if (MissingModels && i.InMyLibrary) return false;
                if (DefaultModels && !i.IsDefault) return false;
                if (OfficialModels && i.IsCustom) return false;
                if (CustomModels && !i.IsCustom) return false;
                if (LegacyModels && !i.IsLegacy) return false;
                if (DeprecatedModels && !i.IsObsolete) return false;

                if (TextGeneration && !i.Capability.HasFlag(ModelFeature.TextGeneration)) return false;
                if (StructuredOutputs && !i.Capability.HasFlag(ModelFeature.StructuredOutputs)) return false;
                if (FunctionCalling && !i.Capability.HasFlag(ModelFeature.FunctionCalling)) return false;
                if (CodeExecution && !i.Capability.HasFlag(ModelFeature.CodeExecution)) return false;
                if (FineTuning && !i.Capability.HasFlag(ModelFeature.FineTuning)) return false;
                if (Streaming && !i.Capability.HasFlag(ModelFeature.Streaming)) return false;
                if (ImageGeneration && !i.Capability.HasFlag(ModelFeature.ImageGeneration)) return false;
                if (ImageInpainting && !i.Capability.HasFlag(ModelFeature.ImageInpainting)) return false;
                if (SpeechGeneration && !i.Capability.HasFlag(ModelFeature.SpeechGeneration)) return false;
                if (SpeechRecognition && !i.Capability.HasFlag(ModelFeature.SpeechRecognition)) return false;
                if (SoundFXGeneration && !i.Capability.HasFlag(ModelFeature.SoundFXGeneration)) return false;
                if (VoiceChanger && !i.Capability.HasFlag(ModelFeature.VoiceChanger)) return false;
                if (VideoGeneration && !i.Capability.HasFlag(ModelFeature.VideoGeneration)) return false;
                if (TextEmbedding && !i.Capability.HasFlag(ModelFeature.TextEmbedding)) return false;
                if (Moderation && !i.Capability.HasFlag(ModelFeature.Moderation)) return false;
                if (Search && !i.Capability.HasFlag(ModelFeature.Search)) return false;
                if (Realtime && !i.Capability.HasFlag(ModelFeature.Realtime)) return false;
                if (ComputerUse && !i.Capability.HasFlag(ModelFeature.ComputerUse)) return false;
                if (VoiceIsolation && !i.Capability.HasFlag(ModelFeature.VoiceIsolation)) return false;
            }

            return base.IsVisible(item);
        }
    }
}