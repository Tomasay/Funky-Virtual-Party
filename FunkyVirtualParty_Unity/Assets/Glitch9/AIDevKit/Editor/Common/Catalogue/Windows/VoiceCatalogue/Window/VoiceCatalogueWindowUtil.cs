using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class VoiceCatalogueWindowUtil
    {
        private static List<SystemLanguage> _availableLanguages;
        private static string[] _availableLanguegesDisplayNames;
        private static List<Api> _availableApis;

        internal static List<SystemLanguage> GetAvailableLanguages()
        {
            if (_availableLanguages == null)
            {
                List<string> displayNames = new() {
                    "All", // as SystemLanguage.Unknown
                };

                _availableLanguages = new()
                {
                    SystemLanguage.Unknown
                };

                foreach (var entry in VoiceCatalogue.Instance.Entries)
                {
                    if (entry == null) continue;
                    var language = entry.Language;

                    if (!_availableLanguages.Contains(language))
                    {
                        _availableLanguages.Add(language);
                        displayNames.Add(language.ToString());
                    }
                }

                _availableLanguegesDisplayNames = displayNames.ToArray();
            }

            return _availableLanguages;
        }

        internal static string[] GetAvailableLanguagesDisplayNames()
        {
            if (_availableLanguegesDisplayNames == null) GetAvailableLanguages();
            return _availableLanguegesDisplayNames;
        }

        internal static List<Api> GetAvailableApis()
        {
            if (_availableApis == null)
            {
                _availableApis = new List<Api>() { Api.All };
                foreach (var entry in VoiceCatalogue.Instance.Entries)
                {
                    if (entry == null) continue;
                    var provider = entry.Api;

                    if (!_availableApis.Contains(provider))
                    {
                        _availableApis.Add(provider);
                    }
                }
            }

            return _availableApis;
        }
    }
}