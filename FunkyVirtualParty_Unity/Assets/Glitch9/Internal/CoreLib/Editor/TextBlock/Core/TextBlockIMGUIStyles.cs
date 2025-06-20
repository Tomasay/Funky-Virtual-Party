using UnityEngine;

namespace Glitch9.Editor
{
    internal static class TextBlockIMGUIStyles
    {
        private static readonly IMGUIStyleCache _cache = new();

        internal static GUIStyle PlainText => _cache.Get(nameof(PlainText), new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
            richText = true,
            stretchWidth = true,
            stretchHeight = true,
            alignment = TextAnchor.UpperLeft,
        });

        internal static GUIStyle UList => _cache.Get(nameof(UList), new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
            richText = true,
            margin = new RectOffset(0, 0, 0, 6),
            padding = new RectOffset(0, 0, 0, 6),
        });

        internal static GUIStyle UListDot => _cache.Get(nameof(UListDot), new GUIStyle(GUI.skin.label)
        {
            fixedWidth = 16,
            fixedHeight = 10,
        });

        internal static GUIStyle HeaderLabel => _cache.Get(nameof(HeaderLabel), new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
            richText = true,
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            fixedHeight = 18,
            padding = new RectOffset(3, 3, 1, 1),
            margin = new RectOffset(0, 0, 0, 5),
            alignment = TextAnchor.MiddleLeft,
        });

        internal static GUIStyle CodeBlockHeaderButton => _cache.Get(nameof(CodeBlockHeaderButton), new GUIStyle()
        {
            margin = new RectOffset(0, 0, 4, 0),
            fixedWidth = 18,
            fixedHeight = 16,
            alignment = TextAnchor.LowerRight,
        });

        internal static GUIStyle CodeBlockHeader => _cache.Get(nameof(CodeBlockHeader), new GUIStyle()
        {
            fontSize = 10,
            fixedHeight = 22,
            margin = new RectOffset(4, 11, 10, 0),
            padding = new RectOffset(6, 6, 3, 3),
            border = new RectOffset(5, 5, 5, 5),
            normal = { background = EditorTextures.CodeBlockHeaderTexture, textColor = new Color(0.8f, 0.8f, 0.8f) },
        });

        internal static GUIStyle CodeBlockContent => _cache.Get(nameof(CodeBlockContent), new GUIStyle()
        {
            richText = true,
            margin = new RectOffset(3, 10, 0, 10),
            padding = new RectOffset(8, 8, 6, 6),
            border = new RectOffset(5, 5, 5, 5),
            fontSize = 12,
            wordWrap = true,
            normal = { background = EditorTextures.CodeBlockBodyTexture, textColor = Color.white },
        });
    }
}