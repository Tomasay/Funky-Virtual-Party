using System;
using System.Linq;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor.IMGUI;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public class VoiceCatalogueTreeViewItem : ExtendedTreeViewItem
        <
            VoiceCatalogueTreeViewItem,
            VoiceCatalogueEntry,
            VoiceCatalogueFilter
        >
    {
        private const string UNKNOWN_MODEL_NAME = "Unknown Voice";

        internal Api Api
        {
            get
            {
                if (Data == null) return Api.None;
                return Data.Api;
            }
        }

        internal VoiceGender Gender
        {
            get
            {
                if (Data == null || Data.Gender == null) return VoiceGender.None;
                return Data.Gender.Value;
            }
        }

        internal VoiceType Type
        {
            get
            {
                if (Data == null || Data.Type == null) return VoiceType.None;
                return Data.Type.Value;
            }
        }

        internal VoiceAge Age
        {
            get
            {
                if (Data == null || Data.Age == null) return VoiceAge.None;
                return Data.Age.Value;
            }
        }

        internal SystemLanguage Language
        {
            get
            {
                if (Data == null) return SystemLanguage.Unknown;
                return Data.Language;
            }
        }

        internal VoiceCategory Category
        {
            get
            {
                if (Data == null || Data.Category == null) return VoiceCategory.None;
                return Data.Category.Value;
            }
        }

        internal string Accent
        {
            get
            {
                if (Data == null) return null;
                return Data.Accent;
            }
        }

        internal string PreviewUrl
        {
            get
            {
                if (Data == null) return null;
                return Data.PreviewUrl;
            }
        }

        internal string PreviewPath
        {
            get
            {
                if (Data == null) return null;
                return Data.PreviewPath;
            }
        }

        internal string Name
        {
            get
            {
                if (Data == null) return UNKNOWN_MODEL_NAME;
                return Data.Name;
            }
        }

        internal string Id
        {
            get
            {
                if (Data == null) return null;
                return Data.Id;
            }
        }

        internal UnixTime CreatedAt
        {
            get
            {
                if (Data == null || Data.CreatedAt == null) return UnixTime.MinValue;
                return Data.CreatedAt.Value;
            }
        }

        internal string OwnedBy
        {
            get
            {
                if (Data == null || string.IsNullOrEmpty(Data.OwnedBy)) return "Unknown";
                return Data.OwnedBy;
            }
        }

        internal bool IsFree
        {
            get
            {
                if (Data == null) return false;
                return Data.IsFree == true;
            }
        }

        internal bool IsFeatured
        {
            get
            {
                if (Data == null) return false;
                return Data.IsFeatured == true;
            }
        }

        internal string Description
        {
            get
            {
                if (Data == null) return "-";
                return Data.Description;
            }
        }

        internal bool IsCustom
        {
            get
            {
                if (Data == null) return false;
                return Data.IsCustom;
            }
        }

        internal bool IsNew
        {
            get
            {
                if (_isNew == null)
                {
                    // check if this model is released within the last 30 days
                    if (CreatedAt == null)
                    {
                        _isNew = false;
                    }
                    else
                    {
                        var timeDiff = DateTime.UtcNow - CreatedAt.ToDateTime();
                        _isNew = timeDiff.TotalDays < 30;
                    }
                }
                return _isNew.Value;
            }
        }

        internal bool IsDefault
        {
            get
            {
                if (_isDefault == null)
                {
                    if (string.IsNullOrEmpty(Id))
                    {
                        _isDefault = false;
                    }
                    else
                    {
                        _isDefault = AIDevKitConfig.SystemDefaultVoices.Contains(Id);

                        if (_isDefault == false) // check if it's a user defined default voice
                        {
                            _isDefault = Api switch
                            {
                                Api.OpenAI => OpenAISettings.IsDefaultVoice(Id),
                                Api.ElevenLabs => ElevenLabsSettings.IsDefaultVoice(Id),
                                _ => (bool?)false,
                            };
                        }
                    }
                }

                return _isDefault.Value;
            }
        }

        internal readonly bool IsLocked;

        internal bool CanDelete => !IsDefault && !IsLocked;

        private bool? _isNew;
        private bool? _isDefault;

        internal bool InMyLibrary { get; set; }

        internal string LanguageDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(_languageDisplay))
                {
                    using (StringBuilderPool.Get(out var sb))
                    {
                        sb.Append(Data.Language.ToString());

                        bool localeExists = !string.IsNullOrEmpty(Data.Locale);
                        bool accentExists = !string.IsNullOrEmpty(Data.Accent);
                        bool metadataExists = localeExists || accentExists;

                        if (metadataExists) sb.Append(" (");
                        if (localeExists) sb.Append(Data.Locale);
                        if (accentExists)
                        {
                            if (localeExists) sb.Append(", ");
                            sb.Append(Data.Accent.ToTitleCase());
                            sb.Append(" Accent");
                        }
                        if (metadataExists) sb.Append(")");

                        _languageDisplay = sb.ToString();
                    }
                }
                return _languageDisplay;
            }
        }
        private string _languageDisplay;

        internal bool IsDeprecated
        {
            get
            {
                if (Data == null) return false;
                return Data.IsDeprecated == true;
            }
        }


        public VoiceCatalogueTreeViewItem(int id, int depth, string displayName, VoiceCatalogueEntry data) : base(id, depth, displayName, data)
        {
            try
            {
                InMyLibrary = !string.IsNullOrEmpty(data.Id) && VoiceLibrary.Contains(data.Id);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize VoiceLibraryTreeViewItem.HasThisVoice: {e}");
                InMyLibrary = false;
            }
        }

        public override int CompareTo(VoiceCatalogueTreeViewItem anotherItem, int columnIndex, bool ascending)
        {
            try
            {
                return columnIndex switch
                {
                    VoiceCatalogueWindow.ColumnIndex.API => CompareByString(ascending, anotherItem, data => data.Api.ToString()),
                    VoiceCatalogueWindow.ColumnIndex.NAME => CompareByString(ascending, anotherItem, data => data.Name),
                    VoiceCatalogueWindow.ColumnIndex.GENDER => CompareByString(ascending, anotherItem, data => data.Gender.ToString()),
                    VoiceCatalogueWindow.ColumnIndex.TYPE => CompareByString(ascending, anotherItem, data => data.Type.ToString()),
                    VoiceCatalogueWindow.ColumnIndex.AGE => CompareByString(ascending, anotherItem, data => data.Age.ToString()),
                    _ => 0
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return 0;
            }
        }

        public override bool Search(string searchString)
        {
            if (Data == null) return false;
            if (string.IsNullOrEmpty(searchString)) return true;
            if (!string.IsNullOrEmpty(Id) && Id.Contains(searchString)) return true;
            if (!string.IsNullOrEmpty(Name) && Name.Contains(searchString)) return true;
            return false;
        }
    }
}
