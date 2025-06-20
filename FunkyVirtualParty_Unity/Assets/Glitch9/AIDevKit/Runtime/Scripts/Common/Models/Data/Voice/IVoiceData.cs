using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    internal interface IVoiceData : IData
    {
        Api Api { get; } // the provider of the Voice
        //string Id { get; }
        //string Name { get; }
        UnixTime? CreatedAt { get; }
        string OwnedBy { get; }
        string Description { get; }
        string PreviewUrl => null; // the preview url of the voice
        string PreviewPath => null; // the preview path of the voice
        VoiceCategory? Category { get; set; }
        VoiceGender? Gender { get; }
        VoiceType? Type { get; }
        VoiceAge? Age { get; }
        SystemLanguage Language { get; }
        string Accent { get; }
        bool IsCustom { get; }
        bool? IsFeatured => null; // is the voice featured in the catalogue
        bool? IsFree => null; // is the voice free to use
        string ImageUrl => null; // the image url of the voice
        string Descriptive => null; // the descriptive name of the voice
        string Locale => null; // the locale of the voice
        List<string> AvailableForTiers => null;
    }

}