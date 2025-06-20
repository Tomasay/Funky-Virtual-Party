using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    internal static class ElevenLabsUtil
    {
        internal static SystemLanguage GetLanguage(string languageCode)
        {
            return languageCode switch
            {
                "eng" => SystemLanguage.English,
                "spa" => SystemLanguage.Spanish,
                "fra" => SystemLanguage.French,
                "deu" => SystemLanguage.German,
                "ita" => SystemLanguage.Italian,
                "jpn" => SystemLanguage.Japanese,
                "kor" => SystemLanguage.Korean,
                "rus" => SystemLanguage.Russian,
                "zho" => SystemLanguage.Chinese,
                "ara" => SystemLanguage.Arabic,
                "por" => SystemLanguage.Portuguese,
                "nld" => SystemLanguage.Dutch,
                "swe" => SystemLanguage.Swedish,
                "dan" => SystemLanguage.Danish,
                "nor" => SystemLanguage.Norwegian,
                "fin" => SystemLanguage.Finnish,
                "hun" => SystemLanguage.Hungarian,
                "pol" => SystemLanguage.Polish,
                "ces" => SystemLanguage.Czech,
                "tur" => SystemLanguage.Turkish,
                "ell" => SystemLanguage.Greek,
                "tha" => SystemLanguage.Thai,
                "vie" => SystemLanguage.Vietnamese,
                "heb" => SystemLanguage.Hebrew,
                "ukr" => SystemLanguage.Ukrainian,
                "slk" => SystemLanguage.Slovak,
                "slv" => SystemLanguage.Slovenian,
                "lit" => SystemLanguage.Lithuanian,
                "est" => SystemLanguage.Estonian,
                "lav" => SystemLanguage.Latvian,
                "bul" => SystemLanguage.Bulgarian,
                "ron" => SystemLanguage.Romanian,
                "ind" => SystemLanguage.Indonesian,
                _ => SystemLanguage.Unknown,
            };
        }
    }
}