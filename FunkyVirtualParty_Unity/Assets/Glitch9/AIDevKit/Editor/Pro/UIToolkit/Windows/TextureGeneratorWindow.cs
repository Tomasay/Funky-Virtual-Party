using Glitch9.Editor;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class TextureGeneratorWindow : ImageGeneratorWindowBase<TextureGeneratorWindow>
    {
        private EPrefs<bool> _isTileable;
        private EPrefs<bool> IsTileable => _isTileable ??= new EPrefs<bool>($"{nameof(TextureGeneratorWindow)}.IsTileable", false);
        private AnimatedToggle _tileableToggle;
        private AnimatedToggle TileableToggle => _tileableToggle ??= new(IsTileable.Value);

        protected override void DrawIMGUIApiSettingsAfter()
        {
            //IsTileable.Value = EditorGUILayout.ToggleLeft("Tileable", IsTileable.Value);
            IsTileable.Value = TileableToggle.DrawGUI(new UnityEngine.GUIContent("Tileable"), EditorStyles.label);
        }

        protected override string FormatPrompt(string prompt)
        {
            const string kPromptFormat = "Generate a {tileable} mesh texture for a {gameTheme} {gameGenre} game in the {artStyle} style: {prompt}";

            return kPromptFormat
                .Replace("{tileable}", IsTileable.Value ? "tileable" : "non-tileable")
                .Replace("{gameTheme}", GameTheme.FormatEnum())
                .Replace("{gameGenre}", GameGenre.FormatFlagsEnum())
                .Replace("{artStyle}", ArtStyle.FormatFlagsEnum())
                .Replace("{prompt}", prompt)
                .Replace("  ", " ");
        }
    }
}