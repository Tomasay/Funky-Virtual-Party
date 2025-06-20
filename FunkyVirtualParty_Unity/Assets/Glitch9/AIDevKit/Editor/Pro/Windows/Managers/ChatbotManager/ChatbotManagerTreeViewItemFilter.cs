using Glitch9.Editor.IMGUI;
using UnityEditor.IMGUI.Controls;

namespace Glitch9.AIDevKit.Editor.Chatbots
{
    public class ChatbotManagerTreeViewItemFilter : TreeViewItemFilter
    {
        public string Name { get; set; }
        public string Model { get; set; }

        public override bool IsVisible(TreeViewItem item)
        {
            if (item is not ChatbotManagerTreeViewItem ChatbotItem) return false;
            //if (!string.IsNullOrEmpty(Name) && !ChatbotItem.Data.Name.Contains(Name)) return false;
            //if (Model != null && ChatbotItem.Data.Model != Model) return false;
            return base.IsVisible(item);
        }
    }
}