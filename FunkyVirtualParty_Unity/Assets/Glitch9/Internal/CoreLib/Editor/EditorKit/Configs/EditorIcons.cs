using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Custom Editor Icons
    /// </summary>
    public static partial class EditorIcons
    {
        //private const string kIconPath = "{0}/CoreLib/Editor/Common/Gizmos/Icons/";
        private const string kMarkerFileName = "corelib_editor_icons";
        //private static readonly EditorTextureCache<Texture> _iconCache = new(string.Format(kIconPath, EditorPathUtil.FindGlitch9Path()));
        private static readonly EditorTextureCache<Texture> _iconCache = EditorTextureCache<Texture>.WithMarkerFile(kMarkerFileName);


        public static class Media
        {
            public static Texture Next => _iconCache.Get("skip_next_black36dp.png");
            public static Texture Previous => _iconCache.Get("skip_previous_black36dp.png");
            public static Texture FastForwar => _iconCache.Get("fast_forward_black36dp.png");
            public static Texture Rewind => _iconCache.Get("fast_rewind_black36dp.png");
            public static Texture Volume => _iconCache.Get("volumebar.png");
            public static Texture SeekBar => _iconCache.Get("seekbar.png");
            public static Texture Shuffle => _iconCache.Get("baseline_shuffle_black36.png");
            public static Texture Repeat => _iconCache.Get("baseline_repeat_black36.png");
            public static Texture RepeatOne => _iconCache.Get("baseline_repeat_one_black36.png");
        }

        public static class Routina
        {
            public static Texture Tuto => _iconCache.Get("circle_tuto.png");
            public static Texture Aimi => _iconCache.Get("circle_aimi.png");
        }

        public static Texture Collapse => _iconCache.Get("16_up.png");
        public static Texture Expand => _iconCache.Get("16_down.png");
        public static Texture Dot => _iconCache.Get("16_dot.png");
        public static Texture Key => _iconCache.Get("key.png");


        // File
        public static Texture AddFile => _iconCache.Get("add-file.png");
        public static Texture DeleteFile => _iconCache.Get("delete-file.png");
        public static Texture EditFile => _iconCache.Get("edit-file.png");
        public static Texture ExportCSV => _iconCache.Get("export-csv.png");
        public static Texture ImportCSV => _iconCache.Get("import-csv.png");
        public static Texture UpdateFile => _iconCache.Get("update-file.png");
        public static Texture FileDelete => _iconCache.Get("file-delete.png");
        public static Texture FileSettings => _iconCache.Get("file-settings.png");
        public static Texture OpenFolder => _iconCache.Get("open_folder.png");
        public static Texture Edit => _iconCache.Get("edit-file.png");

        // Editing
        public static Texture Add => _iconCache.Get("add.png");
        public static Texture Clear => _iconCache.Get("clean.png");
        public static Texture Delete => _iconCache.Get("delete.png");
        public static Texture Check => _iconCache.Get("done.png");
        public static Texture Export => _iconCache.Get("export.png");
        public static Texture Import => _iconCache.Get("import.png");
        public static Texture Language => _iconCache.Get("language.png");
        public static Texture Question => _iconCache.Get("question.png");
        public static Texture Quotes => _iconCache.Get("quotes.png");
        public static Texture Save => _iconCache.Get("save.png");
        public static Texture Start => _iconCache.Get("start.png");
        public static Texture Translate => _iconCache.Get("translate.png");
        public static Texture FindAndReplace => _iconCache.Get("find_and_replace.png");
        public static Texture Replace => _iconCache.Get("replace.png");
        public static Texture Group => _iconCache.Get("group.png");
        // Extra
        public static Texture Information => _iconCache.Get("information.png");
        public static Texture Phone => _iconCache.Get("phone.psd");
        public static Texture List => _iconCache.Get("add-list.png");
        public static Texture AI => _iconCache.Get("ai.png");
        public static Texture Trash => _iconCache.Get("Delete.png");
        public static Texture NoImageHighRes => _iconCache.Get("no-image-high-res.png");
        public static Texture History => _iconCache.Get("history.png");

        // Custom Editor Status (Added on 2024.06.23)
        public static Texture StatusCheck => _iconCache.Get("status_check.png");
        public static Texture StatusError => _iconCache.Get("status_error.png");
        public static Texture StatusWarning => _iconCache.Get("status_warning.png");
        public static Texture StatusObsolete => _iconCache.Get("status_obsolete.png");

        public static Texture LightModeOn => _iconCache.Get("lightmode_on.png");
        public static Texture LightModeOff => _iconCache.Get("lightmode_off.png");
        public static Texture DarkModeOn => _iconCache.Get("darkmode_on.png");
        public static Texture DarkModeOff => _iconCache.Get("darkmode_off.png");
        public static Texture LightModeSwitch => _iconCache.Get("lightmode_switch.png");
        public static Texture ProBadge => _iconCache.Get("pro_version.png");
        public static Texture NewBadge => _iconCache.Get("badge_new.psd");
        public static Texture Clipboard => _iconCache.Get("clipboard.png");
        public static Texture Copy => _iconCache.Get("copy.png");
        public static Texture EditOutline => _iconCache.Get("edit-outline.png");
        public static Texture Generate => _iconCache.Get("generate.png");
        public static Texture Close => _iconCache.Get("close.png");
        public static Texture Portrait => _iconCache.Get("portrait.png");
        public static Texture LightBulb => _iconCache.Get("bulb.png");
        public static Texture Lightning => _iconCache.Get("lightning.png");
        public static Texture Archive => _iconCache.Get("archive.png");
    }
}
