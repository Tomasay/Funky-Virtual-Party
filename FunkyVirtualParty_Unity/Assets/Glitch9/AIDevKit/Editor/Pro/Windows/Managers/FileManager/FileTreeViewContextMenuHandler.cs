using System;
using System.Collections.Generic;
using Glitch9.Editor.IMGUI;

namespace Glitch9.AIDevKit.Editor.Files
{
    public partial class FileManagerWindow
    {
        public class FileTreeViewContextMenuHandler : TreeViewContextMenuHandler
        {
            public FileTreeViewContextMenuHandler(FileTreeView treeView) : base(treeView) { }

            public override IEnumerable<ITreeViewContextMenu> CreateContextMenus()
            {
                yield return new TreeViewAction(TreeViewContextMenuType.Delete)
                {
                    Action = DeleteItems,
                    ShowConfirmationMessage = true,
                    ConfirmationMessage = "Are you sure you want to delete the selected file(s)?",
                };
            }

            private async void DeleteItems(FileTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                bool successAtLeastOne = false;

                foreach (var item in items)
                {
                    if (item == null) continue;
                    ApiFile file = item.Data;
                    if (file == null) continue;

                    bool success = await FileManager.DeleteFileAsync(file);
                    if (success) successAtLeastOne = true;
                }

                onSuccess?.Invoke(successAtLeastOne);
                if (successAtLeastOne) _treeView.ReloadTreeView(true, true);
            }
        }
    }
}