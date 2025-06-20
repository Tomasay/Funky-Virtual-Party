using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal static class EditorColors
    {
        private static readonly Dictionary<string, Color> _cache = new();
        internal const string kEditorBlueHex = "#4592ff";
        internal static string LinkLabelHex => EditorStyles.linkLabel.normal.textColor.ToHex();
        internal const string kEditorOrangeHex = "#ff6e40";

#pragma warning disable IDE1006
        internal static Color textColor => GetWithComplementary(nameof(textColor), Color.black);
        internal static Color gray => GetWithComplementary(nameof(gray), new Color(0.3f, 0.3f, 0.3f));
        internal static Color blue => Get(nameof(blue), kEditorBlueHex.ToColor());
        internal static Color linkLabel => Get(nameof(linkLabel) + "v1", EditorStyles.linkLabel.normal.textColor);
        internal static Color orange => Get(nameof(orange), kEditorOrangeHex.ToColor());

#pragma warning disable IDE1006

        #region utility
        private static Color GetComplementaryColor(Color color) => new(1 - color.r, 1 - color.g, 1 - color.b, color.a);

        private static Color GetWithComplementary(string key, Color lightColor)
        {
            if (_cache.TryGetValue(key, out Color color)) return color;
            color = EditorGUIUtility.isProSkin ? GetComplementaryColor(lightColor) : lightColor;
            _cache.Add(key, color);
            return color;
        }

        private static Color Get(string key, Color unknownColor)
        {
            if (_cache.TryGetValue(key, out Color color)) return color;
            color = unknownColor;
            _cache.Add(key, color);
            return color;
        }

        private static Color Get(string key, Color lightColor, Color darkColor)
        {
            if (_cache.TryGetValue(key, out Color color)) return color;
            color = EditorGUIUtility.isProSkin ? darkColor : lightColor;
            _cache.Add(key, color);
            return color;
        }

        #endregion utility 
    }
}