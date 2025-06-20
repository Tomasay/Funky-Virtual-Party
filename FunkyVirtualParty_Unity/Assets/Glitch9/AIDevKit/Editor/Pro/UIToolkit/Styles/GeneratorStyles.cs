
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal static class GeneratorStyles
    {
        internal const string kStyleMarkerFileName = "styles_generator";
        internal const string Class_GridViewBoxLabelTitle = "grid-view-box-label-title";
        internal const string Class_GridViewBoxLabelContent = "grid-view-box-label-content";
        internal const string Class_GridViewBoxLabelDetails = "grid-view-box-label-details";

        private static readonly EPrefs<float> kInputPanelWidth = new("GeneratorGUI.InputPanelWidth", 400f);
        private static readonly EPrefs<GeneratorWindowType> kLastGeneratorWindowType = new("GeneratorGUI.LastGeneratorWindowType", GeneratorWindowType.CodeGenerator);
        internal static float InputPanelWidth { get => kInputPanelWidth.Value; set => kInputPanelWidth.Value = value; }
        internal static GeneratorWindowType LastGeneratorWindowType { get => kLastGeneratorWindowType.Value; set => kLastGeneratorWindowType.Value = value; }

        private static readonly IMGUIStyleCache _styleCache = new();

        internal static GUIStyle NodeTitle => _styleCache.Get("NodeTitle", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            margin = new RectOffset(0, 0, 0, 5),
        });

        internal static GUIStyle CodeStyle => _styleCache.Get("CodeStyle", new GUIStyle(EditorStyles.textArea)
        {
            richText = true,
            wordWrap = true,
            fontSize = 12,
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(4, 4, 4, 4),
            border = new RectOffset(4, 4, 4, 4),
            stretchHeight = true,
            stretchWidth = true,
            normal =
            {
                textColor = Color.white,
                background = EditorTextures.blackTexture,
                scaledBackgrounds = new[]
                {
                    EditorTextures.blackTexture,
                },
            },
        });
    }
}