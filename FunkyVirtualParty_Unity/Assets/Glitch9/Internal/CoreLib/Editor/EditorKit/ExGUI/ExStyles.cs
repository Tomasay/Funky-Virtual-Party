using UnityEditor;
using UnityEngine;
// ReSharper disable All

namespace Glitch9.Editor
{
    internal static class ExStyles
    {
        private static readonly IMGUIStyleCache _cache = new();
        private static readonly RectOffset _defaultBoxMargin = new(2, 2, 2, 2);
        private static readonly RectOffset _defaultBoxPadding = new(2, 2, 2, 2);

#pragma warning disable IDE1006
        internal static GUIStyle label => _cache.Get(nameof(label), new GUIStyle(GUI.skin.label)
        {
            padding = new RectOffset(2, 2, 2, 2),
        });

        internal static GUIStyle bigBoldLabel => _cache.Get(nameof(bigBoldLabel), new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            padding = new RectOffset(2, 2, 2, 2),
        });

        internal static GUIStyle clippedLabel => _cache.Get(nameof(clippedLabel), new GUIStyle(EditorStyles.label)
        {
            wordWrap = false,
            clipping = TextClipping.Clip,
        });

        internal static GUIStyle statusBoxBigText => _cache.Get(nameof(statusBoxBigText), new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
        });

        internal static GUIStyle statusBoxText => _cache.Get(nameof(statusBoxText), new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            wordWrap = true,
        });

        internal static readonly GUIStyle textField = new(GUI.skin.textField)
        {
            border = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(4, 4, 4, 4),
            overflow = new RectOffset(4, 2, 0, -2),
            wordWrap = true,
        };

        internal static readonly GUIStyle wordWrappedTextField = new(GUI.skin.textField)
        {
            wordWrap = true,
        };

        internal static readonly GUIStyle paddedTextField = new(GUI.skin.textField)
        {
            padding = new RectOffset(10, 10, 10, 10),
            wordWrap = true,
        };

        internal static GUIStyle boxedSection => _cache.Get(nameof(boxedSection), new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(0, 0, 0, 0)
        });

        internal static GUIStyle helpBoxedSection => _cache.Get(nameof(helpBoxedSection), new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(0, 0, 0, 0)
        });

        internal static GUIStyle centeredRedMiniLabel => _cache.Get(nameof(centeredRedMiniLabel), new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.red }
        });

        internal static GUIStyle centeredBoldLabel => _cache.Get(nameof(centeredBoldLabel), new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        });

        internal static GUIStyle centeredMiniBoldLabel => _cache.Get(nameof(centeredMiniBoldLabel), new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter
        });

        internal static GUIStyle centeredGreyMiniLabel => _cache.Get(nameof(centeredGreyMiniLabel), new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            alignment = TextAnchor.MiddleCenter
        });

        internal static GUIStyle centeredBlueBoldLabel => _cache.Get(nameof(centeredBlueBoldLabel), new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = EditorColors.linkLabel }
        });

        internal static GUIStyle paddedArea => _cache.Get(nameof(paddedArea), new GUIStyle
        {
            padding = new RectOffset(7, 7, 7, 7),
        });

        internal static GUIStyle popupBody => _cache.Get(nameof(popupBody), new GUIStyle()
        {
            padding = new RectOffset(5, 5, 10, 5)
        });

        internal static GUIStyle centeredLabel = _cache.Get(nameof(centeredLabel), new(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter
        });

        internal static GUIStyle horizontalPaddedArea => _cache.Get(nameof(horizontalPaddedArea), new GUIStyle()
        {
            padding = new RectOffset(7, 7, 2, 2),
            alignment = TextAnchor.MiddleCenter,
        });

        internal static GUIStyle wordWrappedMiniLabel => _cache.Get(nameof(wordWrappedMiniLabel), new GUIStyle(EditorStyles.wordWrappedMiniLabel)
        {
            richText = true,
        });

        internal static GUIStyle foldout => _cache.Get(nameof(foldout), new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        });

        internal static GUIStyle title => _cache.Get(nameof(title), new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            margin = new RectOffset(0, 0, 10, 10)
        });

        internal static GUIStyle componentTitle => _cache.Get(nameof(componentTitle), new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            margin = new RectOffset(0, 0, 0, 0)
        });

        internal static GUIStyle componentSubtitle => _cache.Get(nameof(componentSubtitle), new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            margin = new RectOffset(0, 0, 0, 0)
        });

        internal static GUIStyle array => _cache.Get(nameof(array), new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(0, 0, 3, 5)
        });
        internal static GUIStyle verticallyCenteredLabel => _cache.Get(nameof(verticallyCenteredLabel), new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
        });

        internal static GUIStyle centeredIconButton => _cache.Get(nameof(centeredIconButton), new GUIStyle(EditorStyles.iconButton)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = EditorGUIUtility.singleLineHeight + 2,
        });

        internal static GUIStyle miniButton => _cache.Get(nameof(miniButton), new GUIStyle(EditorStyles.miniButton)
        {
            padding = new RectOffset(2, 2, 2, 2),
            margin = new RectOffset(2, 2, 2, 2),
            fixedHeight = 18,
            fixedWidth = 20,
        });

        internal static GUIStyle nakedButton => _cache.Get(nameof(nakedButton), new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset(2, 2, 2, 2),
            margin = new RectOffset(0, 0, 0, 0),
            stretchWidth = true,
        });

        internal static GUIStyle blueButton => _cache.Get(nameof(blueButton), new GUIStyle(GUI.skin.button)
        {
            wordWrap = true,
            padding = new RectOffset(4, 4, 4, 4),
            margin = new RectOffset(0, 0, 0, 0),
            normal = {
                background = EditorTextures.Button(EditorColor.Blue, false),
                scaledBackgrounds = new[] { EditorTextures.Button(EditorColor.Blue, false) }
            },
            hover = {
                background = EditorTextures.Button(EditorColor.Blue, false),
                scaledBackgrounds = new[] { EditorTextures.Button(EditorColor.Blue, false) }
            },
            active = {
                background = EditorTextures.Button(EditorColor.Blue, false),
                scaledBackgrounds = new[] { EditorTextures.Button(EditorColor.Blue, false) }
            },
        });

        internal static GUIStyle headerButton => _cache.Get(nameof(headerButton), new GUIStyle(EditorStyles.miniButton)
        {
            padding = new RectOffset(2, 2, 2, 2),
            margin = new RectOffset(2, 2, 2, 2),
            fixedHeight = 24,
            fixedWidth = 24,
        });


        internal static GUIStyle iconButton => _cache.Get(nameof(iconButton), new GUIStyle(EditorStyles.iconButton)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 18,
            fixedWidth = 18,
        });

        internal static GUIStyle helpBox => _cache.Get(nameof(helpBox), new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(7, 7, 5, 5),
            margin = new RectOffset(0, 0, 5, 5),
            wordWrap = true,
            richText = true,
        });

        internal static GUIStyle contentHelpBox => _cache.Get(nameof(contentHelpBox), new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(12, 12, 7, 7),
            margin = new RectOffset(7, 7, 7, 7)
        });

        internal static GUIStyle clippedLabelStyle => _cache.Get(nameof(clippedLabelStyle), new GUIStyle(EditorStyles.label)
        {
            overflow = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            wordWrap = false,
            clipping = TextClipping.Clip
        });

        internal static GUIStyle menuBarButton => _cache.Get(nameof(menuBarButton), new GUIStyle(EditorStyles.iconButton)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedWidth = 0,
            fixedHeight = EditorStyles.toolbarButton.fixedHeight,
        });

        internal static GUIStyle toolbarTitle => _cache.Get(nameof(toolbarTitle), new GUIStyle(EditorStyles.toolbarButton)
        {
            stretchWidth = true,
            alignment = TextAnchor.MiddleLeft,
        });


        internal static GUIStyle lightBackground => _cache.Get(nameof(lightBackground), new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            stretchHeight = true,
            normal = { background = EditorTextures.grayTexture }
        });

        internal static GUIStyle darkBackground => _cache.Get(nameof(darkBackground), new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            stretchHeight = true,
            normal = { background = EditorTextures.darkerGrayTexture }
        });

        internal static GUIStyle bigLabel => _cache.Get(nameof(bigLabel), new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = true,
            padding = new RectOffset(20, 20, 20, 20),
        });

        internal static GUIStyle bigRedLabel => _cache.Get(nameof(bigRedLabel), new GUIStyle(bigLabel)
        {
            normal = { textColor = Color.red }
        });

        internal static GUIStyle bigButton => _cache.Get(nameof(bigButton), new GUIStyle(GUI.skin.button)
        {
            fontSize = 14,
            fixedHeight = 40,
        });

        internal static GUIStyle addComponentButton => _cache.Get(nameof(addComponentButton), new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fixedHeight = 26,
            fixedWidth = 230,
            padding = new RectOffset(5, 5, 5, 5),
            margin = new RectOffset(0, 0, 0, 0),
        });

        internal static GUIStyle labelRight => _cache.Get(nameof(labelRight), new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleRight,
        });

        internal static GUIStyle colorPickerButton => _cache.Get(nameof(colorPickerButton), new()
        {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            border = new RectOffset(0, 0, 0, 0),
            fixedWidth = 20,
            fixedHeight = 20
        });

#pragma warning restore IDE1006

        internal static GUIStyle IndentedHelpBox(int indentLevel)
        {
            string key = $"IndentedHelpBox_{indentLevel}";
            if (_cache.TryGetValue(key, out GUIStyle style)) return style;

            style = new GUIStyle(EditorStyles.helpBox);
            style.margin.left = EditorStyles.helpBox.margin.left + ((indentLevel + 1) * 12);

            _cache.Add(key, style);

            return style;
        }

        private static GUIStyle Border(BorderSide direction, RectOffset padding)
        {
            string key = $"{direction}_{padding.left},{padding.right},{padding.top},{padding.bottom}";
            if (!_cache.TryGetValue(key, out GUIStyle style))
            {
                Texture2D boxTex = direction == BorderSide.Top ? EditorTextures.BorderTop : EditorTextures.BorderBottom;
                style = new GUIStyle
                {
                    border = new RectOffset(5, 5, 5, 5),
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = padding,
                    normal = { background = boxTex }
                };
                _cache.Add(key, style);
            }
            return style;
        }//

        internal static GUIStyle Border(BorderSide direction)
        {
            RectOffset padding = new(
                7,
                7,
                direction == BorderSide.Top ? 7 : 10,
                direction == BorderSide.Top ? 7 : 4
            );

            return Border(direction, padding);
        }

        internal static GUIStyle Box(TextAnchor alignment = TextAnchor.MiddleLeft, int fontSize = 10, EditorColor color = EditorColor.None, RectOffset margin = null, RectOffset padding = null, float? height = null)
        {
            margin ??= _defaultBoxMargin;
            padding ??= _defaultBoxPadding;
            string key = $"box_{alignment}_{fontSize}_{color}_{margin.left},{margin.right},{margin.top},{margin.bottom}_{padding.left},{padding.right},{padding.top},{padding.bottom},{height}";
            if (!_cache.TryGetValue(key, out GUIStyle style))
            {
                style = new GUIStyle(EditorStyles.label)
                {
                    border = new RectOffset(5, 5, 5, 5),
                    margin = margin,
                    padding = padding,
                    fontSize = fontSize,
                    normal = { background = EditorTextures.Box(color) },
                    alignment = alignment,
                    wordWrap = true,
                    richText = true,
                };
                if (height.HasValue) style.fixedHeight = height.Value;
                _cache[key] = style;
            }
            return style;
        }

        internal static GUIStyle Box(EditorColor color, RectOffset margin = null, RectOffset padding = null)
            => Box(TextAnchor.UpperLeft, 12, color, margin, padding);

        internal static GUIStyle Box(TextAnchor alignment, EditorColor color, RectOffset margin = null, RectOffset padding = null)
            => Box(alignment, 12, color, margin, padding);

    }
}