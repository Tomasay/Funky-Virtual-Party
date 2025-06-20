using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    public partial class VoiceCatalogueWindow
    {
        public class VoiceCatalogueContextMenuHandler : TreeViewContextMenuHandler
        {
            public VoiceCatalogueContextMenuHandler(VoiceCatalogueTreeView treeView) : base(treeView)
            {
            }

            public override IEnumerable<ITreeViewContextMenu> CreateContextMenus()
            {
                yield return new TreeViewAction()
                {
                    Name = "Add to Library",
                    Action = AddToLibrary,
                    Condition = CanAddToLibrary,
                    ShowInDetailsWindow = false,
                };
                yield return new TreeViewAction()
                {
                    Name = "Remove from Library",
                    Action = RemoveFromLibrary,
                    Condition = CanRemoveFromLibrary,
                    ShowInDetailsWindow = false,
                };
            }

            private void AddToLibrary(VoiceCatalogueTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                bool successAtLeastOne = false;

                foreach (var item in items)
                {
                    if (item == null || item.IsInvalid())
                    {
                        continue;
                    }

                    if (VoiceCatalogueUtil.AddToLibrary(item))
                    {
                        successAtLeastOne = true;
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
            }

            private bool CanAddToLibrary(VoiceCatalogueTreeViewItem[] items)
            {
                if (items.IsNullOrEmpty()) return false;

                foreach (var item in items)
                {
                    if (item == null || item.IsInvalid() || item.InMyLibrary)
                    {
                        continue;
                    }

                    return true;
                }

                return false;
            }

            private void RemoveFromLibrary(VoiceCatalogueTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                bool successAtLeastOne = false;

                foreach (var item in items)
                {
                    if (item == null || item.IsInvalid())
                    {
                        continue;
                    }

                    if (VoiceCatalogueUtil.RemoveFromLibrary(item))
                    {
                        successAtLeastOne = true;
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
            }

            private bool CanRemoveFromLibrary(VoiceCatalogueTreeViewItem[] items)
            {
                if (items.IsNullOrEmpty()) return false;

                foreach (var item in items)
                {
                    if (item == null || item.IsInvalid() || !item.InMyLibrary || !item.CanDelete)
                    {
                        continue;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}