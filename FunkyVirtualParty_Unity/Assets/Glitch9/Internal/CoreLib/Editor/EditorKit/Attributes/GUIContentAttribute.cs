using UnityEngine;

namespace Glitch9.Editor
{
    public class GUIContentAttribute : PropertyAttribute
    {
        public string Text { get; private set; }
        public string Tooltip { get; private set; }
        public string Icon { get; private set; }

        public GUIContentAttribute(string text, string tooltip = null, string icon = null)
        {
            Text = text;
            Tooltip = tooltip;
            Icon = icon;
        }
    }
}