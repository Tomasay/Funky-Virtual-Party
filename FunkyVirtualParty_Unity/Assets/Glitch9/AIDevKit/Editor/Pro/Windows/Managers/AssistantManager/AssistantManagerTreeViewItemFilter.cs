using Glitch9.Editor.IMGUI;
using UnityEditor.IMGUI.Controls;

namespace Glitch9.AIDevKit.Editor.Assistants
{
    public class AssistantManagerTreeViewItemFilter : TreeViewItemFilter
    {
        public string Name { get; set; }
        public string Model { get; set; }

        public override bool IsVisible(TreeViewItem item)
        {
            if (item is not AssistantManagerTreeViewItem assistantItem) return false;
            //if (!string.IsNullOrEmpty(Name) && !assistantItem.Data.Name.Contains(Name)) return false;
            //if (Model != null && assistantItem.Data.Model != Model) return false;
            return base.IsVisible(item);
        }
    }
}