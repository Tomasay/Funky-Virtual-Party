using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public class VoiceCatalogueFilter : TreeViewItemFilter
    {
        private static readonly EPrefs<Api> kApi = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kApi)}", Api.None);
        private static readonly EPrefs<VoiceCategory> kCategory = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kCategory)}", VoiceCategory.None);
        private static readonly EPrefs<VoiceGender> kGender = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kGender)}", VoiceGender.None);
        private static readonly EPrefs<VoiceType> kType = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kType)}", VoiceType.None);
        private static readonly EPrefs<VoiceAge> kAge = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kAge)}", VoiceAge.None);
        private static readonly EPrefs<SystemLanguage> kLanguage = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kLanguage)}", SystemLanguage.Unknown);
        private static readonly EPrefs<bool> kFeatured = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kFeatured)}", false);

        private static readonly EPrefs<bool> kOfficial = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kOfficial)}", false);
        private static readonly EPrefs<bool> kCustom = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kCustom)}", false);
        private static readonly EPrefs<bool> kShowCustom = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kShowCustom)}", false);

        private static readonly EPrefs<bool> kInMyLibrary = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kInMyLibrary)}", false);
        private static readonly EPrefs<bool> kNotInMyLibrary = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kNotInMyLibrary)}", false);
        private static readonly EPrefs<bool> kDeprecated = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kDeprecated)}", false);
        private static readonly EPrefs<bool> kDefault = new($"AIDevKit.VoiceCatalogue.Filter.{nameof(kDefault)}", false);

        internal static Api Api { get => kApi.Value; set => kApi.Value = value; }
        internal static VoiceCategory Category { get => kCategory.Value; set => kCategory.Value = value; }
        internal static VoiceGender Gender { get => kGender.Value; set => kGender.Value = value; }
        internal static VoiceType Type { get => kType.Value; set => kType.Value = value; }
        internal static VoiceAge Age { get => kAge.Value; set => kAge.Value = value; }
        internal static SystemLanguage Language { get => kLanguage.Value; set => kLanguage.Value = value; }
        internal static bool Featured { get => kFeatured.Value; set => kFeatured.Value = value; }

        internal static bool InMyLibrary { get => kInMyLibrary.Value; set => kInMyLibrary.Value = value; }
        internal static bool NotInMyLibrary { get => kNotInMyLibrary.Value; set => kNotInMyLibrary.Value = value; }
        internal static bool Official { get => kOfficial.Value; set => kOfficial.Value = value; }
        internal static bool Custom { get => kCustom.Value; set => kCustom.Value = value; }
        internal static bool Deprecated { get => kDeprecated.Value; set => kDeprecated.Value = value; }
        internal static bool Default { get => kDefault.Value; set => kDefault.Value = value; }


        internal static bool ShowCustom { get => kShowCustom.Value; set => kShowCustom.Value = value; }


        public override bool IsVisible(TreeViewItem item)
        {
            if (item is VoiceCatalogueTreeViewItem i)
            {
                if (!ShowCustom && i.IsCustom) return false;

                if (Api != Api.All && Api != i.Api) return false;
                if (Category != VoiceCategory.None && Category != i.Category) return false;
                if (Gender != VoiceGender.None && Gender != i.Gender) return false;
                if (Type != VoiceType.None && Type != i.Type) return false;
                if (Age != VoiceAge.None && Age != i.Age) return false;
                if (Language != SystemLanguage.Unknown && Language != i.Language) return false;

                if (Featured && !i.IsFeatured) return false;
                if (Deprecated && !i.IsDeprecated) return false;
                if (InMyLibrary && !i.InMyLibrary) return false;
                if (NotInMyLibrary && i.InMyLibrary) return false;

                if (Official && i.IsCustom) return false;
                if (Custom && !i.IsCustom) return false;
                if (Default && !i.IsDefault) return false;
            }

            return base.IsVisible(item);
        }
    }
}