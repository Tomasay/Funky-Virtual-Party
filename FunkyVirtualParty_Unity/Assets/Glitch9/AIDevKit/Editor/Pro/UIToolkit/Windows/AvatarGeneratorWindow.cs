

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class AvatarGeneratorWindow : ImageGeneratorWindowBase<AvatarGeneratorWindow>
    {
        protected override ImageSize Size => ImageSize._1024x1024;
        protected override Google.AspectRatio AspectRatio => Google.AspectRatio.Square;


        protected override string FormatPrompt(string prompt)
        {
            const string kPromptFormat = "Generate a stylized game character portrait for a {gameTheme} {gameGenre} game in the {artStyle} style: {prompt}";

            return kPromptFormat
                .Replace("{gameTheme}", GameTheme.FormatEnum())
                .Replace("{gameGenre}", GameGenre.FormatFlagsEnum())
                .Replace("{artStyle}", ArtStyle.FormatFlagsEnum())
                .Replace("{prompt}", prompt)
                .Replace("  ", " ");
        }
    }
}