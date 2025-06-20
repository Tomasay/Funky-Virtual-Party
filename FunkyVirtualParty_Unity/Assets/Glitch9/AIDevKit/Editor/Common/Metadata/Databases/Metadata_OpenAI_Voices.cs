using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    /// <summary>
    /// OpenAI does not have an API for retrieving voices, so we need to hard-code them for now.
    /// </summary>
    internal static class Metadata_OpenAI_Voices
    {
        internal const string Alloy = "alloy";
        internal const string Echo = "echo";
        internal const string Fable = "fable";
        internal const string Onyx = "onyx";
        internal const string Nova = "nova";
        internal const string Shimmer = "shimmer";

        // New voice actors added 2025.01.23
        internal const string Ash = "ash";
        internal const string Coral = "coral";
        internal const string Sage = "sage";

        // New voice actors added 2025.04.14
        internal const string Ballad = "ballad";

        internal static readonly List<TempVoiceData> List = new()
        {
            TempVoice.OpenAI(Alloy, "Alloy", VoiceGender.Male, VoiceAge.Young, VoiceType.Narration),
            TempVoice.OpenAI(Echo, "Echo", VoiceGender.Male, VoiceAge.MiddleAged, VoiceType.Narration),
            TempVoice.OpenAI(Fable, "Fable", VoiceGender.Female, VoiceAge.Young, VoiceType.SocialMedia),
            TempVoice.OpenAI(Onyx, "Onyx", VoiceGender.Male, VoiceAge.MiddleAged, VoiceType.News),
            TempVoice.OpenAI(Nova, "Nova", VoiceGender.Female, VoiceAge.Young, VoiceType.Narration),
            TempVoice.OpenAI(Shimmer, "Shimmer", VoiceGender.Female, VoiceAge.Young, VoiceType.SocialMedia),
            TempVoice.OpenAI(Ash, "Ash", VoiceGender.Male, VoiceAge.Young, VoiceType.Narration),
            TempVoice.OpenAI(Coral, "Coral", VoiceGender.Female, VoiceAge.Young, VoiceType.SocialMedia),
            TempVoice.OpenAI(Sage, "Sage", VoiceGender.NonBinary, VoiceAge.MiddleAged, VoiceType.Narration),
            TempVoice.OpenAI(Ballad, "Ballad", VoiceGender.Male, VoiceAge.Senior, VoiceType.Characters),
        };
    }
}