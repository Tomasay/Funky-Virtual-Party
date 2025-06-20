using System.Collections.Generic;
using Glitch9.Editor.IMGUI;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.Pro
{
    public partial class PromptHistoryWindow
    {
        public class PromptHistoryContextMenuHandler : TreeViewContextMenuHandler
        {
            public PromptHistoryContextMenuHandler(PromptHistoryTreeView treeView) : base(treeView)
            {
            }

            public override IEnumerable<ITreeViewContextMenu> CreateContextMenus()
            {
                yield return new TreeViewAction("Copy Prompt", "Copy the prompt to the clipboard")
                {
                    Type = TreeViewContextMenuType.Copy,
                    ShowInDetailsWindow = false,
                    Action = (items, onSuccess) =>
                    {
                        if (items.Length == 1)
                        {
                            EditorGUIUtility.systemCopyBuffer = items[0].Data.InputText;
                        }
                        onSuccess(true);
                    },
                    Condition = items => items.Length == 1
                };

                yield return new TreeViewAction("Copy Response", "Copy the response to the clipboard")
                {
                    Type = TreeViewContextMenuType.Copy,
                    ShowInDetailsWindow = false,
                    Action = (items, onSuccess) =>
                    {
                        if (items.Length == 1)
                        {
                            EditorGUIUtility.systemCopyBuffer = items[0].Data.OutputText;
                        }
                        onSuccess(true);
                    },
                    Condition = items => items.Length == 1
                };

                yield return new TreeViewAction(TreeViewContextMenuType.Delete, "Delete", "Delete the selected prompt(s)")
                {
                    Type = TreeViewContextMenuType.Delete,
                    Condition = items => true,
                    Action = (items, onSuccess) =>
                    {
                        foreach (var item in items)
                        {
                            PromptHistory.Remove(item.Data);
                        }

                        _treeView.ReloadTreeView(true, true);
                    }
                };
            }
        }
    }
}