using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public abstract partial class ExtendedTreeViewWindow<TTreeViewWindow, TTreeView, TTreeViewItem, TTreeViewDetailsWindow, TTreeViewData, TTreeViewItemFilter, TTreeViewContextMenuHandler>
    {
        public abstract partial class TreeViewContextMenuHandler
        {
            internal static class Util
            {
                private static class ConfirmationMessages
                {
                    internal const string ADD_ITEM = "Do you wish to add this item?";
                    internal const string REVERT_CHANGES = "Do you wish to revert changes?";
                    internal const string SAVE_ITEM = "Do you wish to save changes?";
                    internal const string DELETE_ITEM = "Do you really wish to delete this item?";
                    internal const string COPY_ITEM = "Do you wish to copy this item?";
                    internal const string PASTE_ITEM = "Do you wish to paste this item?";
                }

                private static class MenuNames
                {
                    internal const string ADD_ITEM = "Add";
                    internal const string SAVE_ITEM = "Save";
                    internal const string REVERT_CHANGES = "Revert Changes";
                    internal const string DELETE_ITEM = "Delete";
                    internal const string COPY_ITEM = "Copy";
                    internal const string PASTE_ITEM = "Paste";
                }

                internal static string GetDefaultConfirmationMessage(TreeViewContextMenuType type)
                {
                    return type switch
                    {
                        TreeViewContextMenuType.Add => ConfirmationMessages.ADD_ITEM,
                        TreeViewContextMenuType.Save => ConfirmationMessages.SAVE_ITEM,
                        TreeViewContextMenuType.Delete => ConfirmationMessages.DELETE_ITEM,
                        TreeViewContextMenuType.Copy => ConfirmationMessages.COPY_ITEM,
                        TreeViewContextMenuType.Paste => ConfirmationMessages.PASTE_ITEM,
                        TreeViewContextMenuType.Revert => ConfirmationMessages.REVERT_CHANGES,
                        _ => string.Empty
                    };
                }

                internal static string GetDefaultMenuName(TreeViewContextMenuType type)
                {
                    return type switch
                    {
                        TreeViewContextMenuType.Add => MenuNames.ADD_ITEM,
                        TreeViewContextMenuType.Save => MenuNames.SAVE_ITEM,
                        TreeViewContextMenuType.Delete => MenuNames.DELETE_ITEM,
                        TreeViewContextMenuType.Copy => MenuNames.COPY_ITEM,
                        TreeViewContextMenuType.Paste => MenuNames.PASTE_ITEM,
                        TreeViewContextMenuType.Revert => MenuNames.REVERT_CHANGES,
                        _ => string.Empty
                    };
                }

                internal static void ShowRightClickMenu(ref GenericMenu menu, IList<TTreeViewItem> items, Action<bool> refreshWindow, TreeViewAction contextMenu)
                {
                    if (!contextMenu.IsEmpty && contextMenu.ShowInContextMenu)
                    {
                        bool isVisible = contextMenu.IsVisible(items.ToArray());

                        if (isVisible)
                        {
                            menu.AddItem(new GUIContent(contextMenu.Name), false, () => contextMenu.Execute(refreshWindow, items.ToArray()));
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent(contextMenu.Name));
                        }
                    }
                }

                internal static void ShowDetailsWindowMenu(ref GenericMenu menu, TTreeViewItem item, Action<IResult> refreshWindow, TreeViewAction contextMenu, IResult customResult = null)
                {
                    if (!contextMenu.IsEmpty && contextMenu.ShowInDetailsWindow)
                    {
                        bool isVisible = contextMenu.IsVisible(item);

                        if (isVisible)
                        {
                            menu.AddItem(new GUIContent(contextMenu.Name), false, () => contextMenu.Execute((success) =>
                            {
                                refreshWindow(customResult ?? Result.Success());
                            }, item));
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent(contextMenu.Name));
                        }
                    }
                }
            }
        }
    }
}