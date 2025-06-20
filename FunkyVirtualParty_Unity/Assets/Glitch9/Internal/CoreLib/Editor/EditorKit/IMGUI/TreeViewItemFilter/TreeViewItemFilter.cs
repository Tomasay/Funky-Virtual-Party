using UnityEditor.IMGUI.Controls;

namespace Glitch9.Editor.IMGUI
{
    /// <summary>
    /// Used to be an interface
    /// </summary>
    public class TreeViewItemFilter
    {
        public string SearchText { get; set; }

        public virtual bool IsVisible(TreeViewItem item)
        {
            if (string.IsNullOrEmpty(SearchText)) return true;
            if (item.displayName.ToLower().Contains(SearchText.ToLower())) return true;
            return false;
        }
    }
}