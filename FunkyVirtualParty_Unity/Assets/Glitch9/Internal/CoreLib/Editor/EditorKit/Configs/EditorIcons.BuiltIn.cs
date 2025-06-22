using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public enum StatusColor
    {
        Gray,
        Green,
        Yellow,
        Red,
    }

    /// <summary>
    /// Built-in Editor Icons
    /// </summary>
    public static partial class EditorIcons
    {
        // Util that gets light/dark icon based on current editor skin 
        private static Texture GetBuiltInIcon(string iconName, bool ignoreDarkMode = false)
        {
            if (ExGUI.IsDarkMode && !ignoreDarkMode) iconName = $"d_{iconName}";

            Texture icon = _iconCache.Get(iconName);
            if (icon == null)
            {
                icon = EditorGUIUtility.IconContent(iconName).image;
                _iconCache.Add(iconName, icon);
            }

            return icon;
        }

        public static Texture Folder => GetBuiltInIcon("Folder Icon");
        public static Texture Refresh => GetBuiltInIcon("Refresh");
        public static Texture TreeEditorRefresh => GetBuiltInIcon("TreeEditor.Refresh");
        public static Texture Reset => Refresh;
        public static Texture PlayButton => GetBuiltInIcon("PlayButton");
        public static Texture PauseButton => GetBuiltInIcon("PauseButton");
        public static Texture StopButton => GetBuiltInIcon("PreMatQuad");
        public static Texture Settings => GetBuiltInIcon("Settings");
        public static Texture Text => GetBuiltInIcon("Text Icon");
        public static Texture BuiltIn => Settings;

        public static Texture TrashDetailed => GetBuiltInIcon("d_TreeEditor.Trash");
        public static Texture Plus => GetBuiltInIcon("Toolbar Plus@2x");
        public static Texture Minus => GetBuiltInIcon("Toolbar Minus@2x");
        public static Texture Neutral => GetBuiltInIcon("d_winbtn_mac_min_h");
        public static Texture ScriptableObjectGray => GetBuiltInIcon("d_ScriptableObject On Icon");
        public static Texture ScriptableObject => GetBuiltInIcon("d_ScriptableObject Icon");
        public static Texture Valid => GetBuiltInIcon("Valid@2x");
        public static Texture Invalid => GetBuiltInIcon("Invalid@2x");
        //public static Texture Info => GetBuiltInIcon("d_console.infoicon.inactive.sml@2x");
        //public static Texture Warning => GetBuiltInIcon("Warning");
        //public static Texture Error => GetBuiltInIcon("Error");
        public static Texture RedQuestionMark => GetBuiltInIcon("P4_Conflicted@2x");
        public static Texture CSScript => GetBuiltInIcon("cs Script Icon");
        public static Texture CGProgram => GetBuiltInIcon("TextScriptImporter Icon");
        public static Texture OrangeLight => GetBuiltInIcon("d_orangeLight");
        public static Texture GreenLight => GetBuiltInIcon("d_greenLight");
        public static Texture Font => GetBuiltInIcon("TrueTypeFontImporter Icon");
        public static Texture Search => GetBuiltInIcon("Search Icon");
        public static Texture Tools => GetBuiltInIcon("SceneViewTools@2x");
        public static Texture Resize => GetBuiltInIcon("PositionAsUV1 Icon");
        public static Texture MacClose => GetBuiltInIcon("winbtn_mac_close_a");
        public static Texture Image => GetBuiltInIcon("Image Icon");
        public static Texture InputField => GetBuiltInIcon("InputField Icon");
        public static Texture Loading => GetBuiltInIcon("Loading", true);
        public static Texture Loading2x => GetBuiltInIcon("Loading@2x", true);
        public static Texture BooScript => GetBuiltInIcon("boo Script Icon");
        public static Texture ToolBarPlusMore => GetBuiltInIcon("Toolbar Plus More");
        public static Texture ToolBarPlusMore2x => GetBuiltInIcon("Toolbar Plus More@2x");
        public static Texture ToolBarPlus => GetBuiltInIcon("Toolbar Plus");
        public static Texture ToolBarPlus2x => GetBuiltInIcon("Toolbar Plus@2x");
        public static Texture ToolBarMinus => GetBuiltInIcon("Toolbar Minus");
        public static Texture ToolBarMinus2x => GetBuiltInIcon("Toolbar Minus@2x");
        public static Texture Menu => GetBuiltInIcon("_Menu");
        public static Texture Menu2x => GetBuiltInIcon("_Menu@2x");
        public static Texture DropDown => GetBuiltInIcon("icon dropdown");
        public static Texture DropDown2x => GetBuiltInIcon("icon dropdown@2x");
        public static Texture Maximize => GetBuiltInIcon("winbtn_win_max_h");
        public static Texture Maximize2x => GetBuiltInIcon("winbtn_win_max_h@2x");
        public static Texture GUISkin => GetBuiltInIcon("GUISkin Icon");
        /// <summary>
        /// Icon looks like branching
        /// </summary>
        public static Texture AnimatorIcon => GetBuiltInIcon("Animator Icon");

        public static Texture AlignHorizontallyCenter => GetBuiltInIcon("align_horizontally_center");
        public static Texture AlignHorizontallyCenterActive => GetBuiltInIcon("align_horizontally_center_active");
        public static Texture AlignHorizontallyLeft => GetBuiltInIcon("align_horizontally_left");
        public static Texture AlignHorizontallyLeftActive => GetBuiltInIcon("align_horizontally_left_active");
        public static Texture AlignHorizontallyRight => GetBuiltInIcon("align_horizontally_right");
        public static Texture AlignHorizontallyRightActive => GetBuiltInIcon("align_horizontally_right_active");

        //AudioChorusFilter Icon
        public static Texture AudioChorusFilter => GetBuiltInIcon("AudioChorusFilter Icon");

        //d_scenepicking_notpickable
        public static Texture ScenePickingNotPickable => GetBuiltInIcon("scenepicking_notpickable");

        //d_AudioClip Icon
        public static Texture AudioListener => GetBuiltInIcon("AudioListener Icon");

        // d_RawImage Icon
        // public static Texture RawImage => GetBuiltInIcon("RawImage Icon");

        // d_Microphone Icon
        public static Texture Microphone => GetBuiltInIcon("Microphone Icon");
        public static Texture GameObject => GetBuiltInIcon("GameObject Icon");

        //d_scenevis_hidden@2x (the eye with a line through it)
        public static Texture Hide => GetBuiltInIcon("scenevis_hidden@2x");

        //d_pick
        public static Texture Pick => GetBuiltInIcon("pick");

        //d_scrollleft_uielements
        public static Texture ScrollLeft => GetBuiltInIcon("scrollleft_uielements");
        //d_scrollright_uielements
        public static Texture ScrollRight => GetBuiltInIcon("scrollright_uielements");

        //AssetLabelIcon
        //public static Texture AssetLabel => GetBuiltInIcon("AssetLabelIcon");

        //P4_DeletedRemote
        public static Texture P4DeletedRemote => GetBuiltInIcon("P4_DeletedRemote");

        //d_ol_minus
        public static Texture OlMinus => GetBuiltInIcon("ol_minus");

        //EditorGUIUtility.IconContent("d_FilterByLabel")
        public static Texture FilterByLabel => GetBuiltInIcon("FilterByLabel");

        //AudioClip Icon
        public static Texture AudioClip => GetBuiltInIcon("AudioClip Icon");
        //Texture Icon
        public static Texture Texture => GetBuiltInIcon("Texture Icon");

        //EditorGUIUtility.IconContent("dropdown_PreviewPackages")
        public static Texture DropdownPreviewPackages => GetBuiltInIcon("dropdown_PreviewPackages");

        //icon dropdown
        public static Texture IconDropdown => GetBuiltInIcon("icon dropdown");

        public static Texture ArrowNavigationRight => GetBuiltInIcon("ArrowNavigationRight", true);
        public static Texture ArrowNavigationRight2x => GetBuiltInIcon("ArrowNavigationRight@2x", true);

        public static Texture ArrowNavigationLeft => GetBuiltInIcon("ArrowNavigationLeft", true);
        public static Texture ArrowNavigationLeft2x => GetBuiltInIcon("ArrowNavigationLeft@2x", true);

        //EditorGUIUtility.IconContent("d_more@2x")
        public static Texture More => GetBuiltInIcon("more@2x");

        //UnityEditor.ConsoleWindow
        public static Texture ConsoleWindow => GetBuiltInIcon("UnityEditor.ConsoleWindow");


        public static Texture ConsoleInfoIcon => GetBuiltInIcon("console.infoicon");
        public static Texture ConsoleWarnIcon => GetBuiltInIcon("console.warnicon");
        public static Texture ConsoleErrorIcon => GetBuiltInIcon("console.erroricon");

        //d_BuildSettings.iPhone
        public static Texture BuildSettingsiPhone => GetBuiltInIcon("BuildSettings.iPhone");
        //d_BuildSettings.Android
        public static Texture BuildSettingsAndroid => GetBuiltInIcon("BuildSettings.Android");
        //d_BuildSettings.Lumin
        public static Texture BuildSettingsLumin => GetBuiltInIcon("BuildSettings.Lumin");

        //cs Script Icon
        public static Texture CsScript => GetBuiltInIcon("cs Script Icon");

        //EditorGUIUtility.IconContent("UxmlScript Icon")
        public static Texture UxmlScript => GetBuiltInIcon("UxmlScript Icon", true);

        //Image Icon
        public static Texture ImageIcon => GetBuiltInIcon("Image Icon");

        //VideoPlayer Icon
        public static Texture VideoPlayer => GetBuiltInIcon("VideoPlayer Icon");

        //EditorGUIUtility.IconContent("PackageBadgeNew")
        public static Texture PackageBadgeNew => GetBuiltInIcon("PackageBadgeNew", true);

        public static Texture StatusLight(StatusColor color)
        {
            return color switch
            {
                StatusColor.Green => GetBuiltInIcon("greenLight"),
                StatusColor.Yellow => GetBuiltInIcon("orangeLight"),
                StatusColor.Red => GetBuiltInIcon("redLight"),
                _ => GetBuiltInIcon("lightOff")
            };
        }
    }
}