using Glitch9.Editor.IMGUI;
using UnityEditor.IMGUI.Controls;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public class PromptHistoryTreeViewItemFilter : TreeViewItemFilter
    {
        public override bool IsVisible(TreeViewItem item)
        {
            if (item is not PromptHistoryTreeViewItem i) return false;

            if (!PromptHistoryTreeViewSettings.ShowOpenAI && i.Api != Api.OpenAI) return false;
            if (!PromptHistoryTreeViewSettings.ShowGoogle && i.Api != Api.Google) return false;
            if (!PromptHistoryTreeViewSettings.ShowElevenLabs && i.Api != Api.ElevenLabs) return false;
            if (!PromptHistoryTreeViewSettings.ShowOllama && i.Api != Api.Ollama) return false;
            if (!PromptHistoryTreeViewSettings.ShowOpenRouter && i.Api != Api.OpenRouter) return false;

            return base.IsVisible(item);
        }
    }
}