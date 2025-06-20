using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public class TreeViewStyles
    {
        private static class ExGUISkinStyles
        {
            internal const string TREEVIEW_ITEM = "treeviewitem";
            internal const string TREEVIEW_GROUP = "treeviewgroup";
        }

        private static readonly Dictionary<string, GUIStyle> _styles = new();
        private static GUIStyle Get(string key, GUIStyle defaultStyle)
        {
            if (!_styles.TryGetValue(key, out GUIStyle style))
            {
                style = new GUIStyle(defaultStyle);
                _styles[key] = style;
            }
            return style;
        }

        public static GUIStyle ToolbarDropDown => Get(nameof(ToolbarDropDown), new(EditorStyles.toolbarDropDown)
        {
            padding = new RectOffset(6, 6, 2, 2),
            fontSize = 11,
        });

        public static GUIStyle BottomBarStyle => Get(nameof(BottomBarStyle), new GUIStyle(ExStyles.Border(BorderSide.Bottom))
        {
            fixedHeight = 34,
        });

        public static GUIStyle DetailsWindowTitle => Get(nameof(DetailsWindowTitle), new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            //fixedHeight = 20,
            wordWrap = true,
            stretchWidth = true,
        });

        public static GUIStyle ChildWindowSubtitleLeft => Get(nameof(ChildWindowSubtitleLeft), new GUIStyle(ExStyles.clippedLabelStyle)
        {
            fontSize = 11,
        });

        public static GUIStyle ChildWindowSubtitleRight => Get(nameof(ChildWindowSubtitleRight), new GUIStyle(ExStyles.clippedLabelStyle)
        {
            fontSize = 11,
            alignment = TextAnchor.MiddleRight
        });

        public static GUIStyle EditWindowBody => Get(nameof(EditWindowBody), new GUIStyle()
        {
            padding = new RectOffset(5, 5, 10, 5)
        });

        public static GUIStyle WordWrapTextField => Get(nameof(WordWrapTextField), new GUIStyle(GUI.skin.textField)
        {
            wordWrap = true
        });

        internal static GUIStyle FindAndReplaceLayout => Get(nameof(FindAndReplaceLayout), new GUIStyle()
        {
            padding = new RectOffset(5, 5, 0, 5)
        });

        internal static GUIStyle TalkBubbleStyle => Get(nameof(TalkBubbleStyle), new GUIStyle()
        {
            normal =
            {
                textColor = ExColor.royalpurple,
                background = EditorTextures.Box(EditorColor.Purple),
                scaledBackgrounds = new Texture2D[] { EditorTextures.Box(EditorColor.Purple) }
            },
            margin = new RectOffset(5, 5, 0, 0),
            border = new RectOffset(5, 5, 5, 5),
            padding = new RectOffset(5, 5, 5, 5),
            fontSize = 11,
            wordWrap = true,
            stretchWidth = true,
            stretchHeight = true
        });

        internal static GUIStyle PageBarStyle => Get(nameof(PageBarStyle), new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5, 5, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            border = new RectOffset(5, 5, 5, 5),
            stretchWidth = true
        });

        internal static GUIStyle HeaderLabel => Get(nameof(HeaderLabel), new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            margin = new RectOffset(0, 0, 0, 0),
            border = new RectOffset(0, 0, 0, 0),
            richText = true
        });

        internal static GUIStyle ContextButton => Get(nameof(ContextButton), new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset(5, 5, 5, 5),
            stretchWidth = true,
            fixedHeight = 28,
        });

        internal static GUIStyle StringCell => Get(nameof(StringCell), new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = false,
            clipping = TextClipping.Clip,
        });


        //public static GUIStyle TextField(int fontSize = 12, GUIColor color = GUIColor.None) => GetTextFieldStyle(fontSize, color, 3, 3);
        public static GUIStyle TextField(int fontSize = 12, EditorColor color = EditorColor.None)
        {
            const int leftOverflow = 3;
            const int rightOverflow = 3;

            string key = $"textfield_{fontSize}_{color}_{leftOverflow}_{rightOverflow}";

            return Get(key, new GUIStyle(EditorStyles.textArea)
            {
                border = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(4, 4, 4, 4),
                overflow = new RectOffset(leftOverflow, rightOverflow, 0, 0),
                stretchWidth = true,
                stretchHeight = true,
                fontSize = fontSize,
                alignment = TextAnchor.UpperLeft,
                richText = true,
                wordWrap = true
            });
        }
    }
}