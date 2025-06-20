using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class IconButton : Button
    {
        private readonly Image _icon;

        public IconButton(System.Action onClick, Texture2D icon, string name = null, string tooltip = null) : base(onClick)
        {
            this.name = name ?? "icon-button";
            pickingMode = PickingMode.Position;
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.paddingLeft = 2;
            style.paddingRight = 2;

            _icon = new Image
            {
                image = icon,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 18,
                    height = 18,
                }
            };

            Clear();
            Add(_icon);

            if (!string.IsNullOrEmpty(tooltip))
                this.tooltip = tooltip;
        }

        public void SetIcon(Texture2D texture)
        {
            _icon.image = texture;
        }
    }
}
