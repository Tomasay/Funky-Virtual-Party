using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public static partial class EditorTextures
    {
        //private const string kTextureDir = "{0}/CoreLib/Editor/Common/Gizmos/Textures/";
        //private static readonly EditorTextureCache<Texture2D> _textureCache = new(string.Format(kTextureDir, EditorPathUtil.FindGlitch9Path()));
        private const string kMarkerFileName = "corelib_editor_textures";
        private static readonly EditorTextureCache<Texture2D> _textureCache = EditorTextureCache<Texture2D>.WithMarkerFile(kMarkerFileName);

        public static Texture2D Box(EditorColor color = 0)
        {
            return color switch
            {
                EditorColor.Green => BoxGreen,
                EditorColor.Yellow => BoxYellow,
                EditorColor.Orange => BoxOrange,
                EditorColor.Purple => BoxPurple,
                EditorColor.Blue => BoxBlue,
                EditorColor.White => EditorGUIUtility.whiteTexture,
                _ => BoxDefault,
            };
        }

        public static Texture2D Button(EditorColor color, bool isSelected)
        {
            return (color, selected: isSelected) switch
            {
                (EditorColor.Green, true) => BtnGreenSelected,
                (EditorColor.Purple, true) => BtnPurpleSelected,
                (EditorColor.Yellow, true) => BtnYellowSelected,
                (EditorColor.Blue, true) => BtnBlueSelected,
                (EditorColor.Orange, true) => BtnOrangeSelected,
                (EditorColor.Red, true) => BtnRedSelected,

                (EditorColor.Green, false) => BtnGreen,
                (EditorColor.Purple, false) => BtnPurple,
                (EditorColor.Yellow, false) => BtnYellow,
                (EditorColor.Blue, false) => BtnBlue,
                (EditorColor.Orange, false) => BtnOrange,
                (EditorColor.Red, false) => BtnRed,

                (EditorColor.None, true) => BtnDefaultSelected,
                _ => BtnDefault,
            };
        }

        public static Texture2D BoxDefault => _textureCache.Get("section-box.psd");
        public static Texture2D BoxGreen => _textureCache.Get("section-box-green.psd");
        public static Texture2D BoxYellow => _textureCache.Get("section-box-yellow.psd");
        public static Texture2D BoxOrange => _textureCache.Get("section-box-orange.psd");
        public static Texture2D BoxPurple => _textureCache.Get("section-box-purple.psd");
        public static Texture2D BoxBlue => _textureCache.Get("section-box-blue.psd");

        public static Texture2D BtnDefault => _textureCache.Get("btn-default.psd");
        public static Texture2D BtnGreen => _textureCache.Get("btn-green.psd");
        public static Texture2D BtnPurple => _textureCache.Get("btn-purple.psd");
        public static Texture2D BtnYellow => _textureCache.Get("btn-yellow.psd");
        public static Texture2D BtnBlue => _textureCache.Get("btn-blue.psd");
        public static Texture2D BtnOrange => _textureCache.Get("btn-orange.psd");
        public static Texture2D BtnRed => _textureCache.Get("btn-red.psd");

        public static Texture2D BtnDefaultSelected => _textureCache.Get("btn-default-selected.psd");
        public static Texture2D BtnGreenSelected => _textureCache.Get("btn-green-selected.psd");
        public static Texture2D BtnPurpleSelected => _textureCache.Get("btn-purple-selected.psd");
        public static Texture2D BtnYellowSelected => _textureCache.Get("btn-yellow-selected.psd");
        public static Texture2D BtnBlueSelected => _textureCache.Get("btn-blue-selected.psd");
        public static Texture2D BtnOrangeSelected => _textureCache.Get("btn-orange-selected.psd");
        public static Texture2D BtnRedSelected => _textureCache.Get("btn-red-selected.psd");


        public static Texture2D ToolBarButtonOn => _textureCache.Get("btn-mid-on.psd");
        public static Texture2D ToolBarButtonOff => _textureCache.Get("btn-mid-off.psd");
        public static Texture2D BorderTop => _textureCache.Get("section-border-top.psd");
        public static Texture2D BorderBottom => _textureCache.Get("section-border-bottom.psd");
        public static Texture2D BorderTopBottom => _textureCache.Get("section-border-top-bottom.psd");
        public static Texture2D BorderBottomWithBlueLine => _textureCache.Get("section-border-bottom-blueline.psd");
        public static Texture2D Background => _textureCache.Get("section-background.psd");


        // Config
        public static Texture2D ToggleDescriptionOn => _textureCache.Get("toggle_description_on.psd");
        public static Texture2D ToggleDescriptionOff => _textureCache.Get("toggle_description_off.psd");
        public static Texture2D ToggleIconOn => _textureCache.Get("toggle_icon_on.psd");
        public static Texture2D ToggleIconOff => _textureCache.Get("toggle_icon_off.psd");
        public static Texture2D ToggleLinebreakOn => _textureCache.Get("toggle_linebreakon.psd");
        public static Texture2D ToggleLinebreakOff => _textureCache.Get("toggle_linebreakoff.psd");
        public static Texture2D ToggleNextlineOn => _textureCache.Get("toggle_nextline_on.psd");
        public static Texture2D ToggleNextlineOff => _textureCache.Get("toggle_nextline_off.psd");

        public static Texture2D iOSCircle => _textureCache.Get("ios13_circle.psd");
        public static Texture2D iOSRoundedCorners => _textureCache.Get("ios13_rounded_corners.psd");

        public static Texture2D LoadingSpinner => _textureCache.Get("loading.png");

        public static Texture2D ResizeHandle => _textureCache.Get("resize_handle.png");

        public static Texture2D AnimatedToggleOn => _textureCache.Get("animated-toggle-on.psd");
        public static Texture2D AnimatedToggleOff => _textureCache.Get("animated-toggle-off.psd");
        public static Texture2D AnimatedToggleHandle => _textureCache.Get("animated-toggle-handle.psd");
        public static Texture2D Discord => _textureCache.Get("discord.png");
        public static Texture2D Preferences => _textureCache.Get("preferences.png");
        public static Texture2D Reload => _textureCache.Get("reload.png");
        public static Texture2D Info => _textureCache.Get("information.png");

        // Code Block
        public static Texture2D CodeBlockBodyTexture => _textureCache.Get("codeblock-body.psd");
        public static Texture2D CodeBlockHeaderTexture => _textureCache.Get("codeblock-header.psd");

        private static readonly Dictionary<string, Texture2D> _textures = new();
        private const float DARKEN_FACTOR = 0.5f;

        private static Texture2D GetTexture(string key)
        {
            return _textures.GetValueOrDefault(key);
        }

        private static Texture2D CreateColorTexture(string key, Color color)
        {
            Texture2D texture = ExGUIUtility.CreateTexture(2, 2, color);
            _textures.Add(key, texture);
            return texture;
        }

        public static Texture2D GetColorTexture(EditorColor color)
        {
            return color switch
            {
                EditorColor.Green => greenTexture,
                EditorColor.Purple => purpleTexture,
                EditorColor.Yellow => yellowTexture,
                EditorColor.Blue => blueTexture,
                EditorColor.Red => redTexture,
                EditorColor.Orange => orangeTexture,
                EditorColor.White => EditorGUIUtility.whiteTexture,
                _ => grayTexture
            };
        }

        private static Color CreateEditorColor(float rgb)
        {
            float multiplier = EditorGUIUtility.isProSkin ? DARKEN_FACTOR : 1f;
            return new Color(rgb * multiplier, rgb * multiplier, rgb * multiplier, 1f);
        }

        private static Color CreateEditorColor(float r, float g, float b, float a = 1f)
        {
            float multiplier = EditorGUIUtility.isProSkin ? DARKEN_FACTOR : 1f;
            return new Color(r * multiplier, g * multiplier, b * multiplier, a);
        }

        private static Texture2D ResolveTexture(string key, Color color)
        {
            Texture2D texture = GetTexture(key);
            if (texture != null) return texture;
            return CreateColorTexture(key, color);
        }

#pragma warning disable IDE1006
        public static Texture2D grayTexture => ResolveTexture(nameof(grayTexture), CreateEditorColor(0.8f));
        public static Texture2D darkGrayTexture => ResolveTexture(nameof(darkGrayTexture), CreateEditorColor(EditorGUIUtility.isProSkin ? 0.45f : 0.8f));
        public static Texture2D darkerGrayTexture => ResolveTexture(nameof(darkerGrayTexture), CreateEditorColor(EditorGUIUtility.isProSkin ? 0.3f : 0.65f));
        public static Texture2D borderTexture => ResolveTexture(nameof(borderTexture), CreateEditorColor(0.6f));
        public static Texture2D greenTexture => ResolveTexture(nameof(greenTexture), CreateEditorColor(0.75f, 0.9f, 0.75f));
        public static Texture2D purpleTexture => ResolveTexture(nameof(purpleTexture), CreateEditorColor(0.75f, 0.75f, 0.9f));
        public static Texture2D yellowTexture => ResolveTexture(nameof(yellowTexture), CreateEditorColor(0.9f, 0.9f, 0.75f));
        public static Texture2D blueTexture => ResolveTexture(nameof(blueTexture), CreateEditorColor(0.6f, 0.75f, 0.9f));
        public static Texture2D blackTexture => ResolveTexture(nameof(blackTexture), CreateEditorColor(0f));
        public static Texture2D redTexture => ResolveTexture(nameof(redTexture), CreateEditorColor(0.9f, 0.75f, 0.75f));
        public static Texture2D orangeTexture => ResolveTexture(nameof(orangeTexture), CreateEditorColor(0.9f, 0.75f, 0.6f));
        public static Texture2D transparentPinkTexture =>
            ResolveTexture(nameof(transparentPinkTexture), CreateEditorColor(0.9f, 0.4f, 0.8f, 0.2f));
        public static Texture2D transparentBlueTexture =>
            ResolveTexture(nameof(transparentBlueTexture), CreateEditorColor(0.4f, 0.5f, 1f, 0.2f));
        public static Texture2D transparentGreenTexture =>
            ResolveTexture(nameof(transparentGreenTexture), CreateEditorColor(0.4f, 0.75f, 0.6f, 0.2f));
        public static Texture2D transparentPurpleTexture =>
            ResolveTexture(nameof(transparentPurpleTexture), CreateEditorColor(0.65f, 0.5f, 0.9f, 0.2f));

#pragma warning restore IDE1006

    }
}