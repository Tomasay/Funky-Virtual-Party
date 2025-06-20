using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    /// <summary>
    /// Google does not have an API for retrieving voices, so we need to hard-code them for now.
    /// </summary>
    internal static class Metadata_Google_Voices
    {
        // https://github.com/google-gemini/cookbook/blob/main/quickstarts/Get_started_TTS.ipynb

        /* 
               From: https://ai.google.dev/gemini-api/docs/speech-generation

               Zephyr -- Bright
               Puck -- Upbeat
               Charon -- Informative
               Kore -- Firm
               Fenrir -- Excitable
               Leda -- Youthful
               Orus -- Firm
               Aoede -- Breezy
               Callirrhoe -- Easy-going
               Autonoe -- Bright
               Enceladus -- Breathy
               Iapetus -- Clear
               Umbriel -- Easy-going
               Algieba -- Smooth
               Despina -- Smooth
               Erinome -- Clear
               Algenib -- Gravelly
               Rasalgethi -- Informative
               Laomedeia -- Upbeat
               Achernar -- Soft
               Alnilam -- Firm
               Schedar -- Even
               Gacrux -- Mature
               Pulcherrima -- Forward
               Achird -- Friendly
               Zubenelgenubi -- Casual
               Vindemiatrix -- Gentle
               Sadachbia -- Lively
               Sadaltager -- Knowledgeable
               Sulafat -- Warm
               */

        internal const string Zephyr = "zephyr";
        internal const string Puck = "puck";
        internal const string Charon = "charon";
        internal const string Kore = "kore";
        internal const string Fenrir = "fenrir";
        internal const string Leda = "leda";
        internal const string Orus = "orus";
        internal const string Aoede = "aoede";
        internal const string Callirrhoe = "callirrhoe";
        internal const string Autonoe = "autonoe";
        internal const string Enceladus = "enceladus";
        internal const string Iapetus = "iapetus";
        internal const string Umbriel = "umbriel";
        internal const string Algieba = "algieba";
        internal const string Despina = "despina";
        internal const string Erinome = "erinome";
        internal const string Algenib = "algenib";
        internal const string Rasalgethi = "rasalgethi";
        internal const string Laomedeia = "laomedeia";
        internal const string Achernar = "achernar";
        internal const string Alnilam = "alnilam";
        internal const string Schedar = "schedar";
        internal const string Gacrux = "gacrux";
        internal const string Pulcherrima = "pulcherrima";
        internal const string Achird = "achird";
        internal const string Zubenelgenubi = "zubenelgenubi";
        internal const string Vindemiatrix = "vindemiatrix";
        internal const string Sadachbia = "sadachbia";
        internal const string Sadaltager = "sadaltager";
        internal const string Sulafat = "sulafat";

        internal static readonly List<TempVoiceData> List = new()
        {
            TempVoice.Google("zephyr", "Zephyr", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("puck", "Puck", VoiceGender.Male, VoiceAge.Young),
            TempVoice.Google("charon", "Charon", VoiceGender.Male, VoiceAge.MiddleAged),
            TempVoice.Google("kore", "Kore", VoiceGender.Female, VoiceAge.MiddleAged),
            TempVoice.Google("fenrir", "Fenrir", VoiceGender.Male, VoiceAge.Young),
            TempVoice.Google("leda", "Leda", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("orus", "Orus", VoiceGender.Male, VoiceAge.MiddleAged),
            TempVoice.Google("aoede", "Aoede", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("callirrhoe", "Callirrhoe", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("autonoe", "Autonoe", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("enceladus", "Enceladus", VoiceGender.Male, VoiceAge.Young),
            TempVoice.Google("iapetus", "Iapetus", VoiceGender.Male, VoiceAge.Young),
            TempVoice.Google("umbriel", "Umbriel", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("algieba", "Algieba", VoiceGender.Female, VoiceAge.MiddleAged),
            TempVoice.Google("despina", "Despina", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("erinome", "Erinome", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("algenib", "Algenib", VoiceGender.Male, VoiceAge.Senior),
            TempVoice.Google("rasalgethi", "Rasalgethi", VoiceGender.Male, VoiceAge.Senior),
            TempVoice.Google("laomedeia", "Laomedeia", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("achernar", "Achernar", VoiceGender.Female, VoiceAge.MiddleAged),
            TempVoice.Google("alnilam", "Alnilam", VoiceGender.Male, VoiceAge.MiddleAged),
            TempVoice.Google("schedar", "Schedar", VoiceGender.Male, VoiceAge.MiddleAged),
            TempVoice.Google("gacrux", "Gacrux", VoiceGender.Female, VoiceAge.MiddleAged),
            TempVoice.Google("pulcherrima", "Pulcherrima", VoiceGender.Female, VoiceAge.MiddleAged),
            TempVoice.Google("achird", "Achird", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("zubenelgenubi", "Zubenelgenubi", VoiceGender.Male, VoiceAge.Young),
            TempVoice.Google("vindemiatrix", "Vindemiatrix", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("sadachbia", "Sadachbia", VoiceGender.Female, VoiceAge.Young),
            TempVoice.Google("sadaltager", "Sadaltager", VoiceGender.Male, VoiceAge.MiddleAged),
            TempVoice.Google("sulafat", "Sulafat", VoiceGender.Female, VoiceAge.MiddleAged),
        };
    }
}