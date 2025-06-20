using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class InfoLabel : VisualElement
    {
        public Label KeyLabel { get; }
        public Label ValueLabel { get; }

        public InfoLabel(string key, string value, string tooltip = null, string keyClass = "info-label-key", string valueClass = "info-label-value")
        {
            this.tooltip = tooltip;

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.flexGrow = 1;

            KeyLabel = new Label(key);
            KeyLabel.AddToClassList(keyClass);

            ValueLabel = new Label(value);
            ValueLabel.AddToClassList(valueClass);

            Add(KeyLabel);
            Add(ValueLabel);
        }

        public void SetValue(string value)
        {
            ValueLabel.text = value;
        }

        public void SetKey(string key)
        {
            KeyLabel.text = key;
        }
    }
}