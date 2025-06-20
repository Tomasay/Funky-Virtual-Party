using System;
using System.Collections.Generic;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;

namespace Glitch9.AIDevKit.Editor
{
    public partial class ModelCatalogueWindow
    {
        public class ModelCatalogueContextMenuHandler : TreeViewContextMenuHandler
        {
            public ModelCatalogueContextMenuHandler(ModelCatalogueTreeView treeView) : base(treeView) { }

            public override IEnumerable<ITreeViewContextMenu> CreateContextMenus()
            {
                yield return new TreeViewAction(TreeViewContextMenuType.Add)
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

                yield return new TreeViewSeparator();

                yield return new TreeViewAction()
                {
                    Name = "Update Metadata",
                    Action = UpdateMetadata,
                    ButtonIcon = EditorIcons.Refresh,
                };

                yield return new TreeViewAction(TreeViewContextMenuType.Delete)
                {
                    Name = "Delete Fine-Tuned Model from API",
                    Action = RemoveCustomModel,
                    Condition = CanRemoveCustomModel,
                    ConfirmationMessage = "Are you sure you want to delete the selected fine-tuned model(s) from the API? This action cannot be undone.",
                };

                yield return new TreeViewAction(TreeViewContextMenuType.Delete)
                {
                    Name = "Remove from Catalogue",
                    Condition = CanRemoveFromCatalogue,
                    Action = RemoveFromCatalogue,
                };
            }

            private void AddToLibrary(ModelCatalogueTreeViewItem[] items, Action<bool> onSuccess)
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

                    if (ModelCatalogueUtil.AddToLibrary(item))
                    {
                        successAtLeastOne = true;
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
            }

            private bool CanAddToLibrary(ModelCatalogueTreeViewItem[] items)
            {
                if (items.IsNullOrEmpty()) return false;

                foreach (var item in items)
                {
                    if (item == null || item.IsInvalid() || item.InMyLibrary || item.IsObsolete)
                    {
                        continue;
                    }

                    return true;
                }

                return false;
            }

            private void RemoveFromLibrary(ModelCatalogueTreeViewItem[] items, Action<bool> onSuccess)
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

                    if (ModelCatalogueUtil.RemoveFromLibrary(item))
                    {
                        successAtLeastOne = true;
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
            }

            private bool CanRemoveFromLibrary(ModelCatalogueTreeViewItem[] items)
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

            private bool CanRemoveCustomModel(ModelCatalogueTreeViewItem[] items)
            {
                if (items.IsNullOrEmpty()) return false;

                foreach (var item in items)
                {
                    if (item == null || !item.IsCustom)
                    {
                        continue;
                    }

                    return true;
                }

                return false;
            }

            private async void RemoveCustomModel(ModelCatalogueTreeViewItem[] items, Action<bool> onSuccess)
            {
                if (items.IsNullOrEmpty())
                {
                    onSuccess?.Invoke(false);
                    return;
                }

                bool successAtLeastOne = false;

                foreach (var item in items)
                {
                    if (item == null || !item.IsCustom)
                    {
                        continue;
                    }

                    if (await ModelCatalogueUtil.RemoveCustomModelAsync(item))
                    {
                        successAtLeastOne = true;
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
                _treeView.ReloadTreeView(true, true);
            }

            private bool CanRemoveFromCatalogue(ModelCatalogueTreeViewItem[] items)
            {
                if (items.IsNullOrEmpty()) return false;

                foreach (var item in items)
                {
                    if (item != null && (item.InMyLibrary || !item.CanDelete))
                    {
                        return false;
                    }
                }

                return true;
            }

            private void RemoveFromCatalogue(ModelCatalogueTreeViewItem[] items, Action<bool> onSuccess)
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

                    ModelCatalogue.Instance.RemoveEntry(item.Data);
                    successAtLeastOne = true;
                }

                onSuccess?.Invoke(successAtLeastOne);
                _treeView.ReloadTreeView(true, true);
            }

            private async void UpdateMetadata(ModelCatalogueTreeViewItem[] items, Action<bool> onSuccess)
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

                    if (await ModelCatalogueUtil.UpdateMetadataAsync(item))
                    {
                        successAtLeastOne = true;
                        // _treeView.ReloadDetailsWindow(item);
                    }
                }

                onSuccess?.Invoke(successAtLeastOne);
                _treeView.ReloadTreeView(true, true);
            }
        }
    }
}