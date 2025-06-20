using System;
using UnityEngine;

namespace Glitch9.Editor
{
    public class ButtonEntry
    {
        public string Label { get; set; }
        public Texture2D Icon { get; set; }
        public string Tooltip { get; set; }
        public Action Callback { get; set; }
        public Func<bool> IsEnabled { get; set; }

        public ButtonEntry() { }
        public ButtonEntry(string label, Action callback, Func<bool> isEnabled = null)
        {
            Label = label;
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            IsEnabled = isEnabled ?? (() => true);
        }

        public ButtonEntry(string label, Texture2D icon, Action callback, Func<bool> isEnabled = null)
        {
            Label = label;
            Icon = icon;
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            IsEnabled = isEnabled ?? (() => true);
        }

        public ButtonEntry(string label, string tooltip, Action callback, Func<bool> isEnabled = null)
        {
            Label = label;
            Tooltip = tooltip;
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            IsEnabled = isEnabled ?? (() => true);
        }

        public ButtonEntry(string label, Texture2D icon, string tooltip, Action callback, Func<bool> isEnabled = null)
        {
            Label = label;
            Icon = icon;
            Tooltip = tooltip;
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            IsEnabled = isEnabled ?? (() => true);
        }
    }
}