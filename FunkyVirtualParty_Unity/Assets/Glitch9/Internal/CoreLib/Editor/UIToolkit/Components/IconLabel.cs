using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class IconLabel : VisualElement
    {
        public Image Icon { get; private set; }
        public Label TextLabel { get; private set; }

        public IconLabel(Texture2D icon, string text, string tooltip = null, int iconSize = 16, int spacing = 2)
        {
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.FlexStart;

            this.tooltip = tooltip;

            Icon = new Image
            {
                image = icon,
                scaleMode = ScaleMode.ScaleToFit,
                style =
            {
                width = iconSize,
                height = iconSize,
                marginRight = spacing,
            }
            };

            TextLabel = new Label(text)
            {
                style =
            {
                unityTextAlign = TextAnchor.MiddleLeft
            }
            };

            Add(Icon);
            Add(TextLabel);
        }
    }
}