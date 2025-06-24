using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public static class UIToolkitFactory
    {
        public static Label SelectableLabel(string text, string className = null)
        {
            var label = new Label(text);
            label.selection.isSelectable = true;
            if (!string.IsNullOrEmpty(className)) label.AddToClassList(className);
            return label;
        }
    }
}