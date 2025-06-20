using Glitch9.Editor;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class BackgroundGeneratorWindow : ImageGeneratorWindowBase<BackgroundGeneratorWindow>
    {
        protected override string FormatPrompt(string prompt)
        {
            const string kPromptFormat = "Generate a background illustration for a {gameTheme} {gameGenre} game in the {artStyle} style: {prompt}";

            return kPromptFormat
                .Replace("{gameTheme}", GameTheme.FormatEnum())
                .Replace("{gameGenre}", GameGenre.FormatFlagsEnum())
                .Replace("{artStyle}", ArtStyle.FormatFlagsEnum())
                .Replace("{prompt}", prompt)
                .Replace("  ", " ");
        }

        protected override void DrawIMGUIApiSettingsBefore()
        {
            if (Model == null) return;

            if (Model.Api == Api.OpenAI)
            {
                Size = AIDevKitGUI.ImageSizePopup(Size, Model);
            }
            else if (Model.Api == Api.Google)
            {
                AspectRatio = ExGUILayout.EnumPopup("Aspect Ratio", AspectRatio);
            }
        }
    }
}