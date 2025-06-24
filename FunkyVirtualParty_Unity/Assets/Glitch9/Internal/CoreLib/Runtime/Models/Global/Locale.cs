using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Glitch9
{
    /// <summary>
    /// Versatile locale type that can be used in various ways.
    /// </summary>
    [Serializable]
    public readonly struct Locale : IComparable
    {
        public static Locale Unknown => new(SystemLanguage.Unknown);
        public static Locale Default => new(SystemLanguage.English);

        public static implicit operator Locale(SystemLanguage language) => new(language);
        public static implicit operator SystemLanguage(Locale locale) => locale.Value;
        public static explicit operator string(Locale locale) => locale.Value.ToIETFTag();
        public static explicit operator Locale(int value) => new(value);
        public static explicit operator int(Locale locale) => (int)locale.Value;

        public SystemLanguage Value { get; }

        public Locale(SystemLanguage value)
        {
            Value = value;
        }

        public Locale(int value)
        {
            Value = (SystemLanguage)value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToInspectorName()
        {
            return Value.ToInspectorName();
        }

        public int CompareTo(object obj)
        {
            if (obj is Locale locale)
            {
                return Value.CompareTo(locale.Value);
            }

            throw new ArgumentException("Object is not a Locale");
        }

        //비교
        public static bool operator ==(Locale a, Locale b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Locale a, Locale b)
        {
            return a.Value != b.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Locale locale && Value == locale.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Locale a, SystemLanguage b)
        {
            return a.Value == b;
        }

        public static bool operator !=(Locale a, SystemLanguage b)
        {
            return a.Value != b;
        }

        public static bool operator ==(SystemLanguage a, Locale b)
        {
            return a == b.Value;
        }

        public static bool operator !=(SystemLanguage a, Locale b)
        {
            return a != b.Value;
        }
    }

    public static class LocaleUtil
    {
        private static readonly Dictionary<SystemLanguage, string> _cachedInspectorNames = new();
        public static string ToInspectorName(this SystemLanguage language)
        {
            if (_cachedInspectorNames.TryGetValue(language, out string displayName))
            {
                return displayName;
            }

            string isoCode = language.ToISOCode();
            displayName = $"{language} ({isoCode})";
            _cachedInspectorNames.Add(language, displayName);
            return displayName;
        }

        /*
         * When it comes to specifying languages and locales in software development, including game development,
         * both the two-letter ISO 639-1 language codes (e.g., EN, JP) and
         * the combination of ISO 639-1 language codes with ISO 3166-1 alpha-2 country codes (e.g., en-US, ja-JP) are used,
         * but for different purposes:
         *
         * ISO 639-1 Language Codes (e.g., EN, JP):
         * These two-letter codes are used to specify languages in a general way.
         * They are useful when your application or content does not need to differentiate between regional dialects or cultural differences.
         * For example, if your game's audio does not change between different English-speaking regions, using "EN" might be sufficient.
         *
         * IETF Language Tags (e.g., en-US, ja-JP):
         * These tags combine ISO 639-1 language codes with ISO 3166-1 alpha-2 country codes to specify not just the language,
         * but also the locale. This is important in cases where regional variations, dialects,
         * or cultural differences need to be accounted for in the content.
         * For instance, English as spoken in the United States (en-US) can have slight differences in spelling, phrasing,
         * or even content, compared to English as spoken in the United Kingdom (en-GB).
         *
         * Which is More Commonly Used?
         * 1. General Use:
         * For general language support where regional differences are minimal or irrelevant,
         * the simpler ISO 639-1 codes are often sufficient and commonly used.
         *
         * 2. Localized Content:
         * When it comes to localized content that needs to respect cultural and regional differences,
         * the more specific IETF language tags (like en-US, ja-JP) are more commonly used.
         * They allow for more precise targeting of language variants and are particularly useful in games, websites,
         * and applications that cater to a global audience with content tailored to specific regions.
         */


        /// <summary>
        /// ISO-639-1 Language Codes (e.g., EN, JP)
        /// </summary>
        public static string ToISOCode(this SystemLanguage locale)
        {
            return locale switch
            {
                SystemLanguage.English => "en",
                SystemLanguage.Korean => "ko",
                SystemLanguage.Japanese => "ja",
                SystemLanguage.Chinese => "zh",
                SystemLanguage.ChineseSimplified => "zh",
                SystemLanguage.ChineseTraditional => "zh",
                SystemLanguage.Thai => "th",
                SystemLanguage.Vietnamese => "vi",
                SystemLanguage.Spanish => "es",
                SystemLanguage.Portuguese => "pt",
                SystemLanguage.French => "fr",
                SystemLanguage.German => "de",
                SystemLanguage.Italian => "it",
                SystemLanguage.Polish => "pl",
                SystemLanguage.Arabic => "ar",
                SystemLanguage.Finnish => "fi",
                SystemLanguage.Norwegian => "no",
                SystemLanguage.Swedish => "sv",
                SystemLanguage.Russian => "ru",
                SystemLanguage.Hungarian => "hu",
                SystemLanguage.Hindi => "hi",
                SystemLanguage.Turkish => "tr",
                SystemLanguage.Romanian => "ro",
                SystemLanguage.Danish => "da",
                SystemLanguage.Dutch => "nl",
                SystemLanguage.Czech => "cs",
                SystemLanguage.Slovak => "sk",
                SystemLanguage.Greek => "el",
                SystemLanguage.Bulgarian => "bg",
                //SystemLanguage.Croatian => "hr",
                SystemLanguage.Slovenian => "sl",
                SystemLanguage.Lithuanian => "lt",
                SystemLanguage.Latvian => "lv",
                SystemLanguage.Estonian => "et",
                SystemLanguage.Icelandic => "is",
                //SystemLanguage.Malay => "ms",
                _ => "en",
            };
        }

        public static SystemLanguage ParseISOCode(string localeCode)
        {
            var result = localeCode.ToLower() switch // Ensure the localeCode is in lowercase for comparison
            {
                "en" => SystemLanguage.English,
                "ko" => SystemLanguage.Korean,
                "ja" => SystemLanguage.Japanese,
                "zh" => SystemLanguage.Chinese, // Note: 'zh' doesn't distinguish between Simplified and Traditional
                "th" => SystemLanguage.Thai,
                "vi" => SystemLanguage.Vietnamese,
                "es" => SystemLanguage.Spanish,
                "pt" => SystemLanguage.Portuguese,
                "fr" => SystemLanguage.French,
                "de" => SystemLanguage.German,
                "it" => SystemLanguage.Italian,
                "pl" => SystemLanguage.Polish,
                "ar" => SystemLanguage.Arabic,
                "fi" => SystemLanguage.Finnish,
                "no" => SystemLanguage.Norwegian,
                "sv" => SystemLanguage.Swedish,
                "ru" => SystemLanguage.Russian,
                "hu" => SystemLanguage.Hungarian,
                "tr" => SystemLanguage.Turkish,
                "hi" => SystemLanguage.Hindi,
                "ro" => SystemLanguage.Romanian,
                "da" => SystemLanguage.Danish,
                "nl" => SystemLanguage.Dutch,
                "cs" => SystemLanguage.Czech,
                "sk" => SystemLanguage.Slovak,
                "el" => SystemLanguage.Greek,
                "bg" => SystemLanguage.Bulgarian,
                //"hr" => SystemLanguage.Croatian,
                "sl" => SystemLanguage.Slovenian,
                "lt" => SystemLanguage.Lithuanian,
                "lv" => SystemLanguage.Latvian,
                "et" => SystemLanguage.Estonian,
                "is" => SystemLanguage.Icelandic,
                //"ms" => SystemLanguage.Malay,
                _ => SystemLanguage.Unknown,
            };

            // Debug.Log($"Parsing ISO code: {localeCode} -> {result}");
            return result;
        }

        public static string ToISOCode(this Locale locale)
        {
            return locale.Value.ToISOCode();
        }

        /// <summary>
        /// IETF Language Tags (e.g., en-US, ja-JP)
        /// </summary>
        public static string ToIETFTag(this SystemLanguage locale)
        {
            string cultureCode = locale switch
            {
                SystemLanguage.English => "en-US",
                SystemLanguage.Korean => "ko-KR",
                SystemLanguage.Japanese => "ja-JP",
                SystemLanguage.Chinese => "zh-CN",
                SystemLanguage.ChineseSimplified => "zh-CN",
                SystemLanguage.ChineseTraditional => "zh-TW",
                SystemLanguage.Thai => "th-TH",
                SystemLanguage.Vietnamese => "vi-VN",
                SystemLanguage.Spanish => "es-ES",
                SystemLanguage.Portuguese => "pt-PT",
                SystemLanguage.French => "fr-FR",
                SystemLanguage.German => "de-DE",
                SystemLanguage.Italian => "it-IT",
                SystemLanguage.Polish => "pl-PL",
                SystemLanguage.Arabic => "ar-SA",
                SystemLanguage.Finnish => "fi-FI",
                SystemLanguage.Norwegian => "nb-NO",
                SystemLanguage.Swedish => "sv-SE",
                SystemLanguage.Russian => "ru-RU",
                SystemLanguage.Hungarian => "hu-HU",
                SystemLanguage.Hindi => "hi-IN",
                _ => "auto",
            };

            return cultureCode;
        }

        public static string ToIETFTag(this Locale locale)
        {
            return locale.Value.ToIETFTag();
        }

        /// <summary>
        /// IETF Language Tags (e.g., en-US, ja-JP)
        /// </summary>
        public static SystemLanguage ParseIETFTag(string localeCode)
        {
            return localeCode switch
            {
                "en-US" => SystemLanguage.English,
                "ko-KR" => SystemLanguage.Korean,
                "ja-JP" => SystemLanguage.Japanese,
                "zh-CN" => SystemLanguage.ChineseSimplified,
                "zh-TW" => SystemLanguage.ChineseTraditional,
                "th-TH" => SystemLanguage.Thai,
                "vi-VN" => SystemLanguage.Vietnamese,
                "es-ES" => SystemLanguage.Spanish,
                "pt-PT" => SystemLanguage.Portuguese,
                "fr-FR" => SystemLanguage.French,
                "de-DE" => SystemLanguage.German,
                "it-IT" => SystemLanguage.Italian,
                "pl-PL" => SystemLanguage.Polish,
                "ar-SA" => SystemLanguage.Arabic,
                "fi-FI" => SystemLanguage.Finnish,
                "nb-NO" => SystemLanguage.Norwegian,
                "sv-SE" => SystemLanguage.Swedish,
                "ru-RU" => SystemLanguage.Russian,
                "hu-HU" => SystemLanguage.Hungarian,
                _ => SystemLanguage.Unknown,
            };
        }

        public static string ToGoogleLocaleCode(this SystemLanguage language)
        {
            return language switch
            {
                SystemLanguage.Italian => "it",
                SystemLanguage.Arabic => "ar",
                SystemLanguage.Japanese => "ja",
                SystemLanguage.Korean => "ko",
                SystemLanguage.ChineseSimplified => "zh-CN",
                SystemLanguage.ChineseTraditional => "zh-TW",
                SystemLanguage.Norwegian => "no",
                SystemLanguage.Polish => "pl",
                SystemLanguage.Portuguese => "pt",
                SystemLanguage.English => "en",
                SystemLanguage.Russian => "ru",
                SystemLanguage.Finnish => "fi",
                SystemLanguage.French => "fr",
                SystemLanguage.Spanish => "es",
                SystemLanguage.Swedish => "sv",
                SystemLanguage.German => "de",
                SystemLanguage.Thai => "th",
                SystemLanguage.Hungarian => "hu",
                SystemLanguage.Vietnamese => "vi",
                _ => "auto",
            };
        }

        public static string ToGoogleLocaleCode(this Locale locale)
        {
            return locale.Value.ToGoogleLocaleCode();
        }

        public static SystemLanguage ParseGoogleLocaleCode(string language)
        {
            return language switch
            {
                "it" => SystemLanguage.Italian,
                "ar" => SystemLanguage.Arabic,
                "ja" => SystemLanguage.Japanese,
                "ko" => SystemLanguage.Korean,
                "zh-CN" => SystemLanguage.ChineseSimplified,
                "zh-TW" => SystemLanguage.ChineseTraditional,
                "no" => SystemLanguage.Norwegian,
                "pl" => SystemLanguage.Polish,
                "pt" => SystemLanguage.Portuguese,
                "en" => SystemLanguage.English,
                "ru" => SystemLanguage.Russian,
                "fi" => SystemLanguage.Finnish,
                "fr" => SystemLanguage.French,
                "es" => SystemLanguage.Spanish,
                "sv" => SystemLanguage.Swedish,
                "de" => SystemLanguage.German,
                "th" => SystemLanguage.Thai,
                "hu" => SystemLanguage.Hungarian,
                "vi" => SystemLanguage.Vietnamese,
                _ => SystemLanguage.Unknown,
            };
        }

        public static List<Locale> ToLocales(this List<SystemLanguage> languages)
        {
            return languages.Select(x => new Locale(x)).ToList();
        }

        public static bool TryParse(string smartString, out Locale locale)
        {
            smartString = smartString.Trim();

            // Check if it has a space or a comma
            if (smartString.Contains(" ") || smartString.Contains(","))
            {
                // Split the string by space or comma and take the first part
                string[] split = smartString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Try to parse with the first part
                locale = ParsePart(split[0]);
                if (locale != Locale.Unknown) return true;

                // Second part might have "(" and ")" or other characters
                // Remove them and try to parse again
                if (split.Length > 1)
                {
                    locale = ParsePart(split[1].Trim('(', ')'));
                    return locale != Locale.Unknown;
                }
            }

            locale = ParsePart(smartString);
            if (locale != Locale.Unknown) return true;
            Debug.LogError($"Failed to parse locale from string: {smartString}");
            return false;
        }

        private static Locale ParsePart(string part)
        {
            if (Enum.TryParse(part, true, out SystemLanguage language))
            {
                return new Locale(language);
            }

            if (part.Contains("-") || part.Contains("_"))
            {
                string[] split = part.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    split[0] = split[0].ToLower();
                    split[1] = split[1].ToUpper();
                    part = string.Join("-", split);
                    language = ParseIETFTag(part);
                    if (language != SystemLanguage.Unknown) return new Locale(language);
                }
            }
            else if (part.Length == 2)
            {
                part = part.ToLower();
                language = ParseISOCode(part);
                if (language != SystemLanguage.Unknown) return new Locale(language);
            }

            return Locale.Unknown;
        }
    }
}