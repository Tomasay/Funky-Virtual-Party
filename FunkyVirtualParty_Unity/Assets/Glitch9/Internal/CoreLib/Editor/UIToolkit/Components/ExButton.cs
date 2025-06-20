using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class ExButton : Button
    {
        private readonly Image _icon;
        private readonly Label _label;

        public ExButton(System.Action onClick, string name = null, Texture2D icon = null, string label = "", string tooltip = null) : base(onClick)
        {
            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(label))
            {
                this.name = label.ToSnakeCase();
            }
            else
            {
                this.name = name ?? "ex-button";
            }

            pickingMode = PickingMode.Position;
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;

            _icon = new Image
            {
                image = icon,
                style =
                {
                    width = 16,
                    height = 16,
                    marginRight = 4
                }
            };

            _label = new Label(label)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };

            Clear();
            if (icon != null) Add(_icon);
            if (!string.IsNullOrEmpty(label)) Add(_label);

            if (!string.IsNullOrEmpty(tooltip))
                this.tooltip = tooltip;
        }

        public void SetIcon(Texture2D texture)
        {
            _icon.image = texture;
            if (!_icon.parent?.Contains(_icon) ?? true)
                Insert(0, _icon);
        }

        public void SetLabel(string text)
        {
            _label.text = text;
        }
    }
}
